using System;

namespace FortniteReplayReader.Observerable.Contracts
{
    public interface IFortniteObservable<T>
    {
        //
        // Summary:
        //     Notifies the provider that an observer is to receive notifications.
        //
        // Parameters:
        //   observer:
        //     The object that is to receive notifications.
        //
        // Returns:
        //     A reference to an interface that allows observers to stop receiving notifications
        //     before the provider has finished sending them.
        IDisposable Subscribe(IFortniteObserver<T> observer);
    }
}
