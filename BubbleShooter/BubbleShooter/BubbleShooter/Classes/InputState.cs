using System;
using Microsoft.Xna.Framework.Input;
using BubbleShooter;

namespace BubbleShooter.Classes
{
    class InputState
    {
        KeyboardState keyboardState, oldKeyboardState;
        MouseState mouseState, oldMouseState;
        public InputState()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }

        public void Update()
        {
            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();
        }
        #region parameters
        public int X
        {
            get { return mouseState.X; }
        }

        public int Y
        {
            get { return mouseState.Y; }
        }
        #endregion parameters

        public Boolean Clicking()
        {
           return  oldMouseState.LeftButton != ButtonState.Pressed ? mouseState.LeftButton == ButtonState.Pressed : false;
        }

        public KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        public MouseState MouseState
        {
            get { return mouseState; }
        }



    }
}