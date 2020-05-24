using PenguinOnTheRun.Audio;
using PenguinOnTheRun.UI;
using Rewired;
using System.Collections;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    public class Player : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private Animator animator;
        [SerializeField] private Transform container;
        [SerializeField] private AnimationCurve laneChangeTransition;
        [SerializeField] private float length = 1f;
        [SerializeField] private float maxSpeed = 1f;
        [SerializeField] private float acceleration = 100f;
        [SerializeField] private float deceleation = 100f;
        [SerializeField] private float minimumRunningSpeed = 0.1f;
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private int health = 3;
#pragma warning restore 649

        public static Player Instance { get; private set; }

        private readonly int changeLane = Animator.StringToHash("changeLane");
        private readonly int damageTrigger = Animator.StringToHash("damageTrigger");
        private readonly int deathTrigger = Animator.StringToHash("deathTrigger");
        private readonly int isRunning = Animator.StringToHash("isRunning");

        private readonly string changeLaneDown = "changeLaneDown";
        private readonly string changeLaneUp = "changeLaneUp";
        private readonly string horizontalMovement = "horizontalMovement";

        private readonly float distanceEpsilon = 0.1f;
        private readonly float speedEpsilon = 0.1f;

        private TrainCar currentTrainCar;
        private float distanceFromCarEntrance;
        private int currentLaneIndex;
        private bool isFacingRight = true;
        private bool isDuringLaneChange = false;
        private float currentSpeed = 0;
        private bool isDuringDamage = false;
        private Rewired.Player rewiredPlayer;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Player instance already exists!");
            }
            else
            {
                Instance = this;
                rewiredPlayer = ReInput.players.GetPlayer(0);
            }
        }

        private void Update()
        {
            if (health <= 0)
                return;

            if (!isDuringLaneChange && rewiredPlayer.GetButton(changeLaneUp) && (currentLaneIndex > 0)
                && IsNotHittingStaticObstacle(currentLaneIndex - 1, distanceFromCarEntrance, length)
                && !currentTrainCar.IsTrainCarEdge(currentLaneIndex - 1, distanceFromCarEntrance, length))
            {
                PenguinSounds.Instance.PlayUpSound();
                SetLaneIndex(currentLaneIndex - 1);
            }

            if (!isDuringLaneChange && rewiredPlayer.GetButton(changeLaneDown) && (currentLaneIndex < currentTrainCar.lanes.Length - 1)
                && IsNotHittingStaticObstacle(currentLaneIndex + 1, distanceFromCarEntrance, length)
                && !currentTrainCar.IsTrainCarEdge(currentLaneIndex + 1, distanceFromCarEntrance, length))
            {
                PenguinSounds.Instance.PlayDownSound();
                SetLaneIndex(currentLaneIndex + 1);
            }

            float horizontalMovenent = rewiredPlayer.GetAxis(horizontalMovement);
            HandleMovement(horizontalMovenent);
            CheckPickableObject();
        }

        private bool IsNotHittingStaticObstacle(int laneIndex, float distanceFromCarEntrance, float length)
        {
            Obstacle obstacle = currentTrainCar.lanes[laneIndex].GetObstacle(distanceFromCarEntrance, length);

            return obstacle == null || obstacle.GetObstacleType().Equals(Obstacle.ObstacleType.ENEMY);
        }

        private void CheckPickableObject()
        {
            PickableObject pickableObject = currentTrainCar.lanes[currentLaneIndex].GetPickableObject(distanceFromCarEntrance, length);

            if (pickableObject == null)
                return;

            pickableObject.gameObject.SetActive(false);

            if (health <= 0)
                return;

            PenguinSounds.Instance.PlayHappySound();

            if (pickableObject.GetObjectType() == PickableObject.ObjectType.FISH && health < maxHealth)
            {
                InfoCanvas.Instance.AddFish(health);
                health++;
            }
            else if (pickableObject.GetObjectType() == PickableObject.ObjectType.BONE)
            {
                Dog.Instance.AddBone();
            }
        }

        private void HandleMovement(float horizontalMovenent)
        {
            bool isLeftPressed = horizontalMovenent < -speedEpsilon;
            bool isRightPressed = horizontalMovenent > speedEpsilon;

            if (isLeftPressed)
            {
                currentSpeed = Mathf.Max(currentSpeed - acceleration * Time.deltaTime, -maxSpeed);

                if (isFacingRight)
                {
                    SetSpriteDirection(false);
                }
            }
            else if (isRightPressed)
            {
                currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

                if (!isFacingRight)
                {
                    SetSpriteDirection(true);
                }
            }
            else
            {
                float facingSign = isFacingRight ? -1f : 1f;
                currentSpeed += deceleation * Time.deltaTime * facingSign;

                if (isFacingRight && (currentSpeed < Mathf.Epsilon)
                    || !isFacingRight && (currentSpeed > -Mathf.Epsilon))
                {
                    currentSpeed = 0;
                }
            }

            float targetPosition = distanceFromCarEntrance + currentSpeed * Time.deltaTime;

            if (!isDuringLaneChange
                && IsNotHittingStaticObstacle(currentLaneIndex, targetPosition, length)
                && !currentTrainCar.IsTrainCarEdge(currentLaneIndex, targetPosition, length))
            {
                distanceFromCarEntrance = targetPosition;
            }
            else
            {
                currentSpeed = 0;
            }

            animator.SetBool(isRunning, (currentSpeed > minimumRunningSpeed) || (currentSpeed < -minimumRunningSpeed));

            if (currentTrainCar == null)
                return;

            if (currentLaneIndex != TrainCar.mainLaneIndex)
                return;

            if (currentTrainCar.IsTrainCarExit(currentLaneIndex, distanceFromCarEntrance, length / 2f + distanceEpsilon))
            {
                GoToNextTrainCar();
            }
        }

        private void GoToNextTrainCar()
        {
            SetTrainCar(GameManager.Instance.GetNextTrainCar());
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(currentTrainCar.transform.position.x + distanceFromCarEntrance,
                currentTrainCar.lanes[currentLaneIndex].transform.position.y);
        }

        private void SetLaneIndex(int laneIndex)
        {
            if (!isDuringLaneChange)
            {
                StartCoroutine(ChangeLane(laneIndex));
            }
        }

        private IEnumerator ChangeLane(int laneIndex)
        {
            isDuringLaneChange = true;
            animator.SetTrigger(changeLane);

            float deltaPosition = currentTrainCar.lanes[laneIndex].transform.localPosition.y
                    - currentTrainCar.lanes[currentLaneIndex].transform.localPosition.y;

            float time = 0;
            float duration = laneChangeTransition.keys[laneChangeTransition.keys.Length - 1].time;
            while (time < duration)
            {
                container.localPosition = Vector3.up * laneChangeTransition.Evaluate(time) * deltaPosition;

                time += Time.deltaTime;
                yield return null;
            }

            currentLaneIndex = laneIndex;
            container.localPosition = Vector3.zero;
            isDuringLaneChange = false;
        }

        public void Damage(int damageAmount)
        {
            if (health <= 0 || isDuringDamage)
                return;

            health -= damageAmount;

            if (health <= 0)
            {
                Death();
                return;
            }

            animator.SetTrigger(damageTrigger);
            
            StartCoroutine(WaitDamageInterval());
            InfoCanvas.Instance.RemoveFish(health);
            PenguinSounds.Instance.PlaySadSound();
        }

        public void Death()
        {
            animator.SetTrigger(deathTrigger);
            PenguinSounds.Instance.PlaySadSound();
            InfoCanvas.Instance.GameOver();
        }

        IEnumerator WaitDamageInterval()
        {
            isDuringDamage = true;

            yield return new WaitForSeconds(1f);

            isDuringDamage = false;
        }

        public void SetStartTrainCar(TrainCar trainCar)
        {
            SetTrainCar(trainCar);
            SetLaneIndex(TrainCar.mainLaneIndex);
        }

        public void SetTrainCar(TrainCar trainCar)
        {
            currentTrainCar = trainCar;
            distanceFromCarEntrance = length / 2f + Mathf.Epsilon;
        }

        public TrainCar GetCurrentTrainCar()
        {
            return currentTrainCar;
        }

        public float GetDistanceForGrandmaOnly()
        {
            return distanceFromCarEntrance - currentTrainCar.lanes[currentLaneIndex].transform.localPosition.x;
        }

        public int GetLaneIndex()
        {
            return currentLaneIndex;
        }

        public float GetLength()
        {
            return length;
        }

        public bool IsHealthy()
        {
            return health > 0 && !isDuringDamage;
        }

        private void SetSpriteDirection(bool isFacingRight)
        {
            if (this.isFacingRight != isFacingRight)
            {
                container.localScale = isFacingRight ? Vector3.one : new Vector3(-1f, 1f, 1f);
                this.isFacingRight = isFacingRight;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.75f, 0.95f, 0.15f);
            Gizmos.DrawCube(transform.position, new Vector3(length, 0.2f));
        }
    }
}