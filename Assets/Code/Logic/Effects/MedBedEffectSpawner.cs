using Services;
using Services.Effects;
using UnityEngine;

namespace Logic.Effects
{
    public class EffectSpawner : MonoBehaviour
    {
        [SerializeField] private IEffectTrigger _effectTrigger;
        [SerializeField] private EffectId _healedEffect;

        private IEffectService _effectService;
        
        private void Awake()
        {
            _effectService = AllServices.Container.Single<IEffectService>();
        }

        private void OnEnable()
        {
            _effectTrigger.EffectTriggered += OnPlayEffect;
        }

        private void OnPlayEffect()
        {
            Transform self = transform;
            _effectService.SpawnEffect(_healedEffect, new Location(self.position, self.rotation));
        }
    }
}