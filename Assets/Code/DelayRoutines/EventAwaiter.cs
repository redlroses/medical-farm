using System;
using NTC.Global.Cache;

namespace DelayRoutines
{
    // Unfinished, better not to use it
    public class EventAwaiter : Awaiter
    {
        private Action _callback;

        public EventAwaiter(Action callback, GlobalUpdate globalUpdate) : base(globalUpdate)
        {
            _callback = callback;
            Deactivate();
            _callback += Next;
        }

        public override void OnRun() { }
    }
}