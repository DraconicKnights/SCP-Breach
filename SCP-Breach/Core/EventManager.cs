using System.Reflection;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using SCP_Breach.Attributes;
using SCP_Breach.Config;

namespace SCP_Breach.Core;

public class EventManager
{
    private Dictionary<string, CustomEventsHandler> _eventsHandlers;
    private readonly BreachConfig _config;

    public EventManager(BreachConfig config)
    {
        _config = config;
        _eventsHandlers = new Dictionary<string, CustomEventsHandler>();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        var eventRegisterClasses = assembly.GetTypes().Select(type => new
            {
                Type = type,
                Attribute = type.GetCustomAttribute<EventAttribute>()
            })
            .Where(x => x.Attribute != null)
            .OrderBy(x => x.Attribute!.Priority)
            .ToList();
        
        Logger.Info($"Found {eventRegisterClasses.Count} event handlers");

        foreach (var cand in eventRegisterClasses)
        {
            var handlerType = cand.Type;
            var attribute = cand.Attribute!;

            var baseType = handlerType.BaseType;
            if (baseType?.IsGenericType == true && baseType.GetGenericTypeDefinition() == typeof(ConditionalEventHandler<>))
            {
                var sectionType = baseType.GetGenericArguments()[0];
                var prop = typeof(BreachConfig)
                    .GetProperties()
                    .FirstOrDefault(p => p.PropertyType == sectionType);
                if (prop == null)
                {
                    Logger.Warn($"No config section of type {sectionType.Name} found; skipping {attribute.Name}");
                    continue;
                }

                var sectionValue = prop.GetValue(_config);
                var instance = (CustomEventsHandler)Activator.CreateInstance(handlerType)!;
                var enabled = (bool)handlerType
                    .GetMethod(nameof(ConditionalEventHandler<object>.IsEnabled))!
                    .Invoke(instance, new[] { sectionValue! })!;

                if (!enabled)
                {
                    Logger.Info($"Skipping {attribute.Name} because {prop.Name} flag is false");
                    continue;
                }

                CustomHandlersManager.RegisterEventsHandler(instance);
                _eventsHandlers[attribute.Name] = instance;
            }
            else
            {
                var instance = (CustomEventsHandler)Activator.CreateInstance(handlerType)!;
                CustomHandlersManager.RegisterEventsHandler(instance);
                _eventsHandlers[attribute.Name] = instance;
            }
            Logger.Info($"Registered event handler: {attribute.Name}");
        }

    }

    public void UnregisterEvents()
    {
        foreach (var eventsHandler in _eventsHandlers)
        {
            Logger.Info($"Unregistering event handler: {eventsHandler.Key}");
            _eventsHandlers.Remove(eventsHandler.Key);
            CustomHandlersManager.UnregisterEventsHandler(eventsHandler.Value);
            Logger.Info($"Unregistered event handler: {eventsHandler.Key}");
        }
        _eventsHandlers.Clear();
    }
}