using PenguinOnTheRun.Gameplay.Obstacles;
using PenguinOnTheRun.Gameplay.Pickables;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    public class Lane : MonoBehaviour
    {
        private Obstacle[] obstacles;
        private PickableObject[] pickableObjects;
        private TrainCar trainCar;
        private int laneIndex;

        private const int luggageLaneIndex = 0;

        public void Initialize(TrainCar trainCar, int laneIndex)
        {
            this.laneIndex = laneIndex;
            this.trainCar = trainCar;

            ConductorSpawnPoint[] conductorSpawnPoints = GetComponentsInChildren<ConductorSpawnPoint>();
            foreach (ConductorSpawnPoint spawnPoint in conductorSpawnPoints)
            {
                spawnPoint.SetData(laneIndex, spawnPoint.transform.localPosition.x);
            }
        }

        public void UpdateObstacleList()
        {
            obstacles = GetComponentsInChildren<Obstacle>();

            foreach (Obstacle obstacle in obstacles)
            {
                obstacle.SetLaneIndex(laneIndex);
                obstacle.SetTrainCar(trainCar);
                obstacle.SetDistanceFromTrainCar(transform.localPosition.x);
            }
        }

        public void UpdatePickableObjectList()
        {
            pickableObjects = GetComponentsInChildren<PickableObject>();

            foreach (PickableObject pickableObject in pickableObjects)
            {
                pickableObject.SetLaneIndex(laneIndex);
                pickableObject.SetTrainCar(trainCar);
                pickableObject.SetDistanceFromTrainCar(transform.localPosition.x);
            }
        }

        public Obstacle GetObstacle(float horizontalPosition, float size)
        {
            foreach (Obstacle ob in obstacles)
            {
                if (IsPositionMatch(ob, horizontalPosition, size))
                {
                    return ob;
                }
            }

            float startA = transform.position.x + horizontalPosition - size / 2f;
            float endA = transform.position.x + horizontalPosition + size / 2f;
            float startB = Dog.PositionX - Dog.Instance.GetLength() / 2;
            float endB = Dog.PositionX + Dog.Instance.GetLength() / 2;

            if (Dog.IsActive && IsOverlappingRectangle(startA, endA, startB, endB))
            {
                return Dog.Instance;
            }

            return null;
        }

        bool IsOverlappingRectangle(float startA, float endA, float startB, float endB)
        {
            return (startA < endB) && (endA > startB);
        }

        public PickableObject GetPickableObject(float horizontalPosition, float size)
        {
            foreach (PickableObject po in pickableObjects)
            {
                if (po.gameObject.activeSelf && IsPositionMatch(po, horizontalPosition, size))
                {
                    return po;
                }
            }

            return null;
        }

        private bool IsPositionMatch(TrainObject trainObject, float horizontalPosition, float size)
        {
            return (horizontalPosition - transform.localPosition.x - size / 2f) < (trainObject.transform.localPosition.x + trainObject.GetLength() / 2f)
                    && (horizontalPosition - transform.localPosition.x + size / 2f) > (trainObject.transform.localPosition.x - trainObject.GetLength() / 2f);
        }

        public bool IsLuggageLane()
        {
            return laneIndex == luggageLaneIndex;
        }
    }
}
