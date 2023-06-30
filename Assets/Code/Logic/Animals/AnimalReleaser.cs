using Logic.Interactions;
using Logic.Spawners;
using Logic.Translators;
using Services;
using Services.Animals;
using Ui.Services;
using UnityEngine;

namespace Logic.Animals
{
    [RequireComponent(typeof(RunTranslator))]
    public class AnimalReleaser : MonoBehaviour
    {
        //TODO: В дальнейшем за каждое животное разное количество денег
        [SerializeField] private int _coinsToSpawn;
        [SerializeField] private Delay _delay;

        private CollectibleCoinSpawner _spawner;
        private IAnimalsService _animalService;
        private IWindowService _windowService;

        private void Awake()
        {
            _spawner = GetComponent<CollectibleCoinSpawner>();
            _animalService = AllServices.Container.Single<IAnimalsService>();
            _windowService = AllServices.Container.Single<IWindowService>();
            _animalService.Released += OnReleased;
            _delay.Complete += OnCompleteDelay;
        }

        private void OnCompleteDelay(GameObject _)
        {
            _windowService.Open(WindowId.AnimalRelease);
        }

        private void OnDestroy()
        {
            _animalService.Released -= OnReleased;
            _delay.Complete -= OnCompleteDelay;
        }

        private void OnReleased(AnimalType type)
        {
            Debug.Log("Spawn coins");
            _spawner.Spawn(_coinsToSpawn);
        }
    }
}
