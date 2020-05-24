using PenguinOnTheRun.UI;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    public class Dog : Obstacle
    {
        [SerializeField] private int maxBoneCount = 3;
        [SerializeField] private float offset = 0;
        [SerializeField] private int speed = 30;
        [SerializeField] private Animator animator;

        private TrainCar currentTrainCar;

        private int boneCount = 0;
        private bool isRunning = false;

        private readonly int startRun = Animator.StringToHash("dogStartRun");
        private readonly int stopRun = Animator.StringToHash("dogStop");
        private readonly int updatedBoneCount = Animator.StringToHash("boneCount");

        public static Dog Instance { get; private set; }

        public static bool IsActive => Instance.gameObject.activeInHierarchy;
        public static float PositionX => Instance.transform.position.x;

        void Awake()
        {
            if (!Application.isPlaying)
                return;

            if (Instance != null)
            {
                Debug.LogWarning("Dog instance already exists!");
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            currentTrainCar = GameManager.Instance.GetStartTrainCar();
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;

            if (!isRunning)
                return;

            MoveUntilTrainCarEnd();
        }

        public void AddBone()
        {
            boneCount++;
            animator.SetInteger(updatedBoneCount, boneCount);
            InfoCanvas.Instance.AddBone(boneCount - 1);
        }

        public void DogRun()
        {
            if (isRunning)
                return;

            isRunning = true;
            animator.SetTrigger(startRun);
            GoToNextTrainCar();
        }

        private void MoveUntilTrainCarEnd()
        {
            Vector3 targetPosition = transform.position + Vector3.right * speed * Time.deltaTime;

            if (targetPosition.x >= currentTrainCar.transform.position.x + currentTrainCar.length - offset)
            {
                Vector3 tmp = targetPosition;
                tmp.x = currentTrainCar.transform.position.x + currentTrainCar.length - offset;
                targetPosition = tmp;

                DogStop();
            }

            transform.position = targetPosition;
        }

        private void DogStop()
        {
            if (!isRunning)
                return;

            isRunning = false;
            animator.SetTrigger(stopRun);
            animator.SetInteger(updatedBoneCount, 0);
        }

        private void GoToNextTrainCar()
        {
            GameManager.Instance.SpawnNextCar(false);
            currentTrainCar = GameManager.Instance.GetNextTrainCar();
            ResetBoneCount();
        }

        private void ResetBoneCount()
        {
            boneCount = 0;
            InfoCanvas.Instance.ZeroBones();
        }
    }
}
