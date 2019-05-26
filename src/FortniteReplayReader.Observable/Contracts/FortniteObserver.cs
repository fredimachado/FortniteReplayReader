using FortniteReplayReader.Observerable.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace FortniteReplayReader.Observerable.Contracts
{
    public abstract class FortniteObserver<T>
    {
        public abstract void Subscribe(IFortniteObservable<T> provider);

        public abstract void Unsubscribe();

        protected TSettings ReadSettingsFile<TSettings>()
        {
            var path = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, $"{this.GetType().Name}.json" });

            if (File.Exists(path))
            {
                try
                {
                    return JsonConvert.DeserializeObject<TSettings>(File.ReadAllText(path));
                }
                catch (Exception)
                {
                    throw new InvalidSettingsException();
                }
            }

            throw new SettingsNotFoundException();
        }
    }
}
