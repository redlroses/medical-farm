using System;
using System.Collections.Generic;
using Logic.Animals;
using Logic.Animals.AnimalsBehaviour;
using Services.Animals;

namespace Data
{
    public class AnimalCounter : IAnimalCounter, IDisposable
    {
        private readonly Dictionary<AnimalType, AnimalCountData> _countDatas = new Dictionary<AnimalType, AnimalCountData>();
        private readonly Dictionary<AnimalId, AnimalSatietyObserver> _satietyObservers = new Dictionary<AnimalId, AnimalSatietyObserver>();

        public event Action<AnimalType, AnimalCountData> Updated = (_, _) => { };

        public void Register(IAnimal animal)
        {
            AnimalType animalType = animal.AnimalId.Type;

            if (_countDatas.ContainsKey(animalType))
            {
                _countDatas[animalType] = _countDatas[animalType].AddTotal();
            }
            else
            {
                var animalCountData = new AnimalCountData().AddTotal();
                _countDatas.Add(animalType, animalCountData);
            }

            _satietyObservers.Add(animal.AnimalId, new AnimalSatietyObserver(animal, this));
            Updated.Invoke(animalType, _countDatas[animalType]);
        }

        public void Unregister(IAnimal animal)
        {
            AnimalId animalId = animal.AnimalId;
            AnimalType animalType = animalId.Type;
            
            if (_countDatas.ContainsKey(animalType) == false)
                throw new ArgumentNullException();
            
            _countDatas[animalType] = _countDatas[animalType].RemoveTotal();

            AnimalSatietyObserver animalSatietyObserver = _satietyObservers[animalId];
            animalSatietyObserver.Dispose();
            _satietyObservers.Remove(animalId);
            
            if (_countDatas[animalType].Total == 0)
                _countDatas.Remove(animalType);
            
            Updated.Invoke(animalType, _countDatas[animalType]);
        }

        public AnimalCountData GetAnimalCountData(AnimalType byType)
        {
            if (_countDatas.TryGetValue(byType, out AnimalCountData data))
                return data;

            throw new ArgumentNullException();
        }

        public IReadOnlyDictionary<AnimalType, AnimalCountData> GetAllData() =>
            _countDatas;

        public void Dispose()
        {
            foreach ((_, AnimalSatietyObserver value) in _satietyObservers)
                value.Dispose();
            
            _countDatas.Clear();
            _satietyObservers.Clear();
        }

        private void AddReleaseReady(AnimalType animalType)
        {
            _countDatas[animalType] = _countDatas[animalType].AddReleaseReady();
            Updated.Invoke(animalType, _countDatas[animalType]);
        }

        private void RemoveReleaseReady(AnimalType animalType)
        {
            _countDatas[animalType] = _countDatas[animalType].RemoveReleaseReady();
            Updated.Invoke(animalType, _countDatas[animalType]);
        }

        private class AnimalSatietyObserver : IDisposable
        {
            private readonly IAnimal _animal;
            private readonly AnimalCounter _countData;
            private bool _isFullBar;

            public AnimalSatietyObserver(IAnimal animal, AnimalCounter countData)
            {
                _countData = countData;
                _animal = animal;
                animal.Stats.Satiety.Full += OnFullSatiety;
                animal.Stats.Satiety.Empty += OnEmptySatiety;
            }

            public void Dispose()
            {
                _animal.Stats.Satiety.Full -= OnFullSatiety;
                _animal.Stats.Satiety.Empty -= OnEmptySatiety;

                if (_isFullBar)
                    RemoveFromRelease();
            }

            private void OnEmptySatiety()
            {
                _isFullBar = false;
                RemoveFromRelease();
            }

            private void OnFullSatiety()
            {
                _isFullBar = true;
                AddToRelease();
            }

            private void AddToRelease() =>
                _countData.AddReleaseReady(_animal.AnimalId.Type);

            private void RemoveFromRelease() =>
                _countData.RemoveReleaseReady(_animal.AnimalId.Type);
        }
    }
}