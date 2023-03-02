﻿namespace BytingLib.UI
{
    public static class PaddingExtension
    {
        public static float WidthOr0(this Padding? padding)
        {
            if (padding == null)
                return 0f;
            return padding.Width;
        }
        public static float HeightOr0(this Padding? padding)
        {
            if (padding == null)
                return 0f;
            return padding.Height;
        }
    }

    public class Padding
    {
        public float Left { get; set; }
        public float Right { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }

        public Padding(float padding)
        {
            Left = Right = Top = Bottom = padding;
        }

        public Padding(float paddingX, float paddingY)
        {
            Left = Right = paddingX;
            Top = Bottom = paddingY;
        }

        public Padding(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public void RemoveFromRect(Rect rect)
        {
            rect.Width -= Right + Left;
            rect.Height -= Bottom + Top;
            rect.X += Left;
            rect.Y += Top;
        }

        public void AddToRect(Rect rect)
        {
            rect.Width += Right + Left;
            rect.Height += Bottom + Top;
            rect.X -= Left;
            rect.Y -= Top;
        }

        public Vector2 GetSize()
        {
            return new Vector2(Left + Right, Top + Bottom);
        }

        internal float Size(int d)
        {
            return d switch
            {
                0 => Width,
                1 => Height,
                _ => throw new DimensionDoesNotExistException(d)
            };
        }

        public float Width => Left + Right;
        public float Height => Top + Bottom;
    }
}
