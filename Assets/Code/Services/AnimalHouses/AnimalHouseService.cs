using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Animals;
using Logic.Animals.AnimalsBehaviour;
using Infrastructure.Factory;
using JetBrains.Annotations;
using Logic.Breeding;

namespace Services.AnimalHouses
{
    public class AnimalHouseService : IAnimalHouseService
    {
        private const string HouseNotFoundException = "An animal with this Id has not been assigned a home";

        private readonly IGameFactory _gameFactory;

        private readonly List<AnimalHouse> _animalHouses = new List<AnimalHouse>();
        private readonly List<BreedingHouse> _breedingHouses = new List<BreedingHouse>();
        private readonly List<QueueToHouse> _animalsInQueue = new List<QueueToHouse>();
        private readonly List<QueueToHouse> _animalsInPriorityQueue = new List<QueueToHouse>();
        private readonly Queue<IAnimalHouse> _feedQueue = new Queue<IAnimalHouse>();

        public IReadOnlyList<QueueToHouse> AnimalsInQueue => _animalsInQueue;

        public void RegisterHouse(IAnimalHouse house)
        {
            switch (house)
            {
                case AnimalHouse animalHouse:
                    RegisterAnimalHose(animalHouse);
                    break;
                case BreedingHouse breedingHouse:
                    RegisterBreedingHouse(breedingHouse);
                    break;
            }
        }

        private void RegisterBreedingHouse(BreedingHouse breedingHouse)
        {
            if (breedingHouse.IsServedByKeeper)
                breedingHouse.BowlEmpty += AddToFeedQueue;

            _breedingHouses.Add(breedingHouse);
        }

        private void RegisterAnimalHose(AnimalHouse animalHouse)
        {
            if (_animalHouses.Contains(animalHouse))
                throw new Exception($"House {animalHouse} already registered");

            animalHouse.BowlEmpty += AddToFeedQueue;
            _animalHouses.Add(animalHouse);
            AddToFeedQueue(animalHouse);
            TryTakeHouse(animalHouse);
        }

        public void TakeQueueToHouse(QueueToHouse queueToHouse, bool isHighPriority = false)
        {
            AnimalHouse freeHouse = GetFreeHouseFor(queueToHouse.AnimalId.Type);

            List<QueueToHouse> targetQueue = GetQueueByPriority(isHighPriority);
            
            if (freeHouse is null)
            {
                targetQueue.Add(queueToHouse);
            }
            else
            {
                IAnimal animal = queueToHouse.OnTakeHouse.Invoke();
                TakeHouse(freeHouse, animal);
            }
        }

        private List<QueueToHouse> GetQueueByPriority(bool isHighPriority) =>
            isHighPriority ? _animalsInPriorityQueue : _animalsInQueue;

        public void VacateHouse(AnimalId withAnimalId)
        {
            AnimalHouse attachedHouse =
                _animalHouses.FirstOrDefault(house => house.IsTaken && house.AnimalId.Type.Equals(withAnimalId.Type));

            if (attachedHouse is null)
                throw new NullReferenceException(HouseNotFoundException);

            attachedHouse.DetachAnimal();
            attachedHouse.Clear();
            TryTakeHouse(attachedHouse);
        }

        public bool TryGetNextFeedHouse(out IAnimalHouse feedHouse) =>
            _feedQueue.TryDequeue(out feedHouse);

        public bool TryGetNextAnimalInQueue(out AnimalId animalId)
        {
            if (_animalsInQueue.Count > 0)
            {
                animalId = _animalsInQueue[0].AnimalId;
                return true;
            }

            animalId = null;
            return false;
        }

        private void TryTakeHouse(AnimalHouse house)
        {
            if (TryGetAnimalFromQueue(_animalsInPriorityQueue, house.ForAnimal, out QueueToHouse animalQueue))
            {
                SendTheAnimalHome(house, _animalsInPriorityQueue, animalQueue);
            }
            else if (TryGetAnimalFromQueue(_animalsInQueue, house.ForAnimal, out animalQueue))
            {
                SendTheAnimalHome(house, _animalsInQueue, animalQueue);
            }
        }

        private void SendTheAnimalHome(AnimalHouse house, List<QueueToHouse> queue, QueueToHouse animalQueue)
        {
            IAnimal animal = animalQueue.OnTakeHouse.Invoke();
            queue.Remove(animalQueue);
            TakeHouse(house, animal);
        }

        private bool TryGetAnimalFromQueue(List<QueueToHouse> queue, AnimalType forHouseType, out QueueToHouse queueToHouse)
        {
            queueToHouse = new QueueToHouse();
            
            for (var index = 0; index < queue.Count; index++)
            {
                QueueToHouse queueAnimal = queue[index];

                if (queueAnimal.AnimalId.Type != forHouseType)
                    continue;

                queueToHouse = queueAnimal;
                return true;
            }

            return false;
        }

        private void AddToFeedQueue(IAnimalHouse house) =>
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