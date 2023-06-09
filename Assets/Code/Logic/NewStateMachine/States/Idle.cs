﻿using Logic.AnimalsBehaviour;
using StateMachineBase;

namespace Logic.NewStateMachine.States
{
    public class Idle : State
    {
        public Idle(AnimalAnimator animator) : base(animator) { }
        protected override void OnEnter() => Animator.SetIdle();
    }
}