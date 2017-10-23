using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public abstract class CoordinatorBase
    {
        protected readonly IMessenger _messenger = DefaultMessenger.I.Default;

        protected int TimerInterval { get; set; }
        public int SubscribersCount { get; protected set; } = 0;
        public bool IsStarted { get; protected set; }
        protected readonly object RegistrationLock = new object();
        protected readonly object StateLock = new object();

        public virtual void Register<T>(object subscriber, Action<T> action)
        {
            lock (RegistrationLock)
            {
                SubscribersCount++;
                _messenger.Register(subscriber, action);
                new Task(RegistrationChanged).Start();
            }
        }

        public virtual void Unregister<T>(object subscriber, Action<T> action)
        {
            lock (RegistrationLock)
            {
                SubscribersCount--;
                _messenger.Unregister(subscriber, action);
                new Task(RegistrationChanged).Start();
            }
        }

        protected virtual void Start(UserContext context)
        {
            lock (StateLock)
            {
                if (TimerInterval == 0 || IsStarted)
                    return;

                OnStart(context);

                IsStarted = true;
            }
        }

        protected virtual void Stop()
        {
            lock (StateLock)
            {
                if (!IsStarted)
                    return;

                OnStop();

                IsStarted = false;
            }
        }

        protected abstract void OnStart(UserContext context);
        protected abstract void OnStop();

        protected virtual void Restart(UserContext context)
        {
            lock (StateLock)
            {
                Stop();
                Start(context);
            }
        }

        protected virtual void RegistrationChanged()
        {
            lock (RegistrationLock)
            {
                if (SubscribersCount < 1)
                {
                    SubscribersCount = 0;
                    OnSubscribersChanged();
                }
                else
                    OnSubscribersChanged();
            }
        }

        protected abstract void OnSubscribersChanged();
    }
}