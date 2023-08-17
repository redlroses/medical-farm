using System;
using AYellowpaper;
using DelayRoutines;
using Logic.Interactions.Validators;
using Logic.Player;
using NaughtyAttributes;
using Observer;
using UnityEngine;

namespace Logic.Interactions
{
    [RequireComponent(typeof(TimerOperator))]
    public class InteractionZone<T> : ObserverTargetExit<T, TriggerObserverExit>, IInteractionZone where T : Human
    {
        [SerializeField] private float _interactionDelay;
        [SerializeField] private bool _isValidate;

        [SerializeField] [ShowIf("_isValidate")]
        private InterfaceReference<IInteractionValidator, MonoBehaviour>[] _interactionValidators;

        private bool _isLock;
        private RoutineSequence _waitToUnlock;

#if UNITY_EDITOR
        private float _prevDelayValue;
#endif

        [SerializeField] private TimerOperator _timerOperator;

        private T _cashed;

        public event Action<T> Interacted = c => { };
        public event Action Entered = () => { };
        public event Action Canceled = () => { };
        public event Action Rejected = () => { };

        public float InteractionDelay => _interactionDelay;

        protected override void OnAwake()
        {
            _timerOperator ??= GetComponent<TimerOperator>();
            _timerOperator.SetUp(_interactionDelay, OnDelayPassed);
        }

        private void OnDestroy() =>
            _waitToUnlock?.Kill();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Mathf.Approximately(_interactionDelay, _prevDelayValue))
                return;

            OnAwake();
        }
#endif

        protected override void OnDisabled()
        {
            base.OnDisabled();
            _isLock = false;
            _timerOperator.Pause();
        }

        protected override void OnTargetEntered(T hero)
        {
            if (_isLock)
            {
                _waitToUnlock = new RoutineSequence();
                _waitToUnlock
                    .WaitUntil(() => _isLock == false)
                    .Then(() => OnTargetEntered(hero))
                    .SetAutoKill(true)
                    .Play();
                return;
            }

            if (_isValidate)
                Validate(hero);
            else
                InvokeEntered(hero);
        }

        private void Validate(T hero)
        {
            bool isAllValid = true;

            for (var index = 0; index < _interactionValidators.Length; index++)
                isAllValid &= _interactionValidators[index].Value.IsValid(hero.Inventory);

            if (isAllValid)
                InvokeEntered(hero);
            else
                Rejected.Invoke();
        }

        protected override void OnTargetExited(T _)
        {
            _isLock = false;
            _timerOperator.Pause();
            Canceled.Invoke();
        }

        private void OnDelayPassed()
        {
            Interacted.Invoke(_cashed);
        }

        private void InvokeEntered(T hero)
        {
            _isLock = true;
            _cashed = hero;
            _timerOperator.Restart();
            Entered.Invoke();
        }
    }
}