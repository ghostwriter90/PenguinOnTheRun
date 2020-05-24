using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    [ExecuteInEditMode, SelectionBase]
    public class ConductorSpawnPoint : MonoBehaviour
    {
        private int laneIndex;
        private float distanceFromCarEntrance;

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
            Gizmos.color = new Color(1f, 0f, 0f, 0.7f);
            Gizmos.DrawCube(transform.position, Vector2.one * 3);
        }
    }
}
