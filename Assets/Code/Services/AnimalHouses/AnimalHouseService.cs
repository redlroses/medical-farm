using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Animals;
using Logic.Animals.AnimalsBehaviour;
using Infrastructure.Factory;
using JetBrains.Annotations;

namespace Services.AnimalHouses
{
    public class AnimalHouseService : IAnimalHouseService
    {
        private const string HouseNotFoundException = "An animal with this Id has not been assigned a home";

        private readonly IGameFactory _gameFactory;

        private readonly List<AnimalHouse> _animalHouses = new List<AnimalHouse>();
        private readonly List<QueueToHouse> _animalsInQueue = new List<QueueToHouse>();
        private readonly Queue<AnimalHouse> _feedQueue = new Queue<AnimalHouse>();

        public IReadOnlyList<QueueToHouse> AnimalsInQueue => _animalsInQueue;

        public void RegisterHouse(AnimalHouse house)
        {
            if (_animalHouses.Contains(house))
                throw new Exception($"House {house} already registered");

            _animalHouses.Add(house);
            house.BowlEmpty += AddToFeedQueue;
            AddToFeedQueue(house);
            TryTakeHouse(house);
        }

        public void TakeQueueToHouse(QueueToHouse queueToHouse, bool isHighPriority = false)
        {
            AnimalHouse freeHouse = GetFreeHouseFor(queueToHouse.AnimalId.Type);

            if (freeHouse is null)
            {
                _animalsInQueue.Add(queueToHouse);
            }
            else
            {
                IAnimal animal = queueToHouse.OnTakeHouse.Invoke();
                TakeHouse(freeHouse, animal);
            }
        }

        public void VacateHouse(AnimalId withAnimalId)
        {
            AnimalHouse attachedHouse =
                _animalHouses.FirstOrDefault(house => house.IsTaken && house.AnimalId.Type.Equals(withAnimalId.Type));

            if (attachedHouse is null)
                throw new NullReferenceException(HouseNotFoundException);

            attachedHouse.DetachAnimal();
            TryTakeHouse(attachedHouse);
        }

        public bool TryGetNextFeedHouse(out AnimalHouse feedHouse) =>
            _feedQueue.TryDequeue(out feedHouse);

        private void TryTakeHouse(AnimalHouse house)
        {
            if (TryGetAnimalFromQueue(house.ForAnimal, out QueueToHouse animalQueue))
            {
                IAnimal animal = animalQueue.OnTakeHouse.Invoke();
                _animalsInQueue.Remove(animalQueue);
                TakeHouse(house, animal);
            }
        }

        private bool TryGetAnimalFromQueue(AnimalType forHouseType, out QueueToHouse queueToHouse)
        {
            queueToHouse = new QueueToHouse();

            for (var index = 0; index < _animalsInQueue.Count; index++)
            {
                QueueToHouse queueAnimal = _animalsInQueue[index];

                if (queueAnimal.AnimalId.Type != forHouseType)
                    continue;

                queueToHouse = queueAnimal;
                return true;
            }

            return false;
        }

        private void AddToFeedQueue(AnimalHouse house) =>
            _feedQueue.Enqueue(house);

        [CanBeNull]
        private AnimalHouse GetFreeHouseFor(AnimalType animalIdType) =>
            _animalHouses.FirstOrDefault(house => house.IsTaken == false && house.ForAnimal == animalIdType);

        private void TakeHouse(AnimalHouse builtHouse, IAnimal animal)
        {
            builtHouse.AttachAnimal(animal.AnimalId);
            animal.AttachHouse(builtHouse);
        }
    }
}