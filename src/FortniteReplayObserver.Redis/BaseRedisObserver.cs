using FortniteReplayReader.Core.Contracts;
using FortniteReplayReader.Observerable.Contracts;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace FortniteReplayObserver.Redis
{
    public class BaseRedisObserver<T> : FortniteObserver<T>, IFortniteObserver<T>
    {
        private IDisposable unsubsriber;
        private ConnectionMultiplexer Connection;
        private ISubscriber PubSub;
        private RedisSettings _settings;

        public BaseRedisObserver()
        {
            _settings = ReadSettingsFile<RedisSettings>();

            var options = ConfigurationOptions.Parse(_settings.ConnectionString);
            options.ClientName = $"Redis_{Guid.NewGuid()}";
            options.Password = _settings.Password;
            Connection = ConnectionMultiplexer.Connect(options);
            PubSub = Connection.GetSubscriber();
        }

        protected virtual string CreateMessagePayload(T e)
        {
            return JsonConvert.SerializeObject(e);
        }

        public void OnCompleted()
        {
            Unsubscribe();
        }

        public void OnError(Exception error)
        {
            Unsubscribe();
        }

        public void OnNext(T value)
        {
            var message = CreateMessagePayload(value);
            PubSub.Publish(_settings.Channel, message);
        }

        public void OnStart()
        {
            PubSub.Publish(_settings.Channel, "{ \"start\": 1 }");
        }

        public override void Subscribe(IFortniteObservable<T> provider)
        {
            if (provider != null)
            {
                unsubsriber = provider.Subscribe(this);
            }
        }

        public override void Unsubscribe()
        {
            PubSub.UnsubscribeAll();
            Connection.Dispose();
            unsubsriber.Dispose();
        }
    }

    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public string Password { get; set; }
        public string Channel { get; set; }
    }
}
