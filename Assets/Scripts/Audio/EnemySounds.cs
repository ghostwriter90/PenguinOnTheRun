using PenguinOnTheRun.Gameplay;
using UnityEngine;

namespace PenguinOnTheRun.Audio
{
    public class EnemySounds : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] AudioClip[] smallConductorSounds;
        [SerializeField] AudioClip[] tallConductorSounds;
        [SerializeField] AudioClip[] grandmaSounds;
#pragma warning restore 649

        private AudioSource soundSource;

        public static EnemySounds Instance;

        void Awake()
        {
            soundSource = GetComponent<AudioSource>();

            if (soundSource == null)
            {
                Debug.LogWarning("Sound source not found!");
            }

            if (Instance != null)
            {
                Debug.LogWarning("ConductorSounds instance already exists!");
            }
            else
            {
                Instance = this;
            }
        }

        public void PlayEnemySound(Enemy.EnemyType enemyType)
        {
            switch (enemyType)
            {
                case Enemy.EnemyType.SMALL_CONDUCTOR:
                    PlaySmallConductorSound();
                    break;

                case Enemy.EnemyType.TALL_CONDUCTOR:
                    PlayTallConductorSound();
                    break;

                default: break;
            }
        }

        public void PlaySmallConductorSound()
        {
            int rndInd = Random.Range(0, smallConductorSounds.Length - 1);

            soundSource.PlayOneShot(smallConductorSounds[rndInd]);
        }

        public void PlayTallConductorSound()
        {
            int rndInd = Random.Range(0, tallConductorSounds.Length - 1);

            soundSource.PlayOneShot(tallConductorSounds[rndInd]);
        }

        public void PlayGrandmaSound()
        {
            int rndInd = Random.Range(0, tallConductorSounds.Length - 1);

            soundSource.PlayOneShot(grandmaSounds[rndInd]);
        }
    }
}
