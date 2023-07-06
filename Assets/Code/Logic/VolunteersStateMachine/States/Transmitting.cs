using Logic.AnimatorStateMachine;
using StateMachineBase.States;
using Unity.VisualScripting;

namespace Logic.VolunteersStateMachine.States
{
    public class Transmitting : Idle
    {
        private Volunteer.Volunteer volunteer;

        public Transmitting(IPrimeAnimator animator, Volunteer.Volunteer volunteer) : base(animator)
        {
            this.volunteer = volunteer;
        }
        
    }
}
