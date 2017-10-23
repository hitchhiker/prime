using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core
{
    internal abstract class RenewingSubscriberList<TMessage, TContainer> : SubscriberList<TMessage,TContainer> where TMessage : RenewableSubscribeMessageBase
    {
        private readonly Timer _timer = new Timer();

        protected RenewingSubscriberList()
        {
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
            new Task(delegate ()
            {
                UnsubscribeExpired();
                _timer.Start();
            }).Start();
        }

        private void UnsubscribeExpired()
        {
            lock (Lock)
            {
                if (Subscribers.Count == 0)
                    return;

                var expiredEntries = Subscribers.Where(x => !x.Value.IsFresh()).ToList();

                foreach (var entry in expiredEntries)
                    UnsubscribeAll(entry.Key);
            }
        }

        public override void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
            base.Dispose();
        }
    }
}