using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    [ExecuteInEditMode, SelectionBase]
    public class FishSpawnPoint : PickableSpawnPoint
    {
        [SerializeField] private Fish fishSample;

        public override void SpawnInstance()
        {
            Fish copy = Instantiate(fishSample);
            SpawnInstance(copy);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 0, 1f, 0.7f);
            Gizmos.DrawCube(transform.position, Vector2.one * 3);
        }
    }
}
