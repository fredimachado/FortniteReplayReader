using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Core.Models.Events;
using System.IO;

namespace FortniteReplayReader.Observerable
{
    public class ElimObservableFortniteBinaryReader : ObservableFortniteBinaryReader<PlayerElimination>
    {
        public ElimObservableFortniteBinaryReader(Stream input, bool autoLoad = true) : base(input, autoLoad)
        {
        }

        public ElimObservableFortniteBinaryReader(Stream input, Replay replay, bool autoLoad = true) : base(input, replay, autoLoad)
        {
        }

        public ElimObservableFortniteBinaryReader(Stream input, int offset, bool autoLoad = true) : base(input, offset, autoLoad)
        {
        }

        public ElimObservableFortniteBinaryReader(Stream input, int offset, Replay replay, bool autoLoad = true) : base(input, offset, replay, autoLoad)
        {
        }

        public override PlayerElimination ParseElimination(EventMetadata eventMetadata)
        {
            var elim = base.ParseElimination(eventMetadata);
            base.Notify(elim);
            return elim;
        }
    }
}
