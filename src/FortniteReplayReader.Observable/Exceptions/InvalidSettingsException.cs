using System;

namespace FortniteReplayReader.Core.Exceptions
{
    public class InvalidSettingsException : Exception
    {
        public InvalidSettingsException() : base() { }
        public InvalidSettingsException(string msg) : base(msg) { }
    }
}
