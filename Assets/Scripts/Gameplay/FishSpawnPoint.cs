using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    [ExecuteInEditMode, SelectionBase]
    public class FishSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Fish fishSample;

        private int laneIndex;
        private float distanceFromCarEntrance;

        public void SpawnInstance()
        {
            Fish copy = Instantiate(fishSample);
            copy.transform.SetParent(transform.parent);
            copy.transform.localPosition = transform.localPosition;
        }

        public void SetData(int laneIndex, float distanceFromCarEntrance)
        {
            this.laneIndex = laneIndex;
            this.distanceFromCarEntrance = distanceFromCarEntrance;
        }

        public int GetLaneIndex()
        {
            return laneIndex;
        }

        public float GetDistanceFromCarEntrance()
        {
            return distanceFromCarEntrance;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 0, 1f, 0.7f);
            Gizmos.DrawCube(transform.position, Vector2.one * 3);
        }
    }
}
