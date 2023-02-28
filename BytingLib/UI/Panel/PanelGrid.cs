﻿namespace BytingLib.UI
{
    public partial class PanelGrid : Element
    {
        public Color? Color { get; set; }
        public int Columns { get; }
        public float Gap { get; set; }
        public bool ItemsVerticalDirection { get; set; } = false;
        public bool ItemsStartLeft { get; set; } = true;
        public bool ItemsStartTop { get; set; } = true;

        public PanelGrid(Padding padding, float gap, Vector2 anchor, Color? color, int columns)
        {
            Padding = padding;
            Gap = gap;
            Anchor = anchor;
            Color = color;
            Columns = columns;
        }

        public override void UpdateTree(Rect parentRect)
        {
            Vector2 pos = Anchor * parentRect.Size + parentRect.Pos;

            Vector2 fieldSize = GetMaxChildSize();

            int columnsTaken = Math.Min(Columns, Children.Count);
            int rowsTaken = GetRowsTaken();
            Vector2 contentSize = new Vector2(
                columnsTaken * fieldSize.X + (columnsTaken - 1) * Gap,
                rowsTaken * fieldSize.Y + (rowsTaken - 1) * Gap);
            Vector2 contentSizePlusPadding = contentSize + GetPaddingSize();

            Rect rect = new Anchor(pos, Anchor).Rectangle(contentSizePlusPadding);

            absoluteRect = rect.CloneRect().Round();

            if (Children.Count == 0)
                return;

            Padding?.RemoveFromRect(rect);

            Vector2 toNextItem = Vector2.Zero;
            if (ItemsVerticalDirection)
                toNextItem.Y = fieldSize.Y + Gap;
            else
                toNextItem.X = fieldSize.X + Gap;

            Vector2 toNextRow = Vector2.Zero;
            if (ItemsVerticalDirection)
                toNextRow.X = fieldSize.X + Gap;
            else
                toNextRow.Y = fieldSize.Y + Gap;

            pos.X = ItemsStartLeft ? rect.Left : rect.Right;
            pos.Y = ItemsStartTop ? rect.Top : rect.Bottom;
            if (!ItemsStartLeft)
            {
                toNextItem.X = -toNextItem.X;
                toNextRow.X = -toNextRow.X;
                pos.X -= fieldSize.X;
            }
            if (!ItemsStartTop)
            {
                toNextItem.Y = -toNextItem.Y;
                toNextRow.Y = -toNextRow.Y;
                pos.Y -= fieldSize.Y;
            }

            Vector2 startPos = pos;

            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                Rect r = GetChildRect(new Rect(pos, fieldSize), c);
                c.UpdateTree(r);

                if (i % Columns < Columns - 1)
                {
                    pos += toNextItem;
                }
                else
                {
                    pos += toNextRow;

                    if (ItemsVerticalDirection)
                        pos.Y = startPos.Y;
                    else
                        pos.X = startPos.X;
                }
            }
        }

        private int GetRowsTaken()
        {
            return (int)MathF.Ceiling((float)Children.Count / Columns);
        }

        private Vector2 GetMaxChildSize()
        {
            Vector2 max = Vector2.Zero;
            bool allWidthNegative = true;
            bool allHeightNegative = true;

            for (int i = 0; i < Children.Count; i++)
            {
                float w = Children[i].GetWidthTopToBottom();
                if (w >= max.X)
                {
                    max.X = w;
                    allWidthNegative = false;
                }
                float h = Children[i].GetHeightTopToBottom();
                if (h >= max.Y)
                {
                    max.Y = h;
                    allHeightNegative = false;
                }
            }

            if (allWidthNegative)
                max.X = (GetInnerWidth() - Gap * (Columns - 1)) / Columns;
            if (allHeightNegative)
            {
                int rowsTaken = GetRowsTaken();
                max.Y = (GetInnerHeight() - Gap * (rowsTaken - 1)) / rowsTaken;
            }

            return max;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch, Style style)
        {
            if (Color != null)
                absoluteRect.Draw(spriteBatch, Color.Value);
        }
    }
}
