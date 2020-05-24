using PenguinOnTheRun.UI;
using UnityEngine;
using UnityEngine.Audio;

namespace PenguinOnTheRun.Audio
{
    public class SoundHandler : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private AudioMixer audioMixer;
#pragma warning restore 649

        private const string masterVolumeKey = "MasterVolume";
        private const float minVolume = -80f;
        private const float maxVolume = -10f;

        private void OnEnable()
        {
            InfoCanvas.ToggleSound += ToggleMute;
        }

        private void OnDisable()
        {
            InfoCanvas.ToggleSound -= ToggleMute;
        }

        private void ToggleMute(bool isPlaying)
        {
            float volume = isPlaying ? maxVolume : minVolume;

            audioMixer.SetFloat(masterVolumeKey, volume);
        }
    }
}
