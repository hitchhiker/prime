using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class RenewingSubscriptionsListener<T> : IDisposable where T : RenewableSubscribeMessageBase
    {
        private class SubscriptionEntry
        {
            public readonly T Message;
            public DateTime UtcLastUpdated { get; private set; }

            public SubscriptionEntry(T message)
            {
                UtcLastUpdated = DateTime.UtcNow;
                Message = message;
            }

            public void Refresh()
            {
                UtcLastUpdated = DateTime.UtcNow;
            }
        }

        private TimeSpan _expirationSpan = TimeSpan.MinValue;
        private readonly Action<SubscriptionState, ObjectId, T> _stateChanged;
        protected readonly IMessenger Messenger = DefaultMessenger.I.Default;
        private readonly Dictionary<ObjectId, SubscriptionEntry> _subs = new Dictionary<ObjectId, SubscriptionEntry>();
        protected int TimerInterval { get; set; }
        public bool IsStarted { get; protected set; }
        protected readonly object RegistrationLock = new object();
        protected readonly object StateLock = new object();
        private readonly Timer _timer = new Timer();

        public RenewingSubscriptionsListener(Action<SubscriptionState, ObjectId, T> stateChanged)
        {
            _stateChanged = stateChanged;
            Messenger.RegisterAsync<T>(this, IncomingSubscription);
            _timer = new Timer
            {
                Interval = 500,
                AutoReset = false
            };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            new Task(delegate()
            {
                TrimSubscriptionState();
                _timer.Start();
            }).Start();
        }

        private void IncomingSubscription(T m)
        {
            if (_expirationSpan == TimeSpan.MinValue && m != null) //bit of a hack
                _expirationSpan = m.ExpirationSpan;

            lock (RegistrationLock)
            {
                if (m.Unsubscribe)
                    IncomingUnsubscribe(m);
                else
                    IncomingSubscribe(m);
                
                SendStateChange(m.SubscriberId, m);
            }
        }

        private void IncomingSubscribe(T m)
        {
            if (_subs.ContainsKey(m.SubscriberId))
                _subs[m.SubscriberId].Refresh();
            else
                _subs.Add(m.SubscriberId, new SubscriptionEntry(m));
        }

        private void IncomingUnsubscribe(T m)
        {
            if (_subs.ContainsKey(m.SubscriberId))
                _subs.Remove(m.SubscriberId);
        }

        private void TrimSubscriptionState()
        {
            lock (RegistrationLock)
            {
                if (_subs.Count == 0 || _expirationSpan == TimeSpan.MinValue)
                    return;

                var expired = _subs.Where(kv=> kv.Value.UtcLastUpdated.IsBeforeTheLast(_expirationSpan)).ToList();

                _subs.RemoveAll(x => expired.Any(e=> e.Key == x.Key));

                foreach (var k in expired)
                    SendStateChange(k.Key, k.Value.Message);
            }
        }

        private void SendStateChange(ObjectId subscriberId, T message)
        {
            if (message == null)
                return;

            var state = _subs.ContainsKey(subscriberId) ? SubscriptionState.Subscribed : SubscriptionState.Unsubscribed;
            new Task(() =>
            {
                _stateChanged.Invoke(state, subscriberId, message);
            }).Start();
        }

        public void Dispose()
        {
            Messenger.Unregister(this);
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}