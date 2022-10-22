using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GeoMuzeum.View.ViewServices
{
    public class ViewMessenger
    {
        private static readonly object CreationLock = new object();
        private static readonly ConcurrentDictionary<MessengerKey, object> Dictionary = new ConcurrentDictionary<MessengerKey, object>();

        private static ViewMessenger _instance;

        public static ViewMessenger Default
        {
            get
            {
                if (_instance == null)
                {
                    lock (CreationLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ViewMessenger();
                        }
                    }
                }

                return _instance;
            }
        }

        private ViewMessenger()
        {

        }

        public void Register<T>(object recipient, Action<T> action)
        {
            Register<T>(recipient, action, null);
        }

        public void Register<T>(object recipient, Action<T> action, object context)
        {
            var key = new MessengerKey(recipient, context);
            Dictionary.TryAdd(key, action);
        }

        public void Unregister(object recipient)
        {
            Unregister(recipient, null);
        }

        public void Unregister(object recipient, object context)
        {
            object action;
            var key = new MessengerKey(recipient, context);
            Dictionary.TryRemove(key, out action);
        }

        public void Send<T>(T message)
        {
            Send(message, null);
        }

        public void Send<T>(T message, object context)
        {
            IEnumerable<KeyValuePair<MessengerKey, object>> result;

            if (context == null)
            {
                result = from r in Dictionary where r.Key.Context == null select r;
            }
            else
            {
                result = from r in Dictionary where r.Key.Context != null && r.Key.Context.Equals(context) select r;
            }

            foreach (var action in result.Select(x => x.Value).OfType<Action<T>>())
            {
                action(message);
            }
        }

        protected class MessengerKey
        {
            public object Recipient { get; private set; }
            public object Context { get; private set; }

            public MessengerKey(object recipient, object context)
            {
                Recipient = recipient;
                Context = context;
            }

            protected bool Equals(MessengerKey other)
            {
                return Equals(Recipient, other.Recipient) && Equals(Context, other.Context);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;

                return Equals((MessengerKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Recipient != null ? Recipient.GetHashCode() : 0) * 397) ^ (Context != null ? Context.GetHashCode() : 0);
                }
            }
        }
    }
}
