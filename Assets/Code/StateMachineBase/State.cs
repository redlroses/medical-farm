using System.Collections.Generic;
using Logic.AnimalsBehaviour;

namespace StateMachineBase
{
    public abstract class State
    {
        private readonly List<Transition> _transitions = new List<Transition>();

        protected readonly AnimalAnimator Animator;

        protected State(AnimalAnimator animator) =>
            Animator = animator;

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate() { }

        public void AddTransition(Transition transition) =>
            _transitions.Add(transition);

        public void Enter()
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                _transitions[i].Enter();
            }

            OnEnter();
        }

        public void Exit()
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                _transitions[i].Exit();
            }

            OnExit();
        }

        public void Update()
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                _transitions[i].Update();
            }

            OnUpdate();
        }
    }
}