using UnityEngine;

namespace PenguinOnTheRun.UI
{
    public class LoseCanvas : MonoBehaviour
    {
        public static LoseCanvas Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("LoseCanvas instance already exists!");
            }
            else
            {
                Instance = this;
            }
        }
    }
}
