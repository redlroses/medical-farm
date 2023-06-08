using NaughtyAttributes;
using NTC.Global.Cache;
using UnityEngine;

namespace Logic.AnimalsBehaviour
{
    public class AnimalIndicators : MonoCache
    {
        [SerializeField] [ProgressBar("Vitality", 100f, EColor.Green)] private float _vitalityValue;
        [SerializeField] [ProgressBar("Satiety", 100f, EColor.Red)] private float _satietyValue;
        [SerializeField] [ProgressBar("Peppiness", 100f, EColor.Violet)] private float _peppinessValue;
        
        [SerializeField]  private ProgressBarIndicator _vitality;
        [SerializeField]  private ProgressBarIndicator _satiety;
        [SerializeField]  private ProgressBarIndicator _peppiness;

        protected override void Run()
        {
            _vitalityValue = _vitality.ProgressBar.Current;
            _satietyValue = _satiety.ProgressBar.Current;
            _peppinessValue = _peppiness.ProgressBar.Current;
        }
    }
}