using System;

namespace Game.Data
{
    [Serializable]
    public class GameSave
    {
        [Serializable]
        public class CheckpointData
        {
            public string sceneGuid;
            public int id;
        }

        public CheckpointData currentCheckpoint;
    }
}
