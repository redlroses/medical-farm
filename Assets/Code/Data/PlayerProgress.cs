using System;
using Services.SaveLoad;

namespace Data
{
    [Serializable]
    public class PlayerProgress
    {
        public GlobalData GlobalData;
        public LevelData LevelData;

        public PlayerProgress(string initialLevel)
        {
            GlobalData = new GlobalData(initialLevel);
            LevelData = new LevelData(initialLevel);
        }
    }
}