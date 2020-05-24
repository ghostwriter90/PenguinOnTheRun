using UnityEngine;

namespace PenguinOnTheRun.Input
{
    public class InputController : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] GameObject touchControlSystem;
#pragma warning restore 649

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
