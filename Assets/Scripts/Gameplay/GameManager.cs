using PenguinOnTheRun.Settings;
using System.Collections.Generic;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private TrainCar startCar;
        [SerializeField] private TrainCar[] sampleCars;
        [SerializeField] private TrainCarSetting carSetting;
        [SerializeField] private int minimumCarInstanceCount = 10;

        [SerializeField] private List<TrainCar> availableCars = new List<TrainCar>();
        [SerializeField] private List<TrainCar> spawnedCars = new List<TrainCar>();

        private TrainCar lastSpawnedCar;

        private readonly int currentTrainIndex = 1;
        private readonly int maxTrainCount = 3;


        public static GameManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("GameManager instance already exists!");
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            CreateCarInstances();

            SpawnNextCar(true);
            SpawnNextCar(false);

            player.SetStartTrainCar(startCar);
        }

        public TrainCar GetStartTrainCar()
        {
            return startCar;
        }

        public TrainCar GetNextTrainCar()
        {
            return spawnedCars[currentTrainIndex];
        }

        void CreateCarInstances()
        {
            foreach (TrainCar sampleCar in sampleCars)
            {
                sampleCar.gameObject.SetActive(false);
            }

            for (int i = 0; i < Mathf.Max(minimumCarInstanceCount, sampleCars.Length); i++)
            {
                availableCars.Add(Instantiate(sampleCars[Random.Range(0, sampleCars.Length)]));
            }
        }

        public void SpawnNextCar(bool isStartCar)
        {
            TrainCar newCar;
            if (isStartCar)
            {
                newCar = startCar;
                newCar.Initialize(carSetting);
            }
            else
            {
                int randomIndex = Random.Range(0, availableCars.Count);
                newCar = availableCars[randomIndex];
                availableCars.RemoveAt(randomIndex);

                newCar.Initialize(carSetting);
            }

            if (lastSpawnedCar != null)
            {
                newCar.transform.position = lastSpawnedCar.transform.position + Vector3.right * lastSpawnedCar.length;
            }
            newCar.gameObject.SetActive(true);

            lastSpawnedCar = newCar;
            spawnedCars.Add(newCar);

            if (spawnedCars.Count > maxTrainCount)
            {
                TrainCar lastCar = spawnedCars[0];
                spawnedCars.RemoveAt(0);
                lastCar.Disable();

                if (lastCar != startCar)
                {
                    availableCars.Add(lastCar);
                }
            }
        }
    }
}
