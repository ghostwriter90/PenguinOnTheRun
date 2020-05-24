using UnityEngine;

namespace PenguinOnTheRun.UI
{
    public class InfoCanvas : MonoBehaviour
    {

        [SerializeField] CanvasItem[] fishesFull = new CanvasItem[3];
        [SerializeField] CanvasItem[] bonesFull = new CanvasItem[3];
        [SerializeField] CanvasItem[] fishesMissing = new CanvasItem[3];
        [SerializeField] CanvasItem[] bonesMissing = new CanvasItem[3];

        public static InfoCanvas Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Canvas instance already exists!");
            }
            else
            {
                Instance = this;
            }
        }

        public void AddFish(int health)
        {
            if ((health < 3) && (health >= 0))
            {
                fishesFull[health].gameObject.SetActive(true);
                fishesMissing[health].gameObject.SetActive(false);

                Debug.Log("add fish = " + health, fishesFull[health].gameObject);
            }
        }

        public void DamageFish(int health)
        {
            if (health > 0)
            {
                fishesFull[health].gameObject.SetActive(false);
                fishesMissing[health].gameObject.SetActive(true);
            }
        }

        public void AddBone(int bones)
        {
            if (bones < 3)
            {
                bonesFull[bones].gameObject.SetActive(true);
                bonesMissing[bones].gameObject.SetActive(false);
            }
        }

        public void ZeroBones()
        {
            for (int i = 0; i < 3; ++i)
            {
                bonesFull[i].gameObject.SetActive(false);
                bonesMissing[i].gameObject.SetActive(true);
            }
        }

        public void Die()
        {
            for (int i = 0; i < 3; ++i)
            {
                fishesFull[i].gameObject.SetActive(false);
                fishesMissing[i].gameObject.SetActive(true);
            }
        }
    }
}
