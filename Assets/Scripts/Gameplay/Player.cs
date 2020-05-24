using PenguinOnTheRun.Audio;
using PenguinOnTheRun.UI;
using Rewired;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PenguinOnTheRun.Gameplay
{
    public class Player : MonoBehaviour
    {
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

        public static Player Instance { get; private set; }

        private readonly int changeLane = Animator.StringToHash("changeLane");
        private readonly int damageTrigger = Animator.StringToHash("damageTrigger");
        private readonly int deathTrigger = Animator.StringToHash("deathTrigger");

        private readonly string changeLaneDown = "changeLaneDown";
        private readonly string changeLaneUp = "changeLaneUp";
        private readonly string horizontalMovement = "horizontalMovement";

        private TrainCar currentTrainCar;
        private float distanceFromCarEntrance;
        private int currentLaneIndex;
        private bool isFacingRight = true;
        private bool isDuringLaneChange = false;
        private float currentSpeed = 0;
        private bool isDamaging = false;
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
            HandleMovement((horizontalMovenent < -0.1f), (horizontalMovenent > 0.1f));

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

            PenguinSounds.Instance.PlayHappySound();

            if (pickableObject.GetObjectType() == PickableObject.ObjectType.FISH && health < 3)
            {
                InfoCanvas.Instance.AddFish(health);
                health++;
            }
            else if (pickableObject.GetObjectType() == PickableObject.ObjectType.BONE)
            {
                Dog.Instance.AddBone();
            }

            pickableObject.gameObject.SetActive(false);
        }

        private void HandleMovement(bool isLeftPressed, bool isRightPressed)
        {
            // TODO: change to:     void HandleMovement(float horizontalMovement)
            if (isLeftPressed || isRightPressed)
            {
                if (isLeftPressed)
                {
                    currentSpeed = Mathf.Max(currentSpeed - acceleration * Time.deltaTime, -maxSpeed);

                    if (isFacingRight)
                    {
                        SetSpriteDirection(false);
                    }
                }

                if (isRightPressed)
                {
                    currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

                    if (!isFacingRight)
                    {
                        SetSpriteDirection(true);
                    }
                }
            }
            else
            {
                if (isFacingRight)
                {
                    currentSpeed -= deceleation * Time.deltaTime;
                }
                else
                {
                    currentSpeed += deceleation * Time.deltaTime;
                }

                if (isFacingRight && (currentSpeed < Mathf.Epsilon))
                {
                    currentSpeed = 0;
                }
                else if (!isFacingRight && (currentSpeed > -Mathf.Epsilon))
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

            animator.SetBool("isRunning", (currentSpeed > minimumRunningSpeed) || (currentSpeed < -minimumRunningSpeed));

            if ((currentTrainCar != null)
                && currentTrainCar.IsTrainCarExit(currentLaneIndex, distanceFromCarEntrance, length / 2f + 0.1f)
                && (currentLaneIndex == TrainCar.mainLaneIndex))
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
            if (health <= 0 || isDamaging)
                return;

            health -= damageAmount;

            if (health <= 0)
            {
                Death();
                return;
            }

            animator.SetTrigger(damageTrigger);
            PenguinSounds.Instance.PlaySadSound();
            isDamaging = true;
            StartCoroutine(WaitDamageInterval());
            InfoCanvas.Instance.DamageFish(health);
        }

        public void Death()
        {
            if (health <= 0)
                return;

            animator.SetTrigger(deathTrigger);
            PenguinSounds.Instance.PlaySadSound();
            InfoCanvas.Instance.Die();

            SceneManager.LoadScene(0);
        }

        IEnumerator WaitDamageInterval()
        {
            yield return new WaitForSeconds(1f);

            isDamaging = false;
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
            return health == maxHealth;
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