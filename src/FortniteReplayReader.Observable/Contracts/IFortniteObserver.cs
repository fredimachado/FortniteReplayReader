using System;

namespace FortniteReplayReader.Observerable.Contracts
{
    public interface IFortniteObserver<T> : IObserver<T>
    {
        /// <summary>
        /// Notifies the observer that the provider has started sending push-based notifications.
        /// </summary>
        void OnStart();
    }
}
