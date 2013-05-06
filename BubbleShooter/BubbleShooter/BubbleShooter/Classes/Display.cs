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
    class Display : DrawAbleComponent
    {
        UIState uiState;
        Texture2D texture;
        SpriteFont font;
        Vector2 position;
        Rectangle source;
        DisplayType displayType;
        Boolean visible;
        int message;
        int width;
        int height;
        string stringMessage;

        static Vector2 positionOffset = new Vector2(0, 20);


        public Display(UIState uiState, Texture2D texture, SpriteFont numberFont, Vector2 position, DisplayType displayType, int width, int height, int message, Boolean visible)
        {
            this.uiState = uiState;
            this.texture = texture;
            this.font = numberFont;
            this.font = numberFont;
            this.position = position;
            this.displayType = displayType;
            this.message = message;
            this.width = width;
            this.height = height;
            this.visible = visible;
            this.source = new Rectangle(0, 0, width, height);
        }

        public Display(UIState uiState, Texture2D texture, SpriteFont font, Vector2 position, DisplayType displayType, int width, int height, String stringMessage, Boolean visible)
        {
            this.uiState = uiState;
            this.texture = texture;
            this.font = font;
            this.position = position;
            this.displayType = displayType;
            this.stringMessage = stringMessage;
            this.width = width;
            this.height = height;
            this.visible = visible;
            this.source = new Rectangle(0, 0, width, height);
        }



        public override void Update()
        {
        }

        public override void Draw()
        {
            if (visible)
            {
                uiState.SpriteBatch.Draw(texture, position, source, Color.White);
                uiState.SpriteBatch.DrawString(font, "Congrats, you completed : " + stringMessage, position, Color.White);
                uiState.SpriteBatch.DrawString(font, "Click to go on to the next level", position+positionOffset, Color.White);
            }
        }

        public DisplayType DisplayType
        {
            get {   return displayType; }
        }

        public string StringMessage
        {
            get { return stringMessage; }
            set { stringMessage = value; }
        }

        public Boolean Visible
        {
            get { return visible; }
            set { visible = value; }
        }

    }
}
