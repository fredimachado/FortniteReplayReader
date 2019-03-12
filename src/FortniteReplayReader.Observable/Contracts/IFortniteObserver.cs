using System;
using System.Collections.Generic;
using System.Text;

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
