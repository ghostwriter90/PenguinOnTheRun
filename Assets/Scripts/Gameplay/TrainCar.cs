using PenguinOnTheRun.Gameplay.Obstacles;
using PenguinOnTheRun.Gameplay.Pickables;
using PenguinOnTheRun.Gameplay.SpawnPoints;
using PenguinOnTheRun.Settings;
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


        public void Initialize(TrainCarSetting carSetting)
        {
            enemies = new List<Enemy>();

            for (int i = 0; i < lanes.Length; i++)
            {
                lanes[i].Initialize(this, i);
            }

            SpawnConductors(carSetting.ConductorCount);
            SpawnBones(carSetting.BoneCount);
            SpawnFishes(carSetting.FishCount);
            InitializeChairs(carSetting.GrandmaCount);

            for (int i = 0; i < lanes.Length; i++)
            {
                lanes[i].UpdateObstacleList();
                lanes[i].UpdatePickableObjectList();
            }
        }

        private void InitializeChairs(int grandmaCount)
        {
            ChairSpawnPoint[] chairs = GetComponentsInChildren<ChairSpawnPoint>();
            HashSet<int> selectedIndexSet = new HashSet<int>();

            for (int i = 0; i < Mathf.Min(grandmaCount, chairs.Length); i++)
            {
                int selectedIndex;
                do
                {
                    selectedIndex = Random.Range(0, chairs.Length);
                }
                while (selectedIndexSet.Contains(selectedIndex));
                selectedIndexSet.Add(selectedIndex);
            }

            for (int i = 0; i < chairs.Length; i++)
            {
                chairs[i].Initialize(selectedIndexSet.Contains(i));
            }
        }

        private void SpawnConductors(int conductorCount)
        {
            ConductorSpawnPoint[] conductorSpawnPoints = GetComponentsInChildren<ConductorSpawnPoint>();
            HashSet<int> selectedIndexList = new HashSet<int>();

            for (int i = 0; i < Mathf.Min(conductorCount, conductorSpawnPoints.Length); i++)
            {
                int selectedIndex;
                do
                {
                    selectedIndex = Random.Range(0, conductorSpawnPoints.Length);
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

        private void SpawnBones(int boneCount)
        {
            BoneSpawnPoint[] boneSpawnPoints = GetComponentsInChildren<BoneSpawnPoint>();
            SpawnPickableObjects(boneSpawnPoints, boneCount);
        }

        private void SpawnFishes(int fishCount)
        {
            FishSpawnPoint[] fishSpawnPoints = GetComponentsInChildren<FishSpawnPoint>();
            SpawnPickableObjects(fishSpawnPoints, fishCount);
        }

        private void SpawnPickableObjects(PickableSpawnPoint[] spawnPoints, int count)
        {
            HashSet<int> selectedIndexList = new HashSet<int>();

            for (int i = 0; i < Mathf.Min(count, spawnPoints.Length); i++)
            {
                int selectedIndex;
                do
                {
                    selectedIndex = Random.Range(0, spawnPoints.Length);
                }
                while (selectedIndexList.Contains(selectedIndex));

                selectedIndexList.Add(selectedIndex);
                PickableSpawnPoint spawnPoint = spawnPoints[selectedIndex];
                spawnPoint.SpawnInstance();
            }
        }

        public bool IsTrainCarExit(int laneIndex, float distanceFromCarEntrance, float acceptableNoise)
        {
            if ((distanceFromCarEntrance >= lanes[laneIndex].transform.localPosition.x + length - acceptableNoise)
                && (laneIndex == mainLaneIndex))
            {
                return true;
            }

            return false;
        }

        public bool IsTrainCarEdge(int laneIndex, float distanceFromCarEntrance, float length)
        {
            if (distanceFromCarEntrance - length / 2f < lanes[laneIndex].transform.localPosition.x)
            {
                return true;
            }
            else if (distanceFromCarEntrance + length / 2f >= lanes[laneIndex].transform.localPosition.x + this.length)
            {
                return true;
            }

            return false;
        }

        public void Disable()
        {
            EnemyPool.DisableEnemies(enemies);
            gameObject.SetActive(false);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < lanes.Length; i++)
            {
                Gizmos.DrawLine(lanes[i].transform.position, lanes[i].transform.position + Vector3.right * length);
            }
        }
    }
}
