using UnityEngine;

namespace PenguinOnTheRun.Gameplay
{
    public class GrandmaFlip : MonoBehaviour
    {
        private bool active = false;

        public void SetActive()
        {
            active = true;
        }

        public void SetInactive()
        {
            active = false;
        }

        public bool IsActive()
        {
            return active;
        }
    }
}
