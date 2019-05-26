using FortniteReplayReader.Core.Models;
using FortniteReplayReader.Observerable.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FortniteReplayReader
{
    public abstract class ObservableFortniteBinaryReader<T> : FortniteBinaryReader, IFortniteObservable<T>
    {
        private IList<IFortniteObserver<T>> _observers;

        public ObservableFortniteBinaryReader(Stream input, bool autoLoad = true) : base(input)
        {
            Init(autoLoad);
        }

        public ObservableFortniteBinaryReader(Stream input, Replay replay, bool autoLoad = true) : base(input,  replay)
        {
            Init(autoLoad);
        }

        public ObservableFortniteBinaryReader(Stream input, int offset, bool autoLoad = true) : base(input, offset)
        {
            Init(autoLoad);
        }

        public ObservableFortniteBinaryReader(Stream input, int offset, Replay replay, bool autoLoad = true) : base(input, offset, replay)
        {
            Init(autoLoad);
        }

        private void Init(bool autoLoad)
        {
            _observers = new List<IFortniteObserver<T>>();

            //load all observers so we can register them
            if (autoLoad)
            {
                var binDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var loadedAssemblies = Directory.GetFiles(binDirectory, "FortniteReplayObserver.*.dll");
                foreach (var a in loadedAssemblies)
                {
                    Assembly.LoadFile(a);
                }

                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName.StartsWith("FortniteReplayObserver."))
                    .SelectMany(s => s.GetTypes())
                    .Where(type => typeof(IFortniteObserver<T>).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface).Distinct();

                foreach (var type in types)
                {
                    var instance = Activator.CreateInstance(type) as FortniteObserver<T>;
                    instance.Subscribe(this);
                }
            }
        }

        public override Replay ReadFile()
        {
            if (BaseStream.Position == 0)
            {
                OnStart();
                this.ParseMetadata();
            }

            this.ParseChunks();
            OnCompleted();
            return this.Replay;
        }


        public IDisposable Subscribe(IFortniteObserver<T> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubsriber<T>(_observers, observer);
        }

        internal void Notify(T value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        public void OnStart()
        {
            foreach (var observer in _observers)
            {
                observer.OnStart();
            }
        }

        public void OnCompleted()
        {
            foreach (var observer in _observers.ToArray())
            {
                observer.OnCompleted();
            }
            _observers.Clear();
        }

        private class Unsubsriber<V> : IDisposable
        {
            private IList<IFortniteObserver<V>> _observers;
            private readonly IFortniteObserver<V> _observer;

            public Unsubsriber(IList<IFortniteObserver<V>> observers, IFortniteObserver<V> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}
