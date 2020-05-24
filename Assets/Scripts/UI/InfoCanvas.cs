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
        [SerializeField] private CanvasItem[] fishesFull = new CanvasItem[maxFishCount];
        [SerializeField] private CanvasItem[] bonesFull = new CanvasItem[maxBoneCount];
        [SerializeField] private CanvasItem[] fishesMissing = new CanvasItem[maxFishCount];
        [SerializeField] private CanvasItem[] bonesMissing = new CanvasItem[maxBoneCount];
#pragma warning restore 649

        public static InfoCanvas Instance { get; private set; }

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
        }

        public void AddFish(int health)
        {
            if ((health < maxFishCount) && (health >= 0))
            {
                fishesFull[health].gameObject.SetActive(true);
                fishesMissing[health].gameObject.SetActive(false);
            }
        }

        public void RemoveFish(int health)
        {
            if (health > 0)
            {
                fishesFull[health].gameObject.SetActive(false);
                fishesMissing[health].gameObject.SetActive(true);
            }
        }

        public void AddBone(int bones)
        {
            if (bones < maxBoneCount)
            {
                bonesFull[bones].gameObject.SetActive(true);
                bonesMissing[bones].gameObject.SetActive(false);
            }
        }

        public void ResetBones()
        {
            for (int i = 0; i < maxBoneCount; ++i)
            {
                bonesFull[i].gameObject.SetActive(false);
                bonesMissing[i].gameObject.SetActive(true);
            }
        }

        public void GameOver()
        {
            for (int i = 0; i < maxFishCount; ++i)
            {
                fishesFull[i].gameObject.SetActive(false);
                fishesMissing[i].gameObject.SetActive(true);
            }

            gameOverPanel.SetActive(true);
        }

        private void OnReplay()
        {
            gameOverPanel.SetActive(false);
            SceneManager.LoadScene(mainSceneIndex);
        }
    }
}
