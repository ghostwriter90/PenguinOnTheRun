using UnityEngine;

namespace BasicTools.Utility
{
    public enum Horizontal2 { Right = 1, Left = 5 }
    public enum Vertical2 { Down = 3, Up = 7 }
    public enum Direction4 { Right = 1, Down = 3, Left = 5, Up = 7 }
    public enum Direction6 { Right = 1, Down = 3, Left = 5, Up = 7, Forward = 8, Back = 9 }
    public enum Direction8 { UpRight = 0, Right = 1, DownRight = 2, Down = 3, DownLeft = 4, Left = 5, UpLeft = 6, Up = 7 }
    
    public static class DirectionExpanded
    {
        // Equals
        public static bool Equals (this Direction4 self, Direction6 dir)
        {
            return ((int)self == (int)dir);
        }

        public static bool Equals(this Direction4 self, Direction8 dir)
        {
            return ((int)self == (int)dir);
        }

        public static bool Equals(this Direction6 self, Direction4 dir)
        {
            return ((int)self == (int)dir);
        }

        public static bool Equals(this Direction6 self, Direction8 dir)
        {
            return ((int)self == (int)dir);
        }

        public static bool Equals(this Direction8 self, Direction4 dir)
        {
            return ((int)self == (int)dir);
        }

        public static bool Equals(this Direction8 self, Direction6 dir)
        {
            return ((int)self == (int)dir);
        }


        // Convert
        public static Direction8 ToDirection8(this Direction4 self)
        {
            return (Direction8)((int)self);
        }

        public static Direction6 ToDirection6(this Direction4 self)
        {
            return (Direction6)((int)self);
        }

        // To Vector Float
        public static Vector2 ToVector(this Direction4 dir)
        {
            switch (dir)
            {
                case Direction4.Up: return new Vector2(0, 1);
                case Direction4.Down: return new Vector2(0, -1);
                case Direction4.Right: return new Vector2(1, 0);
                case Direction4.Left: return new Vector2(-1, 0);
            }
            return new Vector2();
        }

        public static Vector3 ToVector(this Direction6 dir)
        {
            switch (dir)
            {
                case Direction6.Up: return new Vector3(0, 1, 0);
                case Direction6.Down: return new Vector3(0, -1, 0);
                case Direction6.Right: return new Vector3(1, 0, 0);
                case Direction6.Left: return new Vector3(-1, 0, 0);
                case Direction6.Forward: return new Vector3(0, 0, 1);
                case Direction6.Back: return new Vector3(0, 0, -0);
            }
            return new Vector3();
        }

        public static Vector2 ToVector(this Direction8 dir)
        {
            switch (dir)
            {
                case Direction8.Up: return new Vector2(0, 1);
                case Direction8.Down: return new Vector2(0, -1);
                case Direction8.Right: return new Vector2(1, 0);
                case Direction8.Left: return new Vector2(-1, 0);
                case Direction8.UpRight: return new Vector2(1, 1);
                case Direction8.DownRight: return new Vector2(1, -1);
                case Direction8.DownLeft: return new Vector2(-1, -1);
                case Direction8.UpLeft: return new Vector2(-1, 1);
            }
            return new Vector2();
        }

        // To Vector Int
        public static Vector2Int ToVectorInt(this Direction4 dir)
        {
            switch (dir)
            {
                case Direction4.Up: return new Vector2Int(0, 1);
                case Direction4.Down: return new Vector2Int(0, -1);
                case Direction4.Right: return new Vector2Int(1, 0);
                case Direction4.Left: return new Vector2Int(-1, 0);
            }
            return new Vector2Int();
        }

        public static Vector3Int ToVectorInt(this Direction6 dir)
        {
            switch (dir)
            {
                case Direction6.Up: return new Vector3Int(0, 1, 0);
                case Direction6.Down: return new Vector3Int(0, -1, 0);
                case Direction6.Right: return new Vector3Int(1, 0, 0);
                case Direction6.Left: return new Vector3Int(-1, 0, 0);
                case Direction6.Forward: return new Vector3Int(0, 0, 1);
                case Direction6.Back: return new Vector3Int(0, 0, -0);
            }
            return new Vector3Int();
        }

        public static Vector2Int ToVectorInt(this Direction8 dir)
        {
            switch (dir)
            {
                case Direction8.Up: return new Vector2Int(0, 1);
                case Direction8.Down: return new Vector2Int(0, -1);
                case Direction8.Right: return new Vector2Int(1, 0);
                case Direction8.Left: return new Vector2Int(-1, 0);
                case Direction8.UpRight: return new Vector2Int(1, 1);
                case Direction8.DownRight: return new Vector2Int(1, -1);
                case Direction8.DownLeft: return new Vector2Int(-1, -1);
                case Direction8.UpLeft: return new Vector2Int(-1, 1);
            }
            return new Vector2Int();
        }

        // To angle (Right = 0, Up = 90)
        public static int GetAngle(this Direction4 dir)
        {
            switch (dir)
            {
                case Direction4.Up: return 90;
                case Direction4.Down: return 270;
                case Direction4.Right: return 0;
                case Direction4.Left: return 180;
            }
            return 0;
        }

        public static int GetAngle(this Direction8 dir)
        {
            switch (dir)
            {
                case Direction8.Up: return 90;
                case Direction8.Down: return 270;
                case Direction8.Right: return 0;
                case Direction8.Left: return 180;
                case Direction8.UpRight: return 45;
                case Direction8.DownRight: return 315;
                case Direction8.DownLeft: return 225;
                case Direction8.UpLeft: return 135;
            }
            return 0;
        }

        // Opposit
        public static Horizontal2 Opposit(this Horizontal2 dir)
        {
            return dir == Horizontal2.Left ? Horizontal2.Right : Horizontal2.Left;
        }

        public static Vertical2 Opposit(this Vertical2 dir)
        {
            return dir == Vertical2.Down ? Vertical2.Up : Vertical2.Down;
        }

        public static Direction4 Opposit(this Direction4 dir)
        {
            return (Direction4)((((int)dir) + 4) % 8);
        }

        public static Direction6 Opposit(this Direction6 dir)
        {
            if ((int)dir < 8)
            {
                return (Direction6)((((int)dir) + 4) % 8);
            }
            else {
                if (dir == Direction6.Forward) { return Direction6.Back; }
                else { return Direction6.Forward; }
            }
        }

        public static Direction8 Opposit(this Direction8 dir)
        {
            return (Direction8)((((int)dir) + 4) % 8);
        }

        // Right
        public static Direction4 Right(this Direction4 dir, int step)
        {
            return (Direction4)((int)(dir + (step*2)) % 8);
        }

        public static Direction8 Right(this Direction8 dir, int step)
        {
            return (Direction8)((int)(dir + step) % 8);
        }

        // Left
        public static Direction4 Left(this Direction4 dir, int step)
        {
            int n = (int)(dir - (step*2)) % 8;
            if (n < 0) n = 8 + n;
            return (Direction4)n;
        }

        public static Direction8 Left(this Direction8 dir, int step)
        {
            int n = (int)(dir - step) % 8;
            if (n < 0) n = 8 + n;
            return (Direction8)n;
        }

        // Main of Diagonal 
        public static bool IsMainDirection(this Direction8 dir)
        {
            return dir == Direction8.Up || dir == Direction8.Down || dir == Direction8.Left || dir == Direction8.Right;
        }
        
        public static bool IsDiagonal(this Direction8 dir) {
            return !IsMainDirection(dir);
        }

        // Vertical or Horizontal
        public static bool IsVertical(this Direction8 dir)
        {
            return dir == Direction8.Up || dir == Direction8.Down;
        }

        public static bool IsHorizontal(this Direction8 dir)
        {
            return dir == Direction8.Left || dir == Direction8.Right;
        }

        public static bool IsVertical(this Direction4 dir)
        {
            return dir == Direction4.Up || dir == Direction4.Down;
        }

        public static bool IsHorizontal(this Direction4 dir)
        {
            return dir == Direction4.Left || dir == Direction4.Right;
        }

        // GetAxis
        public static Axis GetAxis(this Direction4 dir)
        {
            switch (dir)
            {
                case Direction4.Right:
                case Direction4.Left:
                    return Axis.X;
                case Direction4.Up:
                case Direction4.Down:
                    return Axis.Y;
            }
            return Axis.X;
        }

        public static Axis GetAxis(this Direction6 dir)
        {
            switch (dir)
            {
                case Direction6.Right:
                case Direction6.Left:
                    return Axis.X;
                case Direction6.Up:
                case Direction6.Down:
                    return Axis.Y;
                case Direction6.Forward:
                case Direction6.Back:
                    return Axis.Z;
            }
            return Axis.X;
        }

        public static Direction6 LeftHandedRoatate(this Direction6 self, Axis axis, int step)
        {
            if (self.GetAxis() == axis) return self;
            step = step % 4;
            if (step < 0) { step += 4; }

            if (step == 0) { return self; }
            else if (step == 2) { return self.Opposit(); }
            else if (step == 3) { self = self.Opposit(); }

            // One Step In Positive Direction:
            switch (axis)
            {
                case Axis.X:
                    switch (self)
                    {
                        case Direction6.Up: return Direction6.Forward;
                        case Direction6.Forward: return Direction6.Down;
                        case Direction6.Down: return Direction6.Back;
                        case Direction6.Back: return Direction6.Up;
                    }
                    break;
                case Axis.Y:
                    switch (self)
                    {
                        case Direction6.Right: return Direction6.Back;
                        case Direction6.Back: return Direction6.Left;
                        case Direction6.Left: return Direction6.Forward;
                        case Direction6.Forward: return Direction6.Right;
                    }
                    break;
                case Axis.Z:
                    switch (self)
                    {
                        case Direction6.Right: return Direction6.Up;
                        case Direction6.Up: return Direction6.Left;
                        case Direction6.Left: return Direction6.Down;
                        case Direction6.Down: return Direction6.Right;
                    }
                    break;
            }
            throw new System.Exception("Unreachable Code");
        }        
    }
}

