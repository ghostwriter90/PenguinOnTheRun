
namespace PenguinOnTheRun.Gameplay
{
    public class PickableObject : TrainObject
    {
        public enum ObjectType { FISH, BONE };

        protected ObjectType objectType;

        public ObjectType GetObjectType()
        {
            return objectType;
        }
    }
}
