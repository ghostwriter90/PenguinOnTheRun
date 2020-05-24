using UnityEngine;


namespace PenguinOnTheRun.Gameplay
{
    [ExecuteInEditMode, SelectionBase]
    public class Obstacle : TrainObject
    {
        public enum ObstacleType { OBSTACLE, ENEMY, GRANDMA, DOG };

#pragma warning disable 649
        [SerializeField] protected ObstacleType obstacleType = ObstacleType.OBSTACLE;
        [SerializeField] protected int damageAmount = 0;
#pragma warning restore 649

        public ObstacleType GetObstacleType()
        {
            return obstacleType;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, new Vector3(length, 0.2f));
        }
    }
}
