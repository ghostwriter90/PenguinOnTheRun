using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    [ExecuteInEditMode, SelectionBase]
    public class BoneSpawnPoint : PickableSpawnPoint
    {
        [SerializeField] private Bone boneSample;

        public override void SpawnInstance()
        {
            Bone copy = Instantiate(boneSample);
            SpawnInstance(copy);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 165f, 0f, 0.7f);
            Gizmos.DrawCube(transform.position, Vector2.one * 3);
        }
    }
}
