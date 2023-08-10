using Logic.Animals;
using NaughtyAttributes;
using Services;
using Services.AnimalHouses;
using Ui.Services;
using Ui.Windows;
using UnityEngine;

namespace Logic.CellBuilding
{
    public class HouseGridOperator : BuildGridOperator
    {
        private BuildPlaceMarker _cashedMarker;
        private IAnimalHouseService _houseService;
        private IWindowService _windowsService;

        protected override void OnAwake()
        {
            _houseService = AllServices.Container.Single<IAnimalHouseService>();
            _windowsService = AllServices.Container.Single<IWindowService>();
        }

        protected override void BuildCell(BuildPlaceMarker marker)
        {
            _cashedMarker = marker;

            if (IsOpenChoseHouseWindow())
            {
                HouseBuildWindow window = _windowsService.Open(WindowId.BuildHouse).GetComponent<HouseBuildWindow>();
                window.SetOnChoseCallback(OnAnimalChosen);
            }
            else
            {
                OnAnimalChosen(_houseService.AnimalsInQueue[0].AnimalId.Type);
            }
        }

        private bool IsOpenChoseHouseWindow()
        {
#if DEBUG
            return true;
#else
            return _houseService.AnimalsInQueue.Count > 1;
#endif
        }

        private void OnAnimalChosen(AnimalType type)
        {
            AnimalHouse house = GameFactory.CreateAnimalHouse(_cashedMarker.BuildPosition, _cashedMarker.Location.Rotation, type).GetComponent<AnimalHouse>();
            _houseService.RegisterHouse(house);
        }
    }
}