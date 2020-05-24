using UnityEngine;


namespace PenguinOnTheRun.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
    public class GameplaySortingLayerOrderSetter : MonoBehaviour
    {
        [SerializeField] private int offset = 0;
        [SerializeField] private bool enableUpdateInPlayMode = false;

        private SpriteRenderer spriteRenderer;

        private const string sortringLayerName = "GamePlay";
        private const float resolution = 10f;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = sortringLayerName;
        }

        void Update()
        {
            spriteRenderer.sortingOrder = Mathf.FloorToInt(-transform.position.y * resolution) + offset;
        }
    }
}
