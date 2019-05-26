using System;

namespace FortniteReplayReader.Observerable.Contracts
{
    public interface IFortniteObserver<T> : IObserver<T>
    {
        //
        // Summary:
        //     Notifies the observer that the provider has started sending push-based notifications.
        void OnStart();
    }
}
