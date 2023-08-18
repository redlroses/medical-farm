using System;
using Logic.Animals;
using Logic.Animals.AnimalsBehaviour;
using Data.ItemsData;
using Infrastructure.Factory;
using Logic.Effects;
using Logic.Storages;
using Logic.Storages.Items;
using NTC.Global.Cache;
using Services;
using Services.AnimalHouses;
using Services.Effects;
using UnityEngine;

namespace Logic.Medical
{
    [RequireComponent(typeof(TimerOperator))]
    public class MedicalBed : MonoCache, IAddItem, IGetItemObserver
    {
        [SerializeField] private Transform _spawnPlace;
        [SerializeField] private TimerOperator _timerOperator;
        [SerializeField] [Range(0f, 5f)] private float _healingTime = 2.5f;

        private AnimalItemData _animalData;
        private MedicalToolItemData _medicalToolData;

        private IItem _animalItem;
        private IItem _medToolItem;

        private IGameFactory _gameFactory;
        private IAnimalHouseService _houseService;
        private IEffectService _effectService;
        
        private IAnimal _healingAnimal;
        private byte Id;
        private bool _isFree = true;
        
        private Location _workingEffectSpawnLocation;
        private Location _healingEffectSpawnLocation;
        
        private Effect _workingEffect;

        public event Action<IItem> Added = _ => { };
        public event Action<IItem> Removed = _ => { };
        public event Action<AnimalId> Healed = _ => { };
        public event Action FeedUp = () => { };

        public bool IsFree => _isFree;
        public MedicalToolId ThreatTool => _animalData?.TreatToolId ?? MedicalToolId.None;

        private void Awake()
        {
            _timerOperator ??= GetComponent<TimerOperator>();
            _timerOperator.SetUp(_healingTime, OnHealed);
            
            _gameFactory = AllServices.Container.Single<IGameFactory>();
            _houseService = AllServices.Container.Single<IAnimalHouseService>();
            _effectService = AllServices.Container.Single<IEffectService>();

            _workingEffectSpawnLocation = new Location(_spawnPlace.position, Quaternion.LookRotation(-Camera.main.transform.forward));
            _healingEffectSpawnLocation = new Location(_spawnPlace.position, Quaternion.LookRotation(Vector3.up));
        }

        public void Add(IItem item)
        {
            if (ItemIsAnimal(item))
            {
                _animalData = item.ItemData as AnimalItemData;
                _animalItem = item;
                _isFree = false;
            }
            else if (ItemIsMedTool(item))
            {
                _medicalToolData = item.ItemData as MedicalToolItemData;
                _medToolItem = item;
                BeginHeal();
            }

            Added.Invoke(item);
        }

        public bool CanAdd(IItem item) =>
            CanPlaceAnimal(item) || CanPlaceMedTool(item);

        public bool TryAdd(IItem item)
        {
            if (CanAdd(item) == false)
                return false;
            
            Add(item);
            return true;
        }

        private void OnHealed()
        {
            Debug.Log("Healed");
            _workingEffect.Stop();
            _effectService.SpawnEffect(EffectId.HealingPluses, _healingEffectSpawnLocation);
            _healingAnimal = _gameFactory.CreateAnimal(_animalData.StaticData, _spawnPlace.position, _spawnPlace.rotation)
                .GetComponent<Animal>();

            Healed.Invoke(_healingAnimal.AnimalId);

            RemoveItem(_animalItem);
            RemoveItem(_medToolItem);

            QueueToHouse queueToHouse = new QueueToHouse(_healingAnimal,() =>
            {
                FreeTheBad();
                FeedUp.Invoke();
                return _healingAnimal;
            });
            
            _houseService.TakeQueueToHouse(queueToHouse);
        }

        private void FreeTheBad()
        {
            _animalItem = null;
            _animalData = null;
            
            _medToolItem = null;
            _medicalToolData = null;

            _isFree = true;
        }

        private void BeginHeal()
        {
            _workingEffect = _effectService.SpawnEffect(EffectId.Working, _workingEffectSpawnLocation);
            _timerOperator.Restart();
        }

        private void RemoveItem(IItem item)
        {
            Removed.Invoke(item);
            item.Destroy();
        }

        private bool CanPlaceAnimal(IItem item) =>
            ItemIsAnimal(item) && HasAnimal() == false;

        private bool CanPlaceMedTool(IItem item)
        {
            return ItemIsMedTool(item)
                   && HasAnimal()
                   && IsSuitableTool(item)
                   && HasMedTool() == false;
        }

        private bool HasMedTool() =>
            _medicalToolData is not null;

        private bool IsSuitableTool(IItem item) =>
            _animalData.TreatToolId == ((MedicalToolItemData) item.ItemData).MedicineToolId;

        private bool HasAnimal() =>
            _animalData is not null;

        private bool ItemIsMedTool(IItem item) =>
            (item.ItemId & ItemId.Medical) != 0;

        private bool ItemIsAnimal(IItem item) =>
            (item.ItemId & ItemId.Animal) != 0;
    }
}
