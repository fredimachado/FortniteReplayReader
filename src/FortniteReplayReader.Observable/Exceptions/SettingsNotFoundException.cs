using System;

namespace FortniteReplayReader.Observerable.Exceptions
{
    public class SettingsNotFoundException : Exception
    {
        public SettingsNotFoundException() : base() { }
        public SettingsNotFoundException(string msg) : base(msg) { }
    }
}
