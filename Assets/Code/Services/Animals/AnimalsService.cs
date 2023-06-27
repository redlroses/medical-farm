using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Animals;
using Logic.Animals.AnimalsBehaviour;
using Services.AnimalHouses;
using UnityEngine;

namespace Services.Animals
{
    public class AnimalsService : IAnimalsService
    {
        private readonly IAnimalHouseService _houseService;
        private readonly List<IAnimal> _animals = new List<IAnimal>();

        public AnimalsService(IAnimalHouseService houseService)
        {
            _houseService = houseService;
        }

        public event Action<AnimalType> Released = t => { };
        
        public IReadOnlyList<IAnimal> Animals => _animals;

        public void Register(IAnimal animal)
        {
            if (_animals.Contains(animal))
                throw new Exception($"Animal {animal} already registered");

            _animals.Add(animal);

            Debug.Log($"Animal {animal.AnimalId.Type} (id: {animal.AnimalId.ID}) registered");
        }

        public void Release(IAnimal animal)
        {
            ReleaseAnimal(animal);
        }

        public void Release(AnimalType animalType)
        {
            IAnimal unregisterAnimal = Animals.First(animal => animal.AnimalId.Type == animalType);
            ReleaseAnimal(unregisterAnimal);
        }

        public IAnimal[] GetAnimals(AnimalType panelAnimalType) =>
            _animals.FindAll(animal => animal.AnimalId.Type == panelAnimalType).ToArray();

        public AnimalCountData GetAnimalsCount(AnimalType panelAnimalType)
        {
            IAnimal[] animals = GetAnimals(panelAnimalType);

            int total = animals.Length;
            int releaseReady = animals.Count(animal => animal.Stats.Vitality.IsFull);

            return new AnimalCountData(total, releaseReady);
        }

        private void ReleaseAnimal(IAnimal releasedAnimal)
        {
            Unregister(releasedAnimal);
            _houseService.VacateHouse(releasedAnimal.AnimalId);
            releasedAnimal.Destroy();
            Released.Invoke(releasedAnimal.AnimalId.Type);

            Debug.Log($"Animal {releasedAnimal.AnimalId.Type} (id: {releasedAnimal.AnimalId.ID}) released");
        }

        private void Unregister(IAnimal animal)
        {
            if (_animals.Contains(animal) == false)
                throw new Exception($"Animal {animal} wasn't registered");

            _animals.Remove(animal);
        }
    }
}