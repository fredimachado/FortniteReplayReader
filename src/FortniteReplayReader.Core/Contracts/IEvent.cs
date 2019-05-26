using FortniteReplayReader.Core.Models.Events;

namespace FortniteReplayReader.Core.Contracts
{
    public interface IEvent
    {
        EventMetadata EventMetadata { get; set; }
    }
}
