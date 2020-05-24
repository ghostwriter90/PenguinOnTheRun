using System.Collections.Generic;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay.Obstacles
{
    public class EnemyPool : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private Enemy[] conductorSamples;
        [SerializeField] private int defaultConductorCount = 8;
#pragma warning restore 649

        private List<Enemy> enemyPool = new List<Enemy>();
        private HashSet<Enemy> activeConductors = new HashSet<Enemy>();
        private Transform poolContainer;

        private static EnemyPool Instance;

        private void Awake()
        {
            Instance = this;
            DisableSampleInstances();
            CreateInstances();
        }

        public static Enemy GetEnemy(Enemy.LaneSet laneSet)
        {
            if (Instance == null)
            {
                Debug.LogError("ConductorPool instance is missing");
                return null;
            }

            int ttl = 50;
            Enemy selectedEnemy = null;
            do
            {
                for (int i = 0; i < Instance.enemyPool.Count; i++)
                {
                    if (Instance.enemyPool[i].IsCorrespondingLane(laneSet))
                    {
                        selectedEnemy = Instance.enemyPool[i];
                        Instance.enemyPool.RemoveAt(i);
                        break;
                    }
                }

                if (selectedEnemy == null)
                {
                    Instance.CreateInstances();
                }

                if (--ttl == 0)
                {
                    Debug.LogError("Infinite loop detected");
                    break;
                }
            }
            while (selectedEnemy == null);

            selectedEnemy.gameObject.SetActive(true);
            Instance.activeConductors.Add(selectedEnemy);

            return selectedEnemy;
        }

        public static void DisableEnemies(List<Enemy> enemies)
        {
            foreach (Enemy enemy in enemies)
            {
                DisableEnemy(enemy);
            }
        }

        public static void DisableEnemy(Enemy enemy)
        {
            Instance.activeConductors.Remove(enemy);
            Instance.enemyPool.Add(enemy);
            enemy.gameObject.SetActive(false);
        }

        private void DisableSampleInstances()
        {
            foreach (Enemy conductor in conductorSamples)
            {
                conductor.gameObject.SetActive(false);
            }
        }

        private void CreateInstances()
        {
            if (poolContainer == null)
            {
                poolContainer = new GameObject("PoolContainer").transform;
                poolContainer.SetParent(transform);
            }

            for (int i = 0; i < defaultConductorCount; i++)
            {
                Enemy newConductor = Instantiate(conductorSamples[Random.Range(0, conductorSamples.Length)]);
                newConductor.transform.SetParent(poolContainer);
                newConductor.gameObject.SetActive(false);
                enemyPool.Add(newConductor);
            }
        }
    }
}
