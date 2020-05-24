using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    public class DogAnimationEventHandler : MonoBehaviour
    {
        public void ActivateDogRun()
        {
            Dog.Instance.DogRun();
        }
    }
}
