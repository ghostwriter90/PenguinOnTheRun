using UnityEngine;

namespace PenguinOnTheRun.Audio
{
    public class PenguinSounds : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private AudioClip[] happySounds;
        [SerializeField] private AudioClip[] upSounds;
        [SerializeField] private AudioClip[] downSounds;
        [SerializeField] private AudioClip[] sadSounds;
#pragma warning restore 649

        private AudioSource soundSource;

        public static PenguinSounds Instance;

        void Awake()
        {
            soundSource = GetComponent<AudioSource>();

            if (soundSource == null)
            {
                Debug.LogWarning("Sound source not found!");
            }

            if (Instance != null)
            {
                Debug.LogWarning("PenguinSounds instance already exists!");
            }
            else
            {
                Instance = this;
            }
        }

        public void PlayHappySound()
        {
            PlayRandomSound(happySounds);
        }

        public void PlayUpSound()
        {
            PlayRandomSound(upSounds);
        }

        public void PlayDownSound()
        {
            PlayRandomSound(downSounds);
        }

        public void PlaySadSound()
        {
            PlayRandomSound(sadSounds);
        }

        void PlayRandomSound(AudioClip[] sounds)
        {
            int rndInd = Random.Range(0, sounds.Length - 1);

            soundSource.PlayOneShot(sounds[rndInd]);
        }
    }
}
