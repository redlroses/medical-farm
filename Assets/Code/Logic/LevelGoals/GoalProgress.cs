using System;
using System.Collections.Generic;
using Data;
using Logic.Animals;
using Progress;

namespace Logic.LevelGoals
{
    public class GoalProgress : IGoalProgressView
    {
        private readonly Dictionary<AnimalType, ProgressBar> _releaseProgressBars;
        
        private int _notFullBarCount;

        public event Action Compleated = () => { };

        public GoalProgress(SingleGoalData goalData)
        {
            int capacity = goalData.AnimalsToRelease.Count;
            _releaseProgressBars = new Dictionary<AnimalType, ProgressBar>(capacity);

            foreach (var (animalType, goalAmount) in goalData.AnimalsToRelease)
                RegisterProgressBar(animalType, goalAmount);
        }

        public void AddToReleased(AnimalType withType)
        {
            if (_releaseProgressBars.TryGetValue(withType, out ProgressBar bar))
                bar.Increment();
            else
                throw new NullReferenceException(nameof(withType));
        }

        public Observables.IObservable<float> GetProgressAmount(AnimalType byType)
        {
            if (_releaseProgressBars.TryGetValue(byType, out ProgressBar bar))
                return bar.Current;

            throw new ArgumentNullException(nameof(byType));
        }

        private void RegisterProgressBar(AnimalType animalType, int goalAmount)
        {
            ProgressBar progressBar = new ProgressBar(goalAmount);
            _releaseProgressBars.Add(animalType, progressBar);

            if (progressBar.IsFull)
                return;

            void OnFull()
            {
                CheckForGoal();
                progressBar.Full -= OnFull;
            }
            
            progressBar.Full += OnFull;
        }

        private void CheckForGoal()
        {
            _notFullBarCount--;
            
            if (_notFullBarCount <= 0)
                Compleated.Invoke();
        }
    }
}