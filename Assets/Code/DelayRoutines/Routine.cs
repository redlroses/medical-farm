﻿using System;

namespace DelayRoutines
{
    public abstract class Routine : IRoutine
    {
        private Action _executedCallback;

        public bool IsActive { get; protected set; }

        protected Routine()
        {
            _executedCallback = () => { };
        }

        public void Play()
        {
            IsActive = true;
            OnPlay();
        }

        public void Stop()
        {
            if (this is ConstTimeAwaiter awaiter)
            {
                awaiter.Pause();
                return;
            }

            IsActive = false;
        }

        protected virtual void OnPlay() { }

        public void AddNext(IRoutine routine) =>
            _executedCallback = routine.Play;

        protected void Next()
        {
            IsActive = false;
            _executedCallback.Invoke();
        }
    }
}