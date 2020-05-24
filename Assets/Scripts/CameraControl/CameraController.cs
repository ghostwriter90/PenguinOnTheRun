using UnityEngine;

namespace PenguinOnTheRun.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] bool enableUpdateInPlaymode = true;
        [SerializeField] Camera cameraReference;

        private float startOrtographicSize;

        void Start()
        {
            startOrtographicSize = cameraReference.orthographicSize;
            SetCameraSize();

            if (!enableUpdateInPlaymode)
            {
                enabled = false;
            }
        }

        void Update()
        {
            SetCameraSize();
        }

        void SetCameraSize()
        {
            float aspect = (float)Screen.width / (float)Screen.height;
            cameraReference.orthographicSize = startOrtographicSize / (aspect / (18f / 9f));
        }
    }
}
