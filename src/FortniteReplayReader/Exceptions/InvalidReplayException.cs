using System;

namespace FortniteReplayReader.Exceptions
{
    public class InvalidReplayException : Exception
    {
        public InvalidReplayException() : base() { }
        public InvalidReplayException(string msg) : base(msg) { }
    }
}
