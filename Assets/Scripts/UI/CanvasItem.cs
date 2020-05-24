using UnityEngine;

namespace PenguinOnTheRun.UI
{
    public class CanvasItem : MonoBehaviour
    {
        public enum ItemType { FISH, BONE };
        public enum ExistType { FULL, MISSING };

        [SerializeField] ItemType itemType;
        [SerializeField] ExistType existType;
        [SerializeField] int index;

        public ItemType GetItemType()
        {
            return itemType;
        }

        public ExistType GetExistType()
        {
            return existType;
        }

        public int getIndex()
        {
            return index;
        }
    }
}
