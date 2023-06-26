﻿using Logic.Animals.AnimalsBehaviour.Emotions;

namespace Services.StaticData
{
  public interface IStaticDataService : IService
  {
    void Load();
    Emotion EmotionById(EmotionId emotionId);
  }
}