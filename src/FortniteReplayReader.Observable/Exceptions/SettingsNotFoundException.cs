using System;

namespace FortniteReplayReader.Core.Exceptions
{
    public class SettingsNotFoundException : Exception
    {
        public SettingsNotFoundException() : base() { }
        public SettingsNotFoundException(string msg) : base(msg) { }
    }
}
