using FortniteReplayReader.Core.Contracts;

namespace FortniteReplayReader.Core.Models.Events
{
    public abstract class BaseEvent : IEvent
    {
        public EventMetadata EventMetadata { get; set; }
    }
}