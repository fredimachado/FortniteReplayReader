using System.Linq;

namespace FortniteReplayReader.Core.Models
{
    public class Header
    {
        public uint HeaderVersion { get; set; }
        public uint ServerSideVersion { get; set; }
        public uint Season { get; set; }
        public string Guid { get; set; } = "";
        public uint ReplayVersion { get; set; }
        public uint FortniteVersion { get; set; }
        public string Release { get; set; }
        public string Map { get; set; } = "";
        public string SubGame { get; set; } = "";

        private int? _releaseNumber { get; set; }
        public int? ReleaseNumber
        {
            get
            {
                if (_releaseNumber == null)
                {
                    if (string.IsNullOrWhiteSpace(Release))
                    {
                        _releaseNumber = 0;
                    }
                    else
                    {
                        var result = new string(Release.ToCharArray().Where(c => char.IsDigit(c)).ToArray());
                        _releaseNumber = string.IsNullOrWhiteSpace(result) ? 0 : int.Parse(result);
                    }
                }

                return _releaseNumber;
            }
        }


        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
        public uint Unknown4 { get; set; }

    }
}
