using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BubbleShooter;

namespace BubbleShooter.Classes
{
    class Button : DrawAbleComponent
    {
        InputState inputState;
        UIState uiState;
        Texture2D texture;
        SpriteFont font;
        Vector2 position;
        Rectangle source,  Rposition;
        ButtonType buttonType;
        Boolean hovering;

        const int _WIDTH = 75;
        const int _HEIGHT = 20 ;

        public event ButtonEventManager OnClick;

        public Button(InputState inputState, UIState uiState, Texture2D texture, SpriteFont font, Vector2 position, ButtonType buttonType)
        {
            this.inputState = inputState;
            this.uiState = uiState;
            this.texture = texture;
            this.font = font;
            this.position = position;
            this.Rposition = new Rectangle((int)position.X,(int) position.Y, _WIDTH, _HEIGHT) ;
            this.buttonType = buttonType;
            this.source = new Rectangle((int) buttonType*_WIDTH, 0, _WIDTH, _HEIGHT);
        }

        public override void Update()
        {
            hovering = Rposition.Contains(inputState.X, inputState.Y);
            source.Y =  hovering ? (inputState.MouseState.LeftButton == ButtonState.Pressed ? 40 : 20) : 0 ;
            if (hovering&&inputState.Clicking()&&OnClick!= null)
            {
                OnClick();
            }
        }

        public override void Draw()
        {
            uiState.SpriteBatch.Draw(texture, Rposition, source, Color.White);
            uiState.SpriteBatch.DrawString(font, buttonType.ToString(), position, Color.Black);
        }
    }
}
