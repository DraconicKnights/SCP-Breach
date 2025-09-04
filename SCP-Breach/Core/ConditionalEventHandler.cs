using LabApi.Events.CustomHandlers;

namespace SCP_Breach.Core;

public abstract class ConditionalEventHandler<T> : CustomEventsHandler
{
    public abstract bool IsEnabled(T section);
}