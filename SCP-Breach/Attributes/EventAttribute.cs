namespace SCP_Breach.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EventAttribute : Attribute
{
    public string Name { get; }
    public int Priority { get; }
    
    public EventAttribute(string name, int priority = 0)
    {
        Name = name;
        Priority = priority;
    }
}