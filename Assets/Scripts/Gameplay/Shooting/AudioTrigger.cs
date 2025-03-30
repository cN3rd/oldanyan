using UnityEngine;

namespace Game.Gameplay.Shooting
{
    public class MyAudioTrigger : MonoBehaviour
    {
        public float time = 3;
        public AudioClip onClip;

        void Start()
        {
            var audio = gameObject.AddComponent<AudioSource>();
            if (onClip != null)
            {
                audio.clip = onClip;
                audio.Play();
            }

            Destroy(gameObject, time);
        }
    }
}
