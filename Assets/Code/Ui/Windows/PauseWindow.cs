﻿namespace Ui.Windows
{
    public class PauseWindow : WindowBase
    {
        // [SerializeField] private SimpleButton _unpauseButton;
        // [SerializeField] private RestartButton _restartButton;
        // [SerializeField] private MenuButton _menuButton;
        //
        // private IPauseService _pauseService;
        // private IPersistentProgressService _progressService;
        //
        // private int LevelId => _progressService.Progress.WorldData.LevelState.LevelId;
        //
        // public void Construct(IPersistentProgressService progressService, IPauseService pauseService,
        //     GameStateMachine stateMachine)
        // {
        //     _pauseService = pauseService;
        //     _progressService = progressService;
        //     _restartButton.Construct(stateMachine, LevelId);
        //     _menuButton.Construct(stateMachine);
        // }
        //
        // public void Pause() { }
        //
        // public void Resume()
        // {
        //     _pauseService.Unregister(this);
        //     Destroy(gameObject);
        // }
        //
        // protected override void SubscribeUpdates()
        // {
        //     _unpauseButton.Subscribe(() => _pauseService.SetPause(false));
        //     _menuButton.Subscribe();
        //     _restartButton.Subscribe();
        // }
        //
        // protected override void Cleanup() =>
        //     _unpauseButton.Cleanup();
    }
}