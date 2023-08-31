using System;

namespace Data.SaveData
{
    [Serializable]
    public class GlobalData
    {
        public Settings Settings;
        public String LastLevel;
        
        public GlobalData(string initialLevel)
        {
            LastLevel = initialLevel;
            Settings = new Settings();
        }
    }
}