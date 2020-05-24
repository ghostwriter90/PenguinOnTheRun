using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    public abstract class TrainObject : MonoBehaviour
    {
        [SerializeField] protected float length = 3f;

        protected int laneIndex = 1;
        protected TrainCar trainCar;
        protected float distanceFromTrainCar = 0;

        public void SetDistanceFromTrainCar(float laneDistance)
        {
            distanceFromTrainCar = transform.localPosition.x - laneDistance + length / 2;
        }

        public float GetLength()
        {
            return length;
        }

        public void SetLaneIndex(int ind)
        {
            laneIndex = ind;
        }

        public void SetTrainCar(TrainCar trainCar)
        {
            this.trainCar = trainCar;
        }
    }
}
