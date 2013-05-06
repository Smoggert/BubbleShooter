using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BubbleShooter;

namespace BubbleShooter.Classes
{
    class Arrow
    {
        InputState inputState;
        UIState uiState;
        Vector2 position;
        Texture2D texture;
        Rectangle source = new Rectangle(0, 0, _ARROWWIDTH, _ARROWHEIGHT); 
        Rectangle Rposition = new Rectangle(0, 0, MovingBubble._DIAMETER * 2, MovingBubble._DIAMETER);
        float rotation;

        float d = (float)Math.Sqrt(2) * MovingBubble._RADIUS;
        const int _ARROWWIDTH = 100;
        const int _ARROWHEIGHT = 50;
        const float scale = 1;
        const float layerDept = 0;

        public Arrow(InputState inputState, UIState uiState, Vector2 position, Texture2D texture)
        {
            this.inputState = inputState;
            this.uiState = uiState;
            this.position = position;
            this.texture = texture;
            this.rotation = 0;
            this.setRPosition();
        }

        public void Draw()
        {
            uiState.SpriteBatch.Draw(texture,Rposition,source,Color.White,rotation,Vector2.Zero,SpriteEffects.None,layerDept);
        }

        public void Update()
        {
            setRotation();
            setRPosition();           
        }

        private void setRotation()
        {
            Vector2 mouse = Vector2.Normalize(new Vector2(inputState.X, inputState.Y) - position);
            if (mouse.Y > 0)
            {
                rotation = mouse.X >= 0 ? 0 : (float)Math.PI;
            }
            else
            rotation = -(float)Math.Acos(mouse.X);
        }

        private void setRPosition()
        {
           Rposition.X = (int)(position.X - MovingBubble._RADIUS * (Math.Cos(rotation) - Math.Sin(rotation)));
           Rposition.Y = (int)(position.Y - MovingBubble._RADIUS * (Math.Sin(rotation) + Math.Cos(rotation)));
        }
    }
}
