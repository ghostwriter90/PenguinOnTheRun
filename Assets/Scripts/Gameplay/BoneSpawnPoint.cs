using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    [ExecuteInEditMode, SelectionBase]
    public class BoneSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Bone boneSample;

        private int laneIndex;
        private float distanceFromCarEntrance;

        public void SpawnInstance()
        {
            Bone copy = Instantiate(boneSample);
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
            Gizmos.color = new Color(255f, 165f, 0f, 0.7f);
            Gizmos.DrawCube(transform.position, Vector2.one * 3);
        }
    }
}
