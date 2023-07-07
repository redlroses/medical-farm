using System;
using Data.ItemsData;
using Infrastructure.Factory;
using Logic.Animals.AnimalsBehaviour;
using Logic.Interactions;
using Logic.Player;
using Logic.Storages;
using Logic.Storages.Items;
using NTC.Global.Cache;
using Services;
using Services.AnimalHouses;
using UnityEngine;

namespace Logic.Medicine
{
    [RequireComponent(typeof(TimerOperator))]
    public class MedicineBed : MonoCache, IAddItem, IGetItemObserver, IAddItemProvider, IGetItemObserverProvider
    {
        [SerializeField] private Transform _spawnPlace;
        [SerializeField] private PlayerInteraction _playerInteraction;
        [SerializeField] private TimerOperator _timerOperator;
        [SerializeField] [Range(1f, 5f)] private float _healingTime = 2.5f;

        private AnimalItemData _animalData;
        private MedToolItemData _medToolData;

        private IItem _animalItem;
        private IItem _medToolItem;

        private IGameFactory _gameFactory;
        private IAnimalHouseService _houseService;

        private bool _isHealing;
        
        public event Action<IItem> Added = i => { };
        
        public event Action<IItem> Removed = i => { };
        public event Action Healed = () => { };

        public IAddItem ItemAdder => this;
        public IGetItemObserver GetItemObserver => this;

        private void Awake()
        {
            _timerOperator ??= GetComponent<TimerOperator>();
            _timerOperator.SetUp(_healingTime, OnHealed);
            
            _gameFactory = AllServices.Container.Single<IGameFactory>();
            _houseService = AllServices.Container.Single<IAnimalHouseService>();
        }

        protected override void OnEnabled()
        {
            _playerInteraction.Entered += OnEntered;
            _playerInteraction.Canceled += OnCanceled;
        }

        protected override void OnDisabled()
        {
            _playerInteraction.Entered -= OnEntered;
            _playerInteraction.Canceled -= OnCanceled;
        }

        public void Add(IItem item)
        {
            Added.Invoke(item);
            
            if (ItemIsAnimal(item))
            {
                _animalData = item.ItemData as AnimalItemData;
                _animalItem = item;
            }
                

            if (ItemIsMedTool(item))
            {
                _medToolData = item.ItemData as MedToolItemData;
                _medToolItem = item;
                BeginHeal();
            }
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
            Healed.Invoke();
            _isHealing = false;
            
            Animal animal = _gameFactory.CreateAnimal(_animalData.AnimalId.Type, _spawnPlace.position)
                .GetComponent<Animal>();
            
            _houseService.TakeQueueToHouse(() =>
            {
                FreeTheBad();
                return animal;
            });
        }

        private void OnCanceled()
        {
            _timerOperator.Pause();
        }

        private void OnEntered(Hero _)
        {
            if (_isHealing)
                _timerOperator.Play();
        }

        private void FreeTheBad()
        {
            _animalItem = null;
            _animalData = null;
            
            _medToolItem = null;
            _medToolData = null;
        }

        private void BeginHeal()
        {
            _timerOperator.Restart();
            _isHealing = true;

            RemoveItem(_animalItem);
            RemoveItem(_medToolItem);

            Debug.Log("Begin heal");
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
            _medToolData is not null;

        private bool IsSuitableTool(IItem item) =>
            _animalData.TreatToolId == ((MedToolItemData) item.ItemData).MedicineToolId;

        private bool HasAnimal() =>
            _animalData is not null;

        private bool ItemIsMedTool(IItem item) =>
            (item.ItemId & ItemId.Medical) != 0;

        private bool ItemIsAnimal(IItem item) =>
            (item.ItemId & ItemId.Animal) != 0;
    }
}
