using DelayRoutines;
using UnityEngine;

namespace Logic.Effects
{
    public class OnRenderEffectShower : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Renderer _spriteRenderer;

        [SerializeField] private Vector2 _randomPreShowDelay;
        [SerializeField] private float _afterShowDelay;

        private RoutineSequence _routine;

        private void Awake()
        {
            _routine = new RoutineSequence(RoutineUpdateMod.FixedRun)
                .WaitUntil(() => _spriteRenderer.isVisible)
                .WaitForRandomSeconds(_randomPreShowDelay)
                .Then(_particleSystem.Play)
                .WaitForSeconds(_afterShowDelay)
                .LoopWhile(() => enabled);
        }

        private void Start() =>
            _routine.Play();

        private void OnDestroy()
        {
            Debug.Log("Happy effect kill");
            _routine.Kill();
        }
    }
}