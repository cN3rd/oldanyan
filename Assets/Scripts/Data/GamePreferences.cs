using System;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class GamePreferences
    {
        public int lastUsedSlot = -1;

        [Serializable]
        public class VisualSettings
        {
            public int screenWidth;
            public int screenHeight;
            public FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            public bool vsync;
        }
        public VisualSettings visualSettings = new();

        [Serializable]
        public class AudioSettings
        {
            public float masterVolume = 1f;
            public float musicVolume = 0.7f;
            public float ambientVolume = 0.7f;
            public float sfxVolume = 0.7f;
        }
        public AudioSettings audioSettings = new();
    }
}
