using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Observerable.Contracts;
using Newtonsoft.Json;
using System;

namespace FortniteReplayObservers.File
{
    public class BaseFileObserver<T> : FortniteObserver<T>, IFortniteObserver<T>
    {
        private IDisposable unsubscriber;
        private string path = "";

        public BaseFileObserver()
        {
            var settings = ReadSettingsFile<FileSettings>();
            path = settings.Path;
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
            System.IO.File.AppendAllText(path, CreateMessagePayload(value));
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
            System.IO.File.AppendAllText(path, "started");
        }
    }

    public class FileSettings
    {
        public string Path { get; set; }
    }
}