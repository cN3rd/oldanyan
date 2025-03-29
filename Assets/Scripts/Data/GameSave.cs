using System;
using System.Diagnostics.CodeAnalysis;

namespace Game.Data
{
    [Serializable]
    public class GameSave
    {
        [Serializable]
        public struct CheckpointData
        {
            public string sceneGuid;
            public int id;
        }

        public CheckpointData currentCheckpoint;
    }
}
