﻿using Microsoft.Xna.Framework.Input;

namespace BytingLib
{
    public class MouseInput : IUpdate
    {
        private readonly Func<MouseState> getState;

        private MouseState previousState;
        private MouseState currentState;

        private bool isPreviousMouseStateSet = false;
        private int updateCount;

        public delegate void StateChanged(MouseState current, MouseState previous);
        public event StateChanged? OnStateChanged;

        public MouseInput(Func<MouseState> getState)
        {
            this.getState = getState;
        }

        public void Update()
        {
            UpdateUsingState(getState());
        }

        private void UpdateUsingState(MouseState keyboardState)
        {
            previousState = currentState;
            currentState = keyboardState;

            updateCount++;
            if (updateCount == 2)
                isPreviousMouseStateSet = true;

            if (OnStateChanged != null
                && currentState != previousState)
                OnStateChanged?.Invoke(currentState, previousState);
        }

        public IKey Left => GetKey(f => f.LeftButton);
        public IKey Middle => GetKey(f => f.MiddleButton);
        public IKey Right => GetKey(f => f.RightButton);
        public IKey XButton1 => GetKey(f => f.XButton1);
        public IKey XButton2 => GetKey(f => f.XButton2);

        public int X => currentState.X;
        public int Y => currentState.Y;
        public Int2 Position => new Int2(currentState.Position);

        public int XDelta => isPreviousMouseStateSet ? (currentState.X - previousState.X) : 0;
        public int YDelta => isPreviousMouseStateSet ? (currentState.Y - previousState.Y) : 0;
        public Int2 Move => isPreviousMouseStateSet ? (new Int2(currentState.Position - previousState.Position)) : Int2.Zero;

        public int Scroll
        {
            get
            {
                int scroll = currentState.ScrollWheelValue - previousState.ScrollWheelValue;
                scroll = ConsiderIntMaxValue(scroll);
                return scroll;
            }
        }

        public int ScrollHorizontal
        {
            get
            {
                int scroll = currentState.HorizontalScrollWheelValue - previousState.HorizontalScrollWheelValue;
                scroll = ConsiderIntMaxValue(scroll);
                return scroll;
            }
        }

        private int ConsiderIntMaxValue(int scroll)
        {
            // checking for scrolling over int.MaxValue or under int.MinValue
            if (currentState.ScrollWheelValue > int.MaxValue * 0.5
                && previousState.ScrollWheelValue < int.MinValue * 0.5)
                scroll = -1;
            else if (currentState.ScrollWheelValue < int.MinValue * 0.5
                && previousState.ScrollWheelValue > int.MaxValue * 0.5)
                scroll = 1;
            return scroll;
        }


        public IKey GetKey(Func<MouseState, ButtonState> getButtonState)
        {
            bool downNow = getButtonState(currentState) == ButtonState.Pressed;
            bool downPreviously = getButtonState(previousState) == ButtonState.Pressed;
            return new Key(downNow, downNow != downPreviously);
        }

        public MouseState GetState() => currentState;
        public MouseState GetStatePrevious() => previousState;
    }
}