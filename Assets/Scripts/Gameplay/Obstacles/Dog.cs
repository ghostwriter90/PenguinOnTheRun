using PenguinOnTheRun.UI;
using System.Collections;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay.Obstacles
{
    public class Dog : Obstacle
    {
#pragma warning disable 649
        [SerializeField] private int maxBoneCount = 3;
        [SerializeField] private int speed = 30;
        [SerializeField] private float offset = 0;
        [SerializeField] private float boneHintDuration = 1f;
        [SerializeField] private SpriteRenderer boneHint;
        [SerializeField] private Animator animator;
#pragma warning restore 649

        private TrainCar currentTrainCar;

        private int boneCount = 0;
        private bool isRunning = false;
        private bool isBoneHitActive = false;

        private readonly int startRun = Animator.StringToHash("dogStartRun");
        private readonly int stopRun = Animator.StringToHash("dogStop");
        private readonly int updatedBoneCount = Animator.StringToHash("boneCount");

        public static Dog Instance { get; private set; }

        public static bool IsActive => Instance.gameObject.activeInHierarchy;
        public static float PositionX => Instance.transform.position.x;

        private void Awake()
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

        private void Start()
        {
            if (!Application.isPlaying)
                return;

            currentTrainCar = GameManager.Instance.GetStartTrainCar();
        }

        private void Update()
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

        public void BoneHint()
        {
            if (isBoneHitActive)
                return;

            StartCoroutine(HintFadeInFadeOut());
        }

        private IEnumerator HintFadeInFadeOut()
        {
            isBoneHitActive = true;
            float startTime = Time.time;
            Color originalColor = boneHint.color;

            while (Time.time - startTime < boneHintDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, (Time.time - startTime) / boneHintDuration);
                originalColor.a = alpha;
                boneHint.color = originalColor;

                yield return null;
            }

            yield return new WaitForSeconds(boneHintDuration);

            startTime = Time.time;

            while (Time.time - startTime < boneHintDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, (Time.time - startTime) / boneHintDuration);
                originalColor.a = alpha;
                boneHint.color = originalColor;

                yield return null;
            }

            originalColor.a = 0f;
            boneHint.color = originalColor;
            isBoneHitActive = false;
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
            InfoCanvas.Instance.ResetBones();
        }
    }
}
