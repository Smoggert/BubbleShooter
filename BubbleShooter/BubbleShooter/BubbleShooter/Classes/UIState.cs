using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using BubbleShooter;


namespace BubbleShooter.Classes
{
    class UIState 
    {
        SpriteBatch spriteBatch;
        ContentManager contentManager;

        public UIState(SpriteBatch spriteBatch, ContentManager contentManager)
        {
            this.spriteBatch = spriteBatch;
            this.contentManager = contentManager;
        }

        public T Load<T>(String assetName)
        {
            return contentManager.Load<T>(assetName);
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }



    }
}
