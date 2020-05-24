using UnityEngine;

namespace BasicTools.Utility
{
    public class ObjectWithSpeed : MonoBehaviour
    {
        Vector3 lastPosition;
        Vector3 moveVector = Vector3.zero;

        // Use this for initialization
        void Awake()
        {
            lastPosition = transform.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 newPos = transform.position;
            moveVector = (newPos - lastPosition);
            lastPosition = newPos;
        }

        public Vector3 MoveSinceLastFrame { get { return moveVector; } }
        public bool CanMove { get { return true; } }
    }
}