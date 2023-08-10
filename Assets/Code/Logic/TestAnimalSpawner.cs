using Data.ItemsData;
using Infrastructure.Factory;
using Logic.Animals;
using Logic.Animals.AnimalsBehaviour;
using Logic.Medical;
using NaughtyAttributes;
using Services;
using Services.AnimalHouses;
using UnityEngine;

namespace Logic
{
    public class TestAnimalSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private AnimalItemStaticData _animalData;

        private IGameFactory _gameFactory;
        private IAnimalHouseService _houseService;

        private void Awake()
        {
            _gameFactory = AllServices.Container.Single<IGameFactory>();
            _houseService = AllServices.Container.Single<IAnimalHouseService>();
        }

        [Button]
        private void Spawn()
        {
            var animal = _gameFactory.CreateAnimal(new AnimalItemData(_animalData, MedicalToolId.Syringe), _spawnPoint.position,
                _spawnPoint.rotation).GetComponent<Animal>();
            _houseService.TakeQueueToHouse(new QueueToHouse(animal.AnimalId, () => animal));
        }
    }
}