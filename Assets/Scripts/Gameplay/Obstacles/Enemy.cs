using PenguinOnTheRun.Audio;
using UnityEngine;

namespace PenguinOnTheRun.Gameplay.Obstacles
{
    public class Enemy : Obstacle
    {
        public enum EnemyType { SMALL_CONDUCTOR, TALL_CONDUCTOR, CHICKEN };
        public enum LaneSet { ANY_LANE, LUGGAGE_LANE, PASSANGER_LANE };

#pragma warning disable 649
        [SerializeField] private float speed = 2;
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private LaneSet enabledLanes;
#pragma warning restore 649

        private readonly int penguinFound = Animator.StringToHash("penguinFound");

        private Animator animator;
        private TrainCar currentTrainCar = null;
        private float distanceFromCarEntrance;
        private int direction = 1;
        private int currentLaneIndex;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (!Application.isPlaying || (currentTrainCar == null))
                return;

            float targetPosition = distanceFromCarEntrance + speed * Time.deltaTime * direction;
            Obstacle obstacleAtTargetPosition = currentTrainCar.lanes[currentLaneIndex].GetObstacle(targetPosition, length);

            if (currentTrainCar.IsTrainCarEdge(currentLaneIndex, targetPosition, length)
                || obstacleAtTargetPosition?.GetObstacleType() == ObstacleType.OBSTACLE)
            {
                direction = -direction;
                transform.localScale = new Vector3(direction, 1f, 1f);
            }

            if (HasHitPlayer())
            {
                OnPlayerHit();
            }

            distanceFromCarEntrance = targetPosition;
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying || (currentTrainCar == null))
                return;

            transform.position = new Vector3(currentTrainCar.transform.position.x + distanceFromCarEntrance,
                currentTrainCar.lanes[currentLaneIndex].transform.position.y);
        }

        private void OnPlayerHit()
        {
            Player.Instance.Damage(damageAmount);
            EnemySounds.Instance.PlayEnemySound(enemyType);

            if (enemyType != EnemyType.CHICKEN)
            {
                animator.SetTrigger(penguinFound);
                speed = 0;
            }
        }

        private bool HasHitPlayer()
        {
            return currentLaneIndex == Player.Instance.GetLaneIndex() && Player.Instance.IsHealthy()
                   && (transform.position.x - length / 2f) < (Player.Instance.transform.position.x + Player.Instance.GetLength() / 2f)
                   && (transform.position.x + length / 2f) > (Player.Instance.transform.position.x - Player.Instance.GetLength() / 2f);
        }

        public void SetTrainCar(TrainCar trainCar, int laneIndex, float distanceFromCarEntrance)
        {
            currentTrainCar = trainCar;
            this.distanceFromCarEntrance = distanceFromCarEntrance;
            currentLaneIndex = laneIndex;
        }

        public bool IsCorrespondingLane(LaneSet laneSet)
        {
            return (enabledLanes == LaneSet.ANY_LANE) || (enabledLanes == laneSet);
        }
    }
}
