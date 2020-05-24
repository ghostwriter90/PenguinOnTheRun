using PenguinOnTheRun.Audio;
using UnityEngine;


namespace PenguinOnTheRun.Gameplay
{
    [ExecuteInEditMode, SelectionBase]
    public class Grandma : Obstacle
    {
        public enum GrandmaType { LEFT, RIGHT };

#pragma warning disable 649
        [SerializeField] private GrandmaFlip grandmaFlip;
        [SerializeField] private GrandmaType grandmaType;
        [SerializeField] private float flipArea = 3f;
#pragma warning restore 649

        private void Update()
        {
            if (grandmaFlip.IsActive() && HasHitPlayer())
            {
                Player.Instance.Damage(damageAmount);
                EnemySounds.Instance.PlayGrandmaSound();
            }
        }

        private bool HasHitPlayer()
        {
            float playerPos = Player.Instance.GetDistanceForGrandmaOnly();
            float playerLength = Player.Instance.GetLength();

            if ((trainCar == Player.Instance.GetCurrentTrainCar())
                && (laneIndex == Player.Instance.GetLaneIndex())
                && Player.Instance.IsHealthy()
                && IsOverlappingRectangle(playerPos - playerLength / 2f, playerPos + playerLength / 2f, GetFlipAreaLeft(), GetFlipAreaRight()))
            {
                return true;
            }

            return false;
        }

        private bool IsOverlappingRectangle(float startA, float endA, float startB, float endB)
        {
            return (startA < endB) && (endA > startB);
        }

        private float GetFlipAreaLeft()
        {
            if (grandmaType == GrandmaType.LEFT)
            {
                return transform.localPosition.x - length / 2f - flipArea;
            }
            else
            {
                return transform.localPosition.x + length / 2f;
            }
        }

        private float GetFlipAreaRight()
        {
            if (grandmaType == GrandmaType.LEFT)
            {
                return transform.localPosition.x - length / 2f;
            }
            else
            {
                return transform.localPosition.x + length / 2f + flipArea;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
            Gizmos.DrawCube(transform.position, new Vector3(length, 0.2f));

            if ((trainCar != null) && grandmaFlip.IsActive())
            {
                Gizmos.color = Color.magenta;

                Vector3 laneStartPosition = trainCar.lanes[laneIndex].transform.position;
                Gizmos.DrawCube(laneStartPosition + Vector3.right * (GetFlipAreaLeft() + GetFlipAreaRight()) / 2f, new Vector3(GetFlipAreaRight() - GetFlipAreaLeft(), 0.2f));
            }
        }
    }
}
