using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PenguinOnTheRun.UI
{
    public class InfoCanvas : MonoBehaviour
    {
        private const int mainSceneIndex = 0;
        private const int maxFishCount = 3;
        private const int maxBoneCount = 3;
#pragma warning disable 649
        [Header("GameOver")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button replayButton;

        [Header("GamePlay")]
        [SerializeField] private GameObject[] fishesFull = new GameObject[maxFishCount];
        [SerializeField] private GameObject[] bonesFull = new GameObject[maxBoneCount];
        [SerializeField] private GameObject[] fishesMissing = new GameObject[maxFishCount];
        [SerializeField] private GameObject[] bonesMissing = new GameObject[maxBoneCount];

        [Header("Sound")]
        [SerializeField] private Button muteSound;
        [SerializeField] private Button loudSound;
#pragma warning restore 649

        public static InfoCanvas Instance { get; private set; }

        public static Action<bool> ToggleSound;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Canvas instance already exists!");
            }
            else
            {
                Instance = this;
            }

            replayButton.onClick.AddListener(OnReplay);
            muteSound.onClick.AddListener(LoudSound);
            loudSound.onClick.AddListener(MuteSound);
        }

        public void AddFish(int health)
        {
            if ((health < maxFishCount) && (health >= 0))
            {
                fishesFull[health].SetActive(true);
                fishesMissing[health].SetActive(false);
            }
        }

        public void RemoveFish(int health)
        {
            if (health > 0)
            {
                fishesFull[health].SetActive(false);
                fishesMissing[health].SetActive(true);
            }
        }

        public void AddBone(int bones)
        {
            if (bones < maxBoneCount)
            {
                bonesFull[bones].SetActive(true);
                bonesMissing[bones].SetActive(false);
            }
        }

        public void ResetBones()
        {
            for (int i = 0; i < maxBoneCount; ++i)
            {
                bonesFull[i].SetActive(false);
                bonesMissing[i].SetActive(true);
            }
        }

        public void GameOver()
        {
            for (int i = 0; i < maxFishCount; ++i)
            {
                fishesFull[i].SetActive(false);
                fishesMissing[i].SetActive(true);
            }

            gameOverPanel.SetActive(true);
        }

        private void OnReplay()
        {
            gameOverPanel.SetActive(false);
            SceneManager.LoadScene(mainSceneIndex);
        }

        private void MuteSound()
        {
            muteSound.gameObject.SetActive(true);
            loudSound.gameObject.SetActive(false);
            ToggleSound?.Invoke(false);
        }

        private void LoudSound()
        {
            muteSound.gameObject.SetActive(false);
            loudSound.gameObject.SetActive(true);
            ToggleSound?.Invoke(true);
        }
    }
}
