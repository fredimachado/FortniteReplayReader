using System;

namespace FortniteReplayReader.Core.Exceptions
{
    public class UnknownEventException : Exception
    {
        public UnknownEventException() : base() { }
        public UnknownEventException(string msg) : base(msg) { }
    }
}
