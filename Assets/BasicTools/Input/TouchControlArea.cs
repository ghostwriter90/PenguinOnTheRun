using System;
using BasicTools.Utility;
using UnityEngine;

namespace BasicTools.Input {
    public partial class TouchControl {

        public enum AdreaRelativeDistanceType { AreaDiagonal, AreaHeight, AreaWidth}

        [Serializable]
        public struct TouchArea {

            public Direction8 Dock;
            public bool KeepFullyInScreen;
            public bool HideLowerControlAreas;
            public float WidthInScreenRatio;
            public float WidthInCm;

            public float HeightInScreenRatio;
            public float HeightInCm;
            public float OffsetInScreenRatio;
            public float OffsetInCm;

            [Range(0, 1)] public float DistanceDefinitonCmOrRelative;
            public AdreaRelativeDistanceType AdreaDistanceType;

            public bool IsInside (Vector2 screen01Point, Vector2 screenPixel) {
                Vector2 min = GetScreen01MinPoint(screenPixel.x, screenPixel.y);
                Vector2 max = min + GetSizeInScreen01 (screenPixel.x, screenPixel.y);
                return
                screen01Point.x >= min.x && screen01Point.x <= max.x &&
                    screen01Point.y >= min.y && screen01Point.y <= max.y;
            }
            
            public Vector2 GetScreen01MinPoint (Vector2 scrrenPixelSize) {
                return GetScreen01MinPoint (scrrenPixelSize.x, scrrenPixelSize.y);
            }

            public Vector2 GetScreen01MinPoint (float screenW, float screenH) {
                float x, y;
                Vector2 s = GetSizeInScreen01 (screenW, screenH);
                switch (Dock) {
                    case Direction8.Down:
                        x = 0.5f - (s.x / 2f);
                        y = 0;
                        break;
                    case Direction8.Left:
                        x = 0;
                        y = 0.5f - (s.y / 2f);
                        break;
                    case Direction8.Right:
                        x = 1 - s.x;
                        y = 0.5f - (s.y / 2f);
                        break;
                    case Direction8.Up:
                        x = 0.5f - (s.x / 2f);
                        y = 1 - s.y;
                        break;
                    case Direction8.DownRight:
                        x = 1 - s.x;
                        y = 0;
                        break;
                    case Direction8.UpLeft:
                        x = 0;
                        y = 1 - s.y;
                        break;
                    case Direction8.UpRight:
                        x = 1 - s.x;
                        y = 1 - s.y;
                        break;
                    default: // DownLeft
                        x = 0;
                        y = 0;
                        break;

                }

                if (Dock.IsMainDirection ()) {
                    float offset = OffsetInScreenRatio;
                    if (Dock.IsVertical ()) {
                        offset += (OffsetInCm * MathPlus.CmToInch) * TouchControlSystem.DPI / screenW;
                        x += offset;
                    } else {
                        offset += (OffsetInCm * MathPlus.CmToInch) * TouchControlSystem.DPI / screenH;
                        y += offset;
                    }
                }

                if (KeepFullyInScreen) {
                    if (Dock.IsVertical())
                    {
                        if (x < 0) { x = 0; }
                        if (x + s.x > 1) { x = 1 - s.x; }
                    }
                    else if(Dock.IsHorizontal())
                    {
                        if (y + s.y > 1) { y = 1 - s.y ; }
                        if (y < 0) { y = 0; }
                    }
                }
                
                return new Vector2 (x, y);
            }

            public Vector2 GetSizeInScreen01 (Vector2 scrrenPixelSize) {
                return GetSizeInScreen01 (scrrenPixelSize.x, scrrenPixelSize.y);
            }

            public Vector2 GetSizeInScreen01 (float screenW, float screenH) {
                float cmToW = (WidthInCm * MathPlus.CmToInch) * TouchControlSystem.DPI / screenW;
                float cmToH = (HeightInCm * MathPlus.CmToInch) * TouchControlSystem.DPI / screenH;
                Vector2 sizeInScreen01 = new Vector2 (WidthInScreenRatio + cmToW, HeightInScreenRatio + cmToH);

                if (KeepFullyInScreen) {
                    sizeInScreen01 = MathPlus.Clamp01 (sizeInScreen01);
                }

                return sizeInScreen01;
            }

            public float AreaDistanceToWorldSize(Vector2 screenSizeInPixel, float screenDiagonalToWorldSize, float screenCmToWorldSize)
            {
                Vector2 areaSizeInScreen01 = GetSizeInScreen01(screenSizeInPixel.x, screenSizeInPixel.y);

                float screenDiagonalToAreaSize = 1;
                switch (AdreaDistanceType)
                {
                    case AdreaRelativeDistanceType.AreaDiagonal:
                        screenDiagonalToAreaSize =
                        new Vector2(screenSizeInPixel.x * areaSizeInScreen01.x, screenSizeInPixel.y * areaSizeInScreen01.y).magnitude /
                        screenSizeInPixel.magnitude;
                        break;
                    case AdreaRelativeDistanceType.AreaHeight:
                        screenDiagonalToAreaSize = (screenSizeInPixel.y * areaSizeInScreen01.y) / screenSizeInPixel.magnitude;
                        break;
                    case AdreaRelativeDistanceType.AreaWidth:
                        screenDiagonalToAreaSize = (screenSizeInPixel.x * areaSizeInScreen01.x) / screenSizeInPixel.magnitude;
                        break;
                }

                float areaSizeToWorldSize = screenDiagonalToAreaSize * screenDiagonalToWorldSize;
                float worldDistance = 2 * ((screenCmToWorldSize * (1- DistanceDefinitonCmOrRelative)) + (areaSizeToWorldSize * DistanceDefinitonCmOrRelative));
                               
                return worldDistance ;
            }



            public Vector2 Screen01VectorToDistanceVector(Vector2 screen01Vector, Vector2 screenSizeInPixel)
            {
                //CM
                float xCm, yCm;
                float pixelToCm =  MathPlus.InchToCm / TouchControlSystem.DPI;
                float ScreenWInCm = screenSizeInPixel.x * pixelToCm;
                float ScreenHInCm = screenSizeInPixel.y * pixelToCm;
                xCm = screen01Vector.x * ScreenWInCm;
                yCm = screen01Vector.y * ScreenHInCm;

                // Relative
                Vector2 relativeScreenSize = GetScreenSizeInRelativeAreaSize(screenSizeInPixel);
                float xRel = screen01Vector.x * relativeScreenSize.x;
                float yRel = screen01Vector.y * relativeScreenSize.y;


                // Composit
                float cmMultiplyer = (1 - DistanceDefinitonCmOrRelative);
                float cx = screen01Vector.x == 0 ? 0 : 1 / ((cmMultiplyer / xCm) + (DistanceDefinitonCmOrRelative / xRel));
                float cy = screen01Vector.y == 0 ? 0 : 1 / ((cmMultiplyer / yCm) + (DistanceDefinitonCmOrRelative / yRel));
                
                return new Vector2(cx, cy);
            }

            public Vector2 DistanceVectorToScreen01Vector(Vector2 distanceVector, Vector2 screenSizeInPixel)
            {
                //CM
                float xCm,yCm;
                float pixelToCm =  MathPlus.InchToCm / TouchControlSystem.DPI;
                                
                xCm = distanceVector.x / (screenSizeInPixel.x * pixelToCm);
                yCm = distanceVector.y / (screenSizeInPixel.y * pixelToCm);

                // Relative
                Vector2 relativeScreenSize = GetScreenSizeInRelativeAreaSize(screenSizeInPixel);
                float xRel = distanceVector.x / relativeScreenSize.x;
                float yRel = distanceVector.y / relativeScreenSize.y;


                // Composit
                float cmMultiplyer = (1 - DistanceDefinitonCmOrRelative);
                float relativeMultiplyer = ( DistanceDefinitonCmOrRelative );
                Vector2 result = new Vector2(
                    (xCm * cmMultiplyer) + (xRel * relativeMultiplyer),
                    (yCm * cmMultiplyer) + (yRel * relativeMultiplyer));
                
                return result;
            }

            public Vector2 GetScreenSizeInRelativeAreaSize(Vector2 screenSizeInPixel)
            {

                float screenHInRel = 0, screenWInRel = 0, screenAspect;
                Vector2 areaSize = GetSizeInScreen01(screenSizeInPixel.x, screenSizeInPixel.y);
                switch (AdreaDistanceType)
                {
                    case AdreaRelativeDistanceType.AreaDiagonal:
                        float areaDiagonalInPixel = Mathf.Sqrt(Mathf.Pow(areaSize.y * screenSizeInPixel.y, 2) + Mathf.Pow(areaSize.x * screenSizeInPixel.x, 2));
                        screenHInRel = screenSizeInPixel.y / areaDiagonalInPixel;
                        screenWInRel = screenSizeInPixel.x / areaDiagonalInPixel;
                        break;
                    case AdreaRelativeDistanceType.AreaHeight:
                        screenAspect = screenSizeInPixel.y / screenSizeInPixel.x;
                        screenHInRel = 1 / areaSize.y;
                        screenWInRel = screenHInRel / screenAspect;
                        break;
                    case AdreaRelativeDistanceType.AreaWidth:
                        screenAspect = screenSizeInPixel.y / screenSizeInPixel.x;
                        screenWInRel = 1 / areaSize.x;
                        screenHInRel = screenWInRel * screenAspect;
                        break;
                }

                return new Vector2(screenWInRel, screenHInRel);
            }
        }
    }
}