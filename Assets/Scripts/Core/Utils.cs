using UnityEngine;

namespace Game.Core
{
    public static class Utils
    {
        const float DEFAULT_MIN_DB = -80f;
        const float DEFAULT_MAX_DB = 0f;

        public static float LinearVolumeToDecibel(float linearVolume, float minDb = DEFAULT_MIN_DB, float maxDb = DEFAULT_MAX_DB)
        {
            linearVolume = Mathf.Clamp01(linearVolume);
            return linearVolume > 0
                ? Mathf.Clamp(20f * Mathf.Log10(linearVolume), minDb, maxDb)
                : minDb;
        }

        public static float DecibelToLinearVolume(float dB, float minDb = DEFAULT_MIN_DB, float maxDb = DEFAULT_MAX_DB) =>
            Mathf.Clamp01(Mathf.Pow(10f, Mathf.Clamp(dB, minDb, maxDb) / 20f));
    }
}
