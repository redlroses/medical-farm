using System.Collections.Generic;
using Infrastructure.Factory;
using NaughtyAttributes;
using NTC.Global.System;
using Services;
using UnityEngine;

namespace Logic.CellBuilding
{
    public abstract class BuildGridOperator : MonoBehaviour
    {
        private readonly Queue<BuildPlaceMarker> _positions = new Queue<BuildPlaceMarker>();

        [SerializeField] private List<BuildPlaceMarker> _buildPlaces;
        [SerializeField] private int _buildCost;
        [SerializeField] private bool _isAutoBuild = true;

        protected IGameFactory GameFactory;
        protected BuildCell ActiveBuildCell;
        
        private BuildPlaceMarker _currentMarker;
        private bool _isCellBuilt = true;

        private void Awake()
        {
            FillPositions();
            GameFactory = AllServices.Container.Single<IGameFactory>();
            _currentMarker = _positions.Peek();
            ActiveBuildCell = GameFactory.CreateBuildCell(_currentMarker.Location.Position, _currentMarker.Location.Rotation)
                .GetComponent<BuildCell>();
            ActiveBuildCell.transform.SetParent(transform, true);
            ActiveBuildCell.gameObject.SetActive(false);
            ActiveBuildCell.SetBuildCost(_buildCost);
            ActiveBuildCell.SetIcon(_currentMarker.Icon);
            ActiveBuildCell.Build += BuildAndActivate;
            OnAwake();
        }

        private void OnDestroy() =>
            ActiveBuildCell.Build -= BuildAndActivate;

        protected abstract void BuildCell(BuildPlaceMarker marker);

        protected virtual void OnAwake()
        {
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            
            if (_isAutoBuild)
            {
               BuildAndActivate();
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetAutoNext(bool isAuto)
        {
            _isAutoBuild = isAuto;
        }

        [Button("Show Next")]
        public void ShowNextBuildCell()
        {
            if (_isCellBuilt == false)
                return;

            if (_positions.TryDequeue(out BuildPlaceMarker marker))
            {
                ActiveBuildCell.Reposition(marker.Location);
                ActiveBuildCell.SetIcon(marker.Icon);
                _currentMarker = marker;
                _isCellBuilt = false;
                ActiveBuildCell.gameObject.Enable();
            }
        }

        private void FillPositions()
        {
            for (var index = 0; index < _buildPlaces.Count; index++)
            {
                BuildPlaceMarker marker = _buildPlaces[index];
                marker.Init();
                _positions.Enqueue(marker);
            }
        }
        
        private void BuildAndActivate()
        {
            BuildCell();
            
            if (_isAutoBuild)
            {
                ShowNextBuildCell();
            }
        }

        private void BuildCell()
        {
            BuildCell(_currentMarker);
            _isCellBuilt = true;
            ActiveBuildCell.gameObject.Disable();
        }
    }
}