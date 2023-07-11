using Logic.Interactions;
using Logic.Player;
using NaughtyAttributes;
using NTC.Global.Cache;
using UnityEngine;

namespace Logic
{
    public class InteractionSine : MonoCache
    {
        [SerializeField] private PlayerInteraction _playerInteraction;
        [SerializeField] private Transform _sine;

        [SerializeField] private float _defaultSize;
        [SerializeField] private float _maxSize;
        [SerializeField] private float _decreaseTime = 0.25f;

        private float _deltaSize = 1;
        private float _velocity;
        private float _targetSize;
        private float _smoothTime;
        
        private void Awake()
        {
            _playerInteraction.Entered += OnEnter;
            _playerInteraction.Canceled += OnCancel;
        }

        private void OnDestroy()
        {
            _playerInteraction.Entered -= OnEnter;
            _playerInteraction.Canceled -= OnCancel;
        }
        
        private void OnCancel()
        {
            Cancel();
        }

        private void OnEnter(Hero hero)
        {
            BeginIncrease();
        }

        protected override void Run()
        {
            if (Mathf.Approximately(_deltaSize, _targetSize))
            {
                enabled = false;
            }

            _deltaSize = Mathf.SmoothDamp(_deltaSize, _targetSize, ref _velocity, _smoothTime);
            _sine.localScale = Vector3.one * _deltaSize;
        }

        [Button("Invrease")]
        private void BeginIncrease()
        {
            _targetSize = _maxSize;
            _smoothTime = _playerInteraction.InteractionDelay;
            enabled = true;
        }

        [Button("Cancel")]
        private void Cancel()
        {
            _targetSize = _defaultSize;
            _smoothTime = _decreaseTime;
            enabled = true;
        }
    }
}
    