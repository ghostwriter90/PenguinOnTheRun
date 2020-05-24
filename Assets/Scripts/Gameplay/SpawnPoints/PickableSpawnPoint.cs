using PenguinOnTheRun.Gameplay.Pickables;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay.SpawnPoints
{
    public abstract class PickableSpawnPoint : MonoBehaviour
    {
        public abstract void SpawnInstance();

        public void SpawnInstance(PickableObject copy)
        {
            copy.transform.SetParent(transform.parent);
            copy.transform.localPosition = transform.localPosition;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 165f, 0f, 0.7f);
            Gizmos.DrawCube(transform.position, Vector2.one * 3);
        }
    }
}
