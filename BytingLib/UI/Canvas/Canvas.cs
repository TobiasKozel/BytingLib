﻿namespace BytingLib.UI
{
    public abstract class Canvas : Element, IUpdate
    {
        private Element? updateCatch;
        public Color? ClearColor { get; set; }
        protected readonly Func<Rect> getRenderRect;
        protected readonly ElementInput input;
        public StyleRoot StyleRoot { get; set; }

        public Canvas(Func<Rect> getRenderRect, MouseInput mouse, StyleRoot style)
        {
            this.getRenderRect = getRenderRect;
            StyleRoot = style;
            input = CreateElementInput(mouse);
        }

        protected virtual ElementInput CreateElementInput(MouseInput mouse)
        {
            return new ElementInput(mouse, SetUpdateCatch);
        }

        public void Update()
        {
            input.Mouse.Update();

            if (updateCatch != null)
            {
                updateCatch.Update(input);
            }
            else
            {
                UpdateSelf(input);

                for (int i = 0; i < Children.Count; i++)
                    Children[i].Update(input);
            }
        }

        public override void Update(ElementInput input)
        {
            throw new BytingException("Call Update()");
        }

        public void SetUpdateCatch(Element? element)
        {
            updateCatch = element;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch, StyleRoot style)
        {
            throw new BytingException("Call DrawBatch() instead");
        }

        public abstract void UpdateTree();
    }
}
