using FortniteReplayReader.Core.Models;
using System.IO;

namespace FortniteReplayReader
{
    public class ElimObservableFortniteBinaryReader : ObservableFortniteBinaryReader<PlayerElimination>
    {
        public ElimObservableFortniteBinaryReader(Stream input, bool autoLoad = true) : base(input, autoLoad)
        {
        }

        public ElimObservableFortniteBinaryReader(Stream input, int offset, bool autoLoad = true) : base(input, offset, autoLoad)
        {
        }

        protected override PlayerElimination ParseElimination(uint time)
        {
            var elim = base.ParseElimination(time);
            base.Notify(elim);
            return elim;
        }
    }
}
