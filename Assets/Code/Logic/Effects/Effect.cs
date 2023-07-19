using System;
using Services.Pools;
using UnityEngine;

namespace Logic.Effects
{
    public class Effect : MonoBehaviour, ISelfPoolable
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public GameObject GameObject => gameObject;
        public event Action<ISelfPoolable> Disabled = i => { };

        public void Play(Location at)
        {
            transform.SetPositionAndRotation(at.Position, at.Rotation);
            _particleSystem.Play();
        }

        private void OnDisable() =>
            Disabled.Invoke(this);
    }
}