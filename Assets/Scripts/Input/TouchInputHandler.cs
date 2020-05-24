using BasicTools.Utility;
using BasicTools.Input;
using Rewired;
using UnityEngine;

namespace PenguinOnTheRun.Input
{
    public class TouchInputHandler : MonoBehaviour, IGameEventListener<TouchControlAnalogInfo>, IGameEventListener<TouchControlSwipeInfo>
    {
        private const string changeLaneDownTouch = "changeLaneDown_touch";
        private const string changeLaneUpTouch = "changeLaneUp_touch";
        private const string horizontalMovementTouch = "horizontalMovement_touch";

        private int updatesAfterPreviousSwipe = 0;
        private CustomController customControllerTouch;

        void Awake()
        {
            Player player = ReInput.players.GetPlayer(0);
            customControllerTouch = player.controllers.GetControllerWithTag<CustomController>("touch");
        }

        public void OnGameEvent(TouchControlAnalogInfo eventInfo)
        {
            float direction = eventInfo.direction == Direction8.Right ? 1f : -1f;

            customControllerTouch.SetAxisValue(horizontalMovementTouch,
                Mathf.Clamp01(eventInfo.interpolatedDirectionalValue) * direction);

            if (eventInfo.release)
            {
                customControllerTouch.SetAxisValue(horizontalMovementTouch, 0);
            }
        }

        public void OnGameEvent(TouchControlSwipeInfo eventInfo)
        {
            if (eventInfo.swipeIndex == 0)
            {
                customControllerTouch.SetButtonValue(changeLaneUpTouch, true);
                updatesAfterPreviousSwipe = 0;
            }
            else if (eventInfo.swipeIndex == 1)
            {
                customControllerTouch.SetButtonValue(changeLaneDownTouch, true);
                updatesAfterPreviousSwipe = 0;
            }
        }

        void Update()
        {
            if (updatesAfterPreviousSwipe == 1)
            {
                customControllerTouch.SetButtonValue(changeLaneDownTouch, false);
                customControllerTouch.SetButtonValue(changeLaneUpTouch, false);
            }
            updatesAfterPreviousSwipe++;
        }

        void OnEnable()
        {
            GameEventManager.AddListener<TouchControlAnalogInfo>(this);
            GameEventManager.AddListener<TouchControlSwipeInfo>(this);
        }

        void OnDisable()
        {
            GameEventManager.RemoveListener<TouchControlAnalogInfo>(this);
            GameEventManager.RemoveListener<TouchControlSwipeInfo>(this);
        }
    }
}
