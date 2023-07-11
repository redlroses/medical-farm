using System;
using Logic;
using Logic.Interactions;
using Logic.Player;
using Logic.Translators;
using Tools.Extension;
using UnityEngine;

namespace Tutorial
{
    [RequireComponent(typeof(RunTranslator))]
    public class TutorialArrow : MonoBehaviour, ITutorialTrigger
    {
        [SerializeField] private RunTranslator _translator;
        [SerializeField] private TranslatableAgent _translatableAgent;
        [SerializeField] private PlayerInteraction _playerInteraction;

        public event Action Triggered = () => { };

        private void Start()
        {
            _translator.AddTranslatable(_translatableAgent.MainTranslatable);
            _playerInteraction.Interacted += OnEnter;
        }

        private void OnDestroy() =>
            _playerInteraction.Interacted -= OnEnter;

        private void OnEnter(Hero _) =>
            Triggered.Invoke();

        public void Move(Vector3 to)
        {
            gameObject.SetActive(true);
            transform.position = to.ChangeY(transform.position.y);
        }

        public void Hide() =>
            gameObject.SetActive(false);
    }
}