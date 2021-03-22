
using System;

namespace NUnit.Behaviours
{
    internal class InitializationLock
    {
        private bool isInitialized = false;
        private object? lastSeenTarget;
        private readonly object locker = new();

        public void Run(object? target, Action action)
        {
            UnlockIfNewTarget(target);

            if (isInitialized) return;

            lock (locker)
            {
                if (isInitialized) return;

                action();

                isInitialized = true;
            }
        }

        private void UnlockIfNewTarget(object? target)
        {
            if (lastSeenTarget == null || !ReferenceEquals(target, lastSeenTarget))
                lock (locker)
                    if (lastSeenTarget == null || !ReferenceEquals(target, lastSeenTarget))
                    {
                        Unlock();
                        lastSeenTarget = target;
                    }
        }

        public void Unlock()
        {
            isInitialized = false;
        }
    }
}
