using System;

namespace FortniteReplayReader.Core.Exceptions
{
    public class InvalidReplayException : Exception
    {
        public InvalidReplayException() : base() { }
        public InvalidReplayException(string msg) : base(msg) { }
    }
}
