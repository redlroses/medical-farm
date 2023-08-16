using System;
using NTC.Global.Cache;
using NTC.Global.System;
using UnityEngine;

namespace Logic.Movement
{
    public class ItemMover : MonoCache, IItemMover
    {
        [SerializeField] [Min(.0f)] private float _speed = 5.0f;
        [SerializeField] [Min(.0f)] private float _errorOffset = 0.1f;
        [SerializeField] private Vector3 _rotationOffset;

        private Transform _target;
        private Transform _finalParent;
        private Quaternion _finalRotation;

        private Action _endMoveCallback;
        private Action _moving;

        private bool _isModifyRotation;

        public event Action Started = () => { };
        public event Action Ended = () => { };

        private void Awake()
        {
            _moving += Translate;
            enabled = false;
        }

        public void Move(Transform to, Action onMoveEnded, Transform finishParent = null, bool isModifyRotation = false)
        {
            _endMoveCallback = onMoveEnded;
            Move(to, finishParent, isModifyRotation);
        }

        public void Move(Transform to, Transform finishParent = null, bool isModifyRotation = false)
        {
            if (finishParent)
                _moving += Rotate;

            _isModifyRotation = isModifyRotation;
            _target = to;
            _finalParent = finishParent;
            transform.Unparent();
            enabled = true;
            Started.Invoke();
        }

        protected override void Run()
        {
            _moving.Invoke();

            if (IsFinished())
                FinishTranslation();
        }

        private void Rotate()
        {
            float distanceToTarget = GetDistanceToTarget();
            transform.rotation = Quaternion.Lerp(transform.rotation, GetFinalRotation(),
                Time.deltaTime * _speed / distanceToTarget);
        }

        private void Translate()
        {
            Vector3 translateDirection = (_target.position - transform.position).normalized;
            Vector3 deltaTranslation = translateDirection * _speed * Time.smoothDeltaTime;
            transform.Translate(deltaTranslation, Space.World);
        }

        private void FinishTranslation()
        {
            enabled = false;
            transform.SetParent(_finalParent, true);

            if (_finalParent)
            {
                _moving -= Rotate;
                transform.rotation = GetFinalRotation();
            }

            Ended.Invoke();
        }

        private bool IsFinished() =>
            GetDistanceToTarget() < _errorOffset;

        private Quaternion GetFinalRotation() =>
            _isModifyRotation
                ? _finalParent.rotation * Quaternion.Euler(_rotationOffset)
                : _finalParent.rotation;

        private float GetDistanceToTarget() =>
            Vector3.Distance(transform.position, _target.position);
    }
}