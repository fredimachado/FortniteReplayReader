using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Observerable.Contracts;
using Newtonsoft.Json;
using System;

namespace FortniteReplayObservers.File
{
    public class BaseFileObserver<T> : FortniteObserver<T>, IFortniteObserver<T>
    {
        private IDisposable unsubscriber;
        protected FileSettings Settings;

        public BaseFileObserver()
        {
            Settings = ReadSettingsFile<FileSettings>();
        }

        protected virtual string CreateMessagePayload(T e)
        {
            return JsonConvert.SerializeObject(e);
        }

        public void OnCompleted()
        {
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            Unsubscribe();
        }

        public void OnNext(T value)
        {
            System.IO.File.AppendAllText(Settings.Path, CreateMessagePayload(value) + "\n");
        }

        public override void Subscribe(IFortniteObservable<T> provider)
        {
            if (provider != null)
            {
                unsubscriber = provider.Subscribe(this);
            }
        }

        public override void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        public void OnStart()
        {
            System.IO.File.AppendAllText(Settings.Path, "started \n");
        }
    }

    public class FileSettings
    {
        public string UserName { get; set; }
        public string Path { get; set; }
    }
}