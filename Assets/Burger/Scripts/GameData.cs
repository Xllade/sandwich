using System;
using System.Collections.Generic;

namespace Burger
{
    [Serializable]
    public class GameData
    {
        public List<LevelData> levelData = new List<LevelData>();
        public int levelIndex;
        public float sfxVolume;
        public float bgmVolume;
        public bool vibrate;
    }
}