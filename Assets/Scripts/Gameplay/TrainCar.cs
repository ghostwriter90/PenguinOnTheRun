using System.Collections.Generic;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    public class TrainCar : MonoBehaviour
    {
        public float length;
        public Lane[] lanes;
        public PickableObject[] pickableObjects;

        public const int mainLaneIndex = 2;

        private List<Enemy> enemies;

        public void Initialize()
        {
            enemies = new List<Enemy>();
            Initialize(2, 2, 4, 1);
        }

        public void Initialize(int conductorCount, int grandmaCount, int boneCount, int fishCount)
        {
            for (int i = 0; i < lanes.Length; i++)
            {
                lanes[i].Initialize(this, i);
            }

            SpawnConductors(conductorCount);
            SpawnBones(boneCount);
            SpawnFishes(fishCount);
            InitializeChairOrGrandmas(grandmaCount);

            for (int i = 0; i < lanes.Length; i++)
            {
                lanes[i].UpdateObstacleList();
                lanes[i].UpdatePickableObjectList();
            }
        }

        void InitializeChairOrGrandmas(int grandmaCount)
        {
            ChairOrGrandma[] chairOrGrandmas = GetComponentsInChildren<ChairOrGrandma>();
            HashSet<int> selectedIndexSet = new HashSet<int>();

            for (int i = 0; i < Mathf.Min(grandmaCount, chairOrGrandmas.Length); i++)
            {
                int selectedIndex;
                do
                {
                    selectedIndex = UnityEngine.Random.Range(0, chairOrGrandmas.Length);
                }
                while (selectedIndexSet.Contains(selectedIndex));
                selectedIndexSet.Add(selectedIndex);
            }

            for (int i = 0; i < chairOrGrandmas.Length; i++)
            {
                chairOrGrandmas[i].Initialize(selectedIndexSet.Contains(i));
            }
        }

        void SpawnConductors(int conductorCount)
        {
            ConductorSpawnPoint[] conductorSpawnPoints = GetComponentsInChildren<ConductorSpawnPoint>();
            HashSet<int> selectedIndexList = new HashSet<int>();

            for (int i = 0; i < Mathf.Min(conductorCount, conductorSpawnPoints.Length); i++)
            {
                int selectedIndex;
                do
                {
                    selectedIndex = UnityEngine.Random.Range(0, conductorSpawnPoints.Length);
                }
                while (selectedIndexList.Contains(selectedIndex));

                selectedIndexList.Add(selectedIndex);
                ConductorSpawnPoint spawnPoint = conductorSpawnPoints[selectedIndex];

                Enemy.LaneSet laneSet = lanes[spawnPoint.GetLaneIndex()].IsLuggageLane()
                    ? Enemy.LaneSet.LUGGAGE_LANE
                    : Enemy.LaneSet.PASSANGER_LANE;

                Enemy conductor = EnemyPool.GetEnemy(laneSet);
                conductor.SetTrainCar(this, spawnPoint.GetLaneIndex(), spawnPoint.GetDistanceFromCarEntrance());
                enemies.Add(conductor);
            }
        }

        void SpawnBones(int boneCount)
        {
            BoneSpawnPoint[] boneSpawnPoints = GetComponentsInChildren<BoneSpawnPoint>();
            HashSet<int> selectedIndexList = new HashSet<int>();

            for (int i = 0; i < Mathf.Min(boneCount, boneSpawnPoints.Length); i++)
            {
                int selectedIndex;
                do
                {
                    selectedIndex = UnityEngine.Random.Range(0, boneSpawnPoints.Length);
                }
                while (selectedIndexList.Contains(selectedIndex));

                selectedIndexList.Add(selectedIndex);
                BoneSpawnPoint spawnPoint = boneSpawnPoints[selectedIndex];
                spawnPoint.SpawnInstance();
            }
        }

        void SpawnFishes(int fishCount)
        {
            FishSpawnPoint[] fishSpawnPoints = GetComponentsInChildren<FishSpawnPoint>();
            HashSet<int> selectedIndexList = new HashSet<int>();

            for (int i = 0; i < Mathf.Min(fishCount, fishSpawnPoints.Length); i++)
            {
                int selectedIndex;
                do
                {
                    selectedIndex = UnityEngine.Random.Range(0, fishSpawnPoints.Length);
                }
                while (selectedIndexList.Contains(selectedIndex));

                selectedIndexList.Add(selectedIndex);
                FishSpawnPoint spawnPoint = fishSpawnPoints[selectedIndex];
                spawnPoint.SpawnInstance();
            }
        }

        public bool IsTrainCarExit(int laneIndex, float distanceFromCarEntrance, float accaptableNoise)
        {
            if ((distanceFromCarEntrance >= lanes[laneIndex].transform.localPosition.x + length - accaptableNoise)
                && (laneIndex == mainLaneIndex))
            {
                return true;
            }

            return false;
        }

        public bool IsTrainCarEdge(int laneIndex, float distanceFromCarEntrance, float lenght)
        {
            if (distanceFromCarEntrance - lenght / 2f < lanes[laneIndex].transform.localPosition.x)
            {
                return true;
            }
            else if ((distanceFromCarEntrance + lenght / 2f >= lanes[laneIndex].transform.localPosition.x + length))
            {
                return true;
            }

            return false;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < lanes.Length; i++)
            {
                Gizmos.DrawLine(lanes[i].transform.position, lanes[i].transform.position + Vector3.right * length);
            }
        }

        public void Disable()
        {
            EnemyPool.DisableEnemies(enemies);
            gameObject.SetActive(false);
        }

        public PickableObject GetPickableObject(int laneIndex, float horizontalPosition, float size)
        {
            for (int i = 0; i < pickableObjects.Length; i++)
            {
                if (//laneIndex == pickableObjects[i].GetLaneIndex() &&
                    pickableObjects[i].gameObject.activeSelf &&
                    (horizontalPosition - transform.localPosition.x - size / 2f) < (pickableObjects[i].transform.localPosition.x + pickableObjects[i].GetLength() / 2f)
                   //&& (horizontalPosition - transform.localPosition.x + size / 2f) > (pickableObjects[i].transform.localPosition.x - pickableObjects[i].GetLength() / 2f)
                   )
                {
                    return pickableObjects[i];
                }
            }

            return null;
        }
    }
}
