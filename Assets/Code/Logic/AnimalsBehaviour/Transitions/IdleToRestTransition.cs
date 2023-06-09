using Logic.AnimalsBehaviour.States;
using MonoStateMachine;
using UnityEngine;

namespace Logic.AnimalsBehaviour.Transitions
{
    public class IdleToRestTransition : Transition
    {
        [Header("Component References")]
        [SerializeField] private ProgressBarIndicator _peppiness;
        [SerializeField] private Transform _restPlace;
        
        [Header("Rate Settings")] [Space]
        [SerializeField] private float _restPlaceOffset = 0.5f;

        // protected override void OnEnabled()
        // {
        //     _peppiness.ProgressBar.Empty += MoveToRest;
        // }
        //
        // protected override void OnDisabled()
        // {
        //     _peppiness.ProgressBar.Empty -= MoveToRest;
        // }

        protected override void Run()
        {
            if (_peppiness.ProgressBar.IsEmpty)
            {
                MoveToRest();
                Disable();
            }
        }

        private void MoveToRest()
        {
            if (IsInRestPlace())
            {
                StateMachine.Enter<AnimalRestState>();
                
                return;
            }
            
            StateMachine.Enter<AnimalMoveState, Vector3>(_restPlace.position);
        }
        
        private bool IsInRestPlace() =>
            Vector3.Distance(transform.position, _restPlace.position) <= _restPlaceOffset;
    }
}