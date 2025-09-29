using LabApi.Events.CustomHandlers;
using SCP_Breach.Config;

namespace SCP_Breach.Core;

public abstract class ConditionalEventHandler<T> : CustomEventsHandler
{
    public abstract bool IsEnabled(T section);
    public BreachConfig GetConfig() => SCPBreachCore.Instance.BreachConfig;
}