using UnityEngine;

namespace PenguinOnTheRun.Gameplay.Obstacles
{
    public class DogAnimationEventHandler : MonoBehaviour
    {
        public void ActivateDogRun()
        {
            Dog.Instance.DogRun();
        }
    }
}
