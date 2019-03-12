using System;

namespace FortniteReplayReader.Core.Exceptions
{
    public class PlayerEliminationException : Exception
    {
        public PlayerEliminationException() : base() { }
        public PlayerEliminationException(string msg) : base(msg) { }
    }
}
