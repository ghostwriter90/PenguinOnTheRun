using PenguinOnTheRun.Gameplay.Pickables;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay.SpawnPoints
{
    [ExecuteInEditMode, SelectionBase]
    public class FishSpawnPoint : PickableSpawnPoint
    {
#pragma warning disable 649
        [SerializeField] private Fish fishSample;
#pragma warning restore 649

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
