using System;
using System.Collections.Generic;
using BasicTools.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace BasicTools.Input {

    public partial class TouchControlSystem : MonoBehaviour {

        const string circle1SpriteName = "Circle1_512";
        const string circle2SpriteName = "Circle2_512";
        const string arrowSpriteName = "Arrow_512";
        const string swipeSpriteName = "Swipe_512";

        public void FreshRendererTransforms (bool slowButFullFunctionality) {
            
            if(!screenPixelSize.HasValue){ return;}

            Vector2 fullSize = Vector2.one;
            screenRectTrans.SetPositionAndRotation (
                targetCamera.transform.position + (targetCamera.transform.forward * targetCamera.nearClipPlane * (1 + epsilon)),
                targetCamera.transform.rotation);

            if (cameraIsOrtographic) {
                float h = 2 * cameraOrthographicSize;
                fullSize = new Vector2 (h * (screenPixelSize.Value.x / screenPixelSize.Value.y), h);

            } else {
                float h = 2 * Mathf.Tan (targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * targetCamera.nearClipPlane;
                fullSize = new Vector2 (h * (screenPixelSize.Value.x / screenPixelSize.Value.y), h);
            }

            screenRectTrans.sizeDelta = fullSize;
            
            // Controls
            if (Controls != null) {

                for (int i = 0; i < Controls.Count; i++) {
                    TouchControl control = Controls[i];

                    if (control.rectTrans == null) {
                        continue;
                    }

                    if (!control.ControlEnabled) {
                        if (control.rectTrans.gameObject.activeInHierarchy) control.rectTrans.gameObject.SetActive (false);
                        continue;
                    }
                    if (!control.rectTrans.gameObject.activeInHierarchy) control.rectTrans.gameObject.SetActive (true);

                    if (Controls[i].rectTrans.parent != screenRectTrans)
                    {
                        Controls[i].rectTrans = null;
                    }
                    
                    
                    if (slowButFullFunctionality && control.rectTrans.name != control.Name) {
                        control.rectTrans.name = control.Name;
                    }
                    

                    Vector2 min = control.Area.GetScreen01MinPoint (screenPixelSize.Value);
                    Vector2 size = control.Area.GetSizeInScreen01 (screenPixelSize.Value);
                    Vector2 max = min + size;
                    
                    control.rectTrans.sizeDelta = Vector2.zero;
                    control.rectTrans.anchorMin = min;
                    control.rectTrans.anchorMax = max;

                    Vector2 areaSize = fullSize.MulipleAllAxis(size);

                    switch (control.type) {
                        case TouchControlType.ButtonField:
                            SetButtonFieldRectTransforms (control, control.rectTrans, areaSize, slowButFullFunctionality);
                            break;
                        case TouchControlType.AnalogField:
                            SetAnalogFieldRectTransforms (control, control.rectTrans, areaSize, slowButFullFunctionality);
                            break;
                        case TouchControlType.GestureField:
                            SetGestureFieldRectTransforms (control, control.rectTrans, areaSize, slowButFullFunctionality);
                            break;
                    }
                }
            }
        }

        public void CreateJoyStickRenderer(TouchControl control)
        {
            if (control.rectTrans == null)
            {
                GameObject controlChild = new GameObject(control.Name, typeof(RectTransform), typeof(JoypadRenderInfo));
                controlChild.transform.SetParent(screenRectTrans, false);
                control.rectTrans = controlChild.GetComponent<RectTransform>();
                control.JoyPadInfo = controlChild.GetComponent<JoypadRenderInfo>();
            }
            else if (control.JoyPadInfo == null) {
                JoypadRenderInfo jri = control.rectTrans.gameObject.AddComponent<JoypadRenderInfo>();
                control.JoyPadInfo = jri;
            }

            control.JoyPadInfo.Body = CreateNewRenderable(control.rectTrans, "Body", circle1SpriteName);
            control.JoyPadInfo.Finger = CreateNewRenderable(control.rectTrans, "Finger", circle2SpriteName);
        }

        public void CreateButtonRenderers(TouchControl control)
        {
            if (control.rectTrans == null)
            {
                GameObject controlChild = new GameObject(control.Name, typeof(RectTransform));
                controlChild.transform.SetParent(screenRectTrans, false);
                control.rectTrans = controlChild.GetComponent<RectTransform>();
            }

            for (int i = 0; i < control.Buttons.Length; i++)
            {
                TouchControl.TouchButton button = control.Buttons[i];
                
                if ( button.Renderable == null || button.Renderable.rect == null || button.Renderable.image == null)
                {
                    control.Buttons[i].Renderable = CreateNewRenderable(control.rectTrans, button.Name, circle2SpriteName); 
                }
            }
        }

        void SetButtonFieldRectTransforms (TouchControl control, RectTransform area, Vector2 areaSize, bool slowButFullFunctionality)
        {
            float areaDistanceToWorldSize = control.Area.AreaDistanceToWorldSize(screenPixelSize.Value, screenDiagonalToWorldSize, screenCmToWorldSize);
            float? sameSize = null;
            if (control.ButtonSettings.SameButtonSize) { sameSize = control.ButtonSettings.ButtonSize; }
            for (int i = 0; i < control.Buttons.Length; i++)
            {
                TouchControl.TouchButton button = control.Buttons[i];
                TouchControl.Renderable renderable = button.Renderable;
                if (renderable == null) { continue; }

                float sizeInDistance = (sameSize ?? button.ButtonSize) * areaDistanceToWorldSize;
                Vector2 centerPosInArea = button.AreaCoordiante;

                renderable.Set(centerPosInArea, new Vector2(sizeInDistance, sizeInDistance));
            }
        }

        void SetAnalogFieldRectTransforms(TouchControl control, RectTransform areaTrsnf, Vector2 areaSize, bool slowButFullFunctionality)
        {

            if (control.JoyPadInfo == null) { return; }

            float areaDistanceToWorldSize = control.Area.AreaDistanceToWorldSize(screenPixelSize.Value, screenDiagonalToWorldSize, screenCmToWorldSize);

            TouchControl.Renderable body = control.JoyPadInfo.Body;
            TouchControl.Renderable finger = control.JoyPadInfo.Finger;
            
            /*
            // delete reference if not child
            if (body != null && (body.rect == null || body.rect.parent != areaTrsnf)) { control.JoyPadInfo.AnalogBody = null; }
            if (finger != null && (finger.rect == null || finger.rect.parent != areaTrsnf)) { control.JoyPadInfo.Finger = null; }
            */
            
            // Get Real Time touch info
            RealtimeTouchAndControlInformation info = null;
            if (Application.isPlaying && touchAndControlPairs != null)
            {
                for (int i = 0; i < touchAndControlPairs.Count; i++)
                {
                    if (touchAndControlPairs[i].control == control)
                    {
                        info = touchAndControlPairs[i];
                        break;
                    }
                }
            }

            Vector2 centerPos =
                info != null && info.Analog_LastCenterPosition.HasValue ?
                Screen01PositionToAreaPosition(info.Analog_LastCenterPosition.Value, control.Area) :
                control.AnalogSettings.CenterPosition;

            Vector2 fingerPos = centerPos;
            if (info != null)
            {
                float overMaximumRate = info.Analog_DirectionalVectorInDistance.magnitude / control.AnalogSettings.maxDistance;
                Vector2 moveVector = info.Analog_DirectionalVectorInDistance;
                if (overMaximumRate > 1) { moveVector /= overMaximumRate; }

                moveVector = control.Area.DistanceVectorToScreen01Vector(moveVector, screenPixelSize.Value);
                moveVector = Screen01PositionToAreaPosition(moveVector, control.Area);

                fingerPos += moveVector;
            }

            float centerD = areaDistanceToWorldSize * control.AnalogSettings.maxDistance;
            Vector2 centerSize = new Vector2(centerD, centerD);
            
            /*
            for (int i = 0; i < 8; i++) {
                TouchControl.Renderable arrow = control.JoyPadInfo.DirectionButtons[i];
                Vector2 arrowVector = ((Direction8)i).ToVector().normalized * control.AnalogSettings.maxDistance;
                arrowVector = control.Area.DistanceVectorToScreen01Vector(arrowVector, screenPixelSize.Value);
                arrowVector = Screen01PositionToAreaPosition(arrowVector, control.Area);

                arrow.Set(centerPos + arrowVector, centerSize / 3f);
            }
            */

            body.Set(centerPos, centerSize);
            finger.Set(fingerPos, centerSize / 2f);
            
        }

        void SetGestureFieldRectTransforms (TouchControl control, RectTransform area, Vector2 areaSize, bool slowButFullFunctionality) {

        }

        public TouchControl.Renderable CreateNewRenderable(RectTransform areaTrsnf, string defaultName, string defaultSpriteName, float rotateDeg = 0) {
            GameObject centerGameObject = new GameObject(defaultName, typeof(RectTransform), typeof(Image));
            centerGameObject.transform.SetParent(areaTrsnf, false);
            TouchControl.Renderable renderable = new TouchControl.Renderable(
                centerGameObject.GetComponent<RectTransform>(),
                centerGameObject.GetComponent<Image>());
            //renderable.image.drawMode = SpriteDrawMode.Sliced;
            Sprite centerSprite = Resources.Load<Sprite>(defaultSpriteName) as Sprite;
            renderable.image.sprite = centerSprite;

            if (rotateDeg != 0)
            {
                renderable.rect.rotation = Quaternion.Euler(0, 0, rotateDeg);
            }

            return renderable;
        }
    }
}