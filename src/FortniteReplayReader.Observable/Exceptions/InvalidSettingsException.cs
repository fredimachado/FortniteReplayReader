using System;

namespace FortniteReplayReader.Observerable.Exceptions
{
    public class InvalidSettingsException : Exception
    {
        public InvalidSettingsException() : base() { }
        public InvalidSettingsException(string msg) : base(msg) { }
    }
}
