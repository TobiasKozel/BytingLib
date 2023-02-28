﻿
namespace BytingLib.UI
{
	public partial class PanelStack
	{

        private void UpdateTreeVertical(Vector2 pos, Vector2 contentSize, bool anyUnknownHeight, Rect rect)
        {
            float nullHeight = 0f;
            float maxWidthPercentage = Children.Max(f => -MathF.Min(0, f.Width));
            if (anyUnknownHeight)
            {
                float autoHeightSum = Children.Sum(f => -MathF.Min(0, f.Height));
                float fixedHeight = GetFixedHeight();
                float remainingHeight = contentSize.Y - fixedHeight;
                nullHeight = remainingHeight / autoHeightSum;
            }

            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                float height = c.GetHeightTopToBottom();
				float width = rect.Width;
                Rect r = GetChildRect(new Rect(pos.X, pos.Y, width, height), c);
                c.UpdateTree(r);
				pos.Y += height + Gap;
            }
        }

        private float GetFixedHeight()
        {
            return Children.Sum(f => MathF.Max(0f, f.GetHeightTopToBottom())) + Gap * (Children.Count - 1);
        }

        private Vector2 GetContentSizeVertical(out bool anyUnknownHeight)
        {
            float width = Children.Count == 0 ? 0 : Children.Max(f => f.GetWidthTopToBottom());
            float height;

            if (Children.Any(f => f.Height < 0))
            {
                // take 100% height
                height = GetHeightTopToBottom();
                anyUnknownHeight = true;
            }
            else
            {
                height = GetFixedHeight();
                anyUnknownHeight = false;
            }

            return new Vector2(width, height);
        }

        private void UpdateTreeHorizontal(Vector2 pos, Vector2 contentSize, bool anyUnknownWidth, Rect rect)
        {
            float nullWidth = 0f;
            float maxHeightPercentage = Children.Max(f => -MathF.Min(0, f.Height));
            if (anyUnknownWidth)
            {
                float autoWidthSum = Children.Sum(f => -MathF.Min(0, f.Width));
                float fixedWidth = GetFixedWidth();
                float remainingWidth = contentSize.X - fixedWidth;
                nullWidth = remainingWidth / autoWidthSum;
            }

            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                float width = c.GetWidthTopToBottom();
				float height = rect.Height;
                Rect r = GetChildRect(new Rect(pos.X, pos.Y, width, height), c);
                c.UpdateTree(r);
				pos.X += width + Gap;
            }
        }

        private float GetFixedWidth()
        {
            return Children.Sum(f => MathF.Max(0f, f.GetWidthTopToBottom())) + Gap * (Children.Count - 1);
        }

        private Vector2 GetContentSizeHorizontal(out bool anyUnknownWidth)
        {
            float height = Children.Count == 0 ? 0 : Children.Max(f => f.GetHeightTopToBottom());
            float width;

            if (Children.Any(f => f.Width < 0))
            {
                // take 100% width
                width = GetWidthTopToBottom();
                anyUnknownWidth = true;
            }
            else
            {
                width = GetFixedWidth();
                anyUnknownWidth = false;
            }

            return new Vector2(width, height);
        }
	}
}