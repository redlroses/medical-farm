using System;
using Logic.Animals;
using Services.AnimalHouses;
using Services.StaticData;
using Ui.Factory;
using UnityEngine;

namespace Ui.Windows
{
    public class HouseBuildWindow : WindowBase
    {
        [SerializeField] private Transform _panelsParent;

        private Action<AnimalType> _onChoseCallback;

        public void SetOnChoseCallback(Action<AnimalType> callback) =>
            _onChoseCallback = callback;

        public void Construct(IAnimalHouseService houseService, IUIFactory uiFactory, IStaticDataService staticData)
        {
            foreach (QueueToHouse animal in houseService.AnimalsInQueue)
            {
                ChoseAnimalPanel panel = uiFactory.CreateChoseAnimalPanel(_panelsParent);
                panel.Construct(staticData.IconByAnimalType(animal.AnimalId.Type), () =>
                {
                    _onChoseCallback.Invoke(animal.AnimalId.Type);
                    CloseWindow();
                });
            }
        }
    }
}