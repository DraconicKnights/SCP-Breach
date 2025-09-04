using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP_Breach.Features.Duels;

public class DuelControl
{
    private static float GetGracePeriod => 25f;

        public static void StartDuel(Player player1, Player player2)
        {
            Vector3? location = GetAvailableLocation();

            if (location == null) return;

            DuelArray.OriginalPositions[player1] = player1.Position;
            DuelArray.OriginalPositions[player2] = player2.Position;

            DuelArray.OriginalRoles[player1] = player1.Role;
            DuelArray.OriginalRoles[player2] = player2.Role;

            StorePlayerItems(player1);
            StorePlayerItems(player2);

            DuelArray.InDuel.Add(player1, player2);

            if (SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.TutorialOnly)
            {
                player1.SetRole(RoleTypeId.Tutorial);
                player2.SetRole(RoleTypeId.Tutorial);
            }
            else
            {
                AssignDuelRoles(player1, player2);
            }
            
            player1.ClearInventory();
            player2.ClearInventory();

            if (SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.GetSameWeapon)
            {
                var randomWeapon = GetRandomWeapon();
                GiveWeapon(player1, randomWeapon);
                GiveWeapon(player2, randomWeapon);
            }
            else
            {
                GiveWeapon(player1, GetRandomWeapon());
                GiveWeapon(player2, GetRandomWeapon());
            }
            
            StorePlayerDuelItemsForCleanup(player1);
            StorePlayerDuelItemsForCleanup(player2);

            Timing.CallDelayed(0.5f, () =>
            {
                player1.Position = location.Value + new Vector3(2, 0, 0);
                player2.Position = location.Value - new Vector3(2, 0, 0);

                player1.IsGodModeEnabled = true;
                player2.IsGodModeEnabled = true;

                player1.SendHint("You have a short grace period", 4);
                player2.SendHint("You have a short grace period", 4);

                Timing.CallDelayed(GetGracePeriod, () =>
                {
                    player1.IsGodModeEnabled = false;
                    player2.IsGodModeEnabled = false;

                    player1.SendHint("The duel has started!", 4);
                    player2.SendHint("The duel has started!", 4);

                    Timing.RunCoroutine(MonitorDuel(player1, player2, location.Value));
                });
            });
        }

        private static void EndDuel(Player winner, Player loser, Vector3 duelLocation)
        {
            loser.SendHint("You lost the duel.", 5);
            winner.SendHint("You won the duel!", 5);

            Timing.CallDelayed(5f, () =>
            {
                if (DuelArray.OriginalRoles.TryGetValue(winner, out var winnerRole))
                    winner.SetRole(winnerRole);

                if (DuelArray.OriginalRoles.TryGetValue(loser, out var loserRole))
                    loser.SetRole(loserRole);

                if (DuelArray.OriginalPositions.TryGetValue(winner, out var winnerPosition))
                    winner.Position = winnerPosition;

                if (DuelArray.OriginalPositions.TryGetValue(loser, out var loserPosition))
                    loser.Position = loserPosition;

                AddOriginalItems(winner);
                AddOriginalItems(loser);

                RemovePlayerFromDuel(winner);
                RemovePlayerFromDuel(loser);

                ReleaseDuelLocation(duelLocation);
            });
        }

        private static void StorePlayerDuelItemsForCleanup(Player player)
        {
            if (!DuelArray.PlayerDuelItems.ContainsKey(player))
            {
                DuelArray.PlayerDuelItems[player] = new List<Item>();
            }

            foreach (var item in player.Items)
            {
                DuelArray.PlayerDuelItems[player].Add(item);
            }

        } 

        private static void StorePlayerItems(Player player)
        {
            if (!DuelArray.OriginalItems.ContainsKey(player))
            {
                DuelArray.OriginalItems[player] = player.Items.ToList();
            }
        }

        private static void AddOriginalItems(Player player)
        {
            if (DuelArray.OriginalItems.TryGetValue(player, out var items))
            {
                foreach (var item in items)
                {
                    player.AddItem(item.Type);
                }

                DuelArray.OriginalItems.Remove(player);
            }
        }

        private static void RemovePlayerFromDuel(Player player)
        {
            if (DuelArray.InDuel.TryGetValue(player, out var opponent))
            {
                DuelArray.InDuel.Remove(opponent);
            }

            DuelArray.InDuel.Remove(player);
            DuelArray.OriginalRoles.Remove(player);
            DuelArray.OriginalPositions.Remove(player);
            DuelArray.OriginalItems.Remove(player);

            //CleanupDroppedItemsAtLocation(player);
        }

        private static void CleanupDroppedItemsAtLocation(Player player)
        {
            Timing.CallDelayed(5f, () =>
            {
                Logger.Info("Checking for items at location: " + player.Position + "");

                if (DuelArray.PlayerDuelItems.TryGetValue(player, out var items))
                {
                    if (items == null || items.Count == 0)
                    {
                        Logger.Info("No items found for this player.");
                        return; // Exit if there are no items
                    }

                    Logger.Info("Cleaning up items for player: " + player.Nickname);

                    foreach (var item in items)
                    {
                        try
                        {
                            Logger.Info($"Item {item.Type.ToString()} has been found");

                            // Check if the item is the expected type
                            if (item == null)
                            {
                                Logger.Info("Item is null, skipping...");
                                continue; // Skip null items
                            }

                            // Destroy the item's GameObject if it's valid
                            if (item?.GameObject != null)
                            {
                                UnityEngine.Object.Destroy(item.GameObject);
                                Logger.Info($"Item {item.Type.ToString()} has been destroyed");
                            }
                            else
                            {
                                Logger.Info($"Item {item.Type.ToString()} has been destroyed");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Error while processing item {item?.Type.ToString()}: {ex.Message}\n{ex.StackTrace}");
                        }
                    }

                    DuelArray.PlayerDuelItems.Remove(player);
                }
                else
                {
                    Logger.Info("No items found for this player.");
                }
            });
        }

        private static IEnumerator<float> MonitorDuel(Player player1, Player player2, Vector3 duelLocation)
        {
            while (DuelArray.InDuel.ContainsKey(player1) || DuelArray.InDuel.ContainsKey(player2))
            {
                if (!player1.IsAlive || !player2.IsAlive)
                {
                    Player winner = player1.IsAlive ? player1 : player2;
                    Player loser = player1.IsAlive ? player2 : player1;

                    float itemRemovalRadius = 5f;
                    Vector3 duelCenter = loser.Position;

                    /*foreach (var item in Server.S)
                    {
                        if (Vector3.Distance(item.gameObject.transform.position, duelCenter) <= itemRemovalRadius)
                        {
                            Server.Instance.RemoveItem(item);
                        }
                    }*/

                    EndDuel(winner, loser, duelLocation);
                    yield break;
                }

                yield return Timing.WaitForSeconds(10f);
            }
        }

        private static Vector3? GetAvailableLocation()
        {
            foreach (var location in SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.DuelLocations.Where(location =>
                         !DuelArray._locationsInUse.Contains(location)))
            {
                DuelArray._locationsInUse.Add(location);
                return location;
            }

            return null;
        }

        private static void ReleaseDuelLocation(Vector3 location)
        {
            DuelArray._locationsInUse.Remove(location);
        }
        
        private static ItemType GetRandomWeapon()
        {
            // Retrieve the loot pool from the config
            var lootPool = SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.WeaponPool;

            if (lootPool == null || lootPool.Count == 0)
                throw new InvalidOperationException("Loot pool is empty or not defined!");

            // Select a random weapon from the pool
            return lootPool[UnityEngine.Random.Range(0, lootPool.Count)];
        }

        private static void GiveWeapon(Player player, ItemType weapon)
        {
            // Add the weapon to the player
            player.AddItem(weapon);
            player.SendHint($"You have been given a {weapon.ToString()}", 5);
            
            var allItems = (ItemType[])Enum.GetValues(typeof(ItemType));
            foreach (var item in allItems)
            {
                if (item.ToString().Contains("Ammo"))
                {
                    ushort ammoAmount = 50;
                    player.AddAmmo(item, ammoAmount);
                    Logger.Info("Added " + ammoAmount + " of " + item + " to " + player.Nickname + ".");
                }
            }
        }
        
        private static void AssignDuelRoles(Player player1, Player player2)
        {
            // Get the list of available duel roles from the config
            var availableRoles = SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.DuelRoles;

            // Separate roles by team
            var chaosRoles = availableRoles.Where(role => role.GetTeam() == Team.ChaosInsurgency).ToList();
            var foundationRoles = availableRoles.Where(role => role.GetTeam() == Team.FoundationForces).ToList();
            var classDRoles = availableRoles.Where(role => role.GetTeam() == Team.ClassD).ToList();
            var scientistRoles = availableRoles.Where(role => role.GetTeam() == Team.Scientists).ToList();

            // Ensure both team role lists have roles available
            if (chaosRoles.Count == 0 || foundationRoles.Count == 0 || classDRoles.Count == 0 || !scientistRoles.Any())
            {
                Logger.Error("Please check your duel roles configuration and try again.");
                return;
            }

            // Assign roles based on allowed teams
            var player1Team = GetRandomTeam();
            var player2Team = GetOpposingTeam(player1Team);

            RoleTypeId player1Role = GetRandomRoleForTeam(player1Team, chaosRoles, foundationRoles, classDRoles, scientistRoles);
            RoleTypeId player2Role = GetRandomRoleForTeam(player2Team, chaosRoles, foundationRoles, classDRoles, scientistRoles);

            if (player1Role == RoleTypeId.None || player2Role == RoleTypeId.None)
            {
                Logger.Error("Failed to assign duel roles due to missing or invalid configuration.");
                return;
            }

            player1.SetRole(player1Role);
            player2.SetRole(player2Role);

            // Log the role assignments for debugging
            
            Logger.Debug($"Assigned {player1.Nickname} as {player1Role} (Team: {player1Role.GetTeam()})");
            Logger.Debug($"Assigned {player2.Nickname} as {player2Role} (Team: {player2Role.GetTeam()})");
        }

        private static Team GetRandomTeam()
        {
            return UnityEngine.Random.Range(0, 2) == 0 ? Team.ChaosInsurgency : Team.ClassD;
        }

        private static Team? GetOpposingTeam(Team team)
        {
            switch (team)
            {
                case Team.ChaosInsurgency:
                case Team.ClassD:
                    return UnityEngine.Random.Range(0, 2) == 0 ? Team.FoundationForces : Team.Scientists;

                case Team.FoundationForces:
                case Team.Scientists:
                    return UnityEngine.Random.Range(0, 2) == 0 ? Team.ChaosInsurgency : Team.ClassD;

                default:
                    return null;
            }
        }
        
        private static RoleTypeId GetRandomRoleForTeam(Team? team, List<RoleTypeId> chaosRoles, List<RoleTypeId> foundationRoles, List<RoleTypeId> classDRoles, List<RoleTypeId> scientistRoles)
        {
            switch (team)
            {
                case Team.ChaosInsurgency:
                    return chaosRoles[UnityEngine.Random.Range(0, chaosRoles.Count)];

                case Team.FoundationForces:
                    return foundationRoles[UnityEngine.Random.Range(0, foundationRoles.Count)];

                case Team.ClassD:
                    return classDRoles[UnityEngine.Random.Range(0, classDRoles.Count)];

                case Team.Scientists:
                    return scientistRoles[UnityEngine.Random.Range(0, scientistRoles.Count)];

                default:
                    return RoleTypeId.None; // Return a default value for invalid teams
            }
        }
}