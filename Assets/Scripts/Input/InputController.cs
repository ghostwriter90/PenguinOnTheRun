using UnityEngine;

namespace PenguinOnTheRun.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] GameObject touchControlSystem;

        void Start()
        {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            touchControlSystem.SetActive(true);
#else
            touchControlSystem.SetActive(false);
#endif
        }
    }
}
