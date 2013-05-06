using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BubbleShooter;

namespace BubbleShooter.Classes
{
    class Bubble
    {
        protected UIState uiState;
        protected InputState inputState;
        protected Vector2 middlePoint;
        protected Vector2 speed;
        protected Rectangle drawingRectangle;
        protected Rectangle sourceRectangle;
        protected Texture2D texture;
        protected BubbleColor bubbleColor;
        protected Group group;


        public static int _RADIUS = 14;
        public static int _DIAMETER = _RADIUS * 2;
        public static int _BUBBLESEVENLAYER = 10;
        public static int _BUBBLESUNEVENLAYER = 9;
        public static int _BUBBLESBOTHLAYERS = _BUBBLESEVENLAYER + _BUBBLESUNEVENLAYER;

        protected const int _TEXTUREWIDTH = 50;
        protected const float _SPEEDFACTOR = 10f;

        public Bubble()
        {
        }
        public Bubble(UIState uiState, InputState inputState, Vector2 middlePoint, Vector2 speed, Texture2D texture, BubbleColor bubbleColor)
        {
            this.uiState = uiState;
            this.inputState = inputState;
            this.middlePoint = middlePoint;
            this.speed = speed;
            this.texture = texture;
            this.bubbleColor = bubbleColor;
            this.sourceRectangle = new Rectangle((int)bubbleColor * _TEXTUREWIDTH, 0, _TEXTUREWIDTH, _TEXTUREWIDTH);
            this.drawingRectangle = new Rectangle((int)middlePoint.X - _RADIUS, (int)middlePoint.Y - _RADIUS, _RADIUS * 2, _RADIUS * 2);
        }

        public Bubble(Bubble bubble) : this(bubble.uiState,bubble.inputState,bubble.middlePoint,bubble.speed,bubble.texture,bubble.bubbleColor)
        {
            this.sourceRectangle = bubble.sourceRectangle;
            this.drawingRectangle = bubble.drawingRectangle;
        }

        public void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw()
        {
            uiState.SpriteBatch.Draw(texture, drawingRectangle, sourceRectangle, Color.White);
        }

        #region Protected methods
        protected void AdjustDrawingRectangle()
        {
            drawingRectangle.X = (int)middlePoint.X - _RADIUS;
            drawingRectangle.Y = (int)middlePoint.Y - _RADIUS;
        }
        #endregion

        #region Static methods

        public static Vector2 GetGrid(int i, BubbleShooter bS)
        {
            int tempX, tempY;
            float X, Y;
            if (i % _BUBBLESBOTHLAYERS < _BUBBLESEVENLAYER)
            {
                tempX = i % _BUBBLESBOTHLAYERS;
                tempY = 2 * (i - tempX) / _BUBBLESBOTHLAYERS;

                X = bS.OffsetLeft + _RADIUS + tempX * _DIAMETER;
            }
            else
            {
                tempX = i % _BUBBLESBOTHLAYERS - _BUBBLESEVENLAYER;
                tempY = 2 * (i - tempX) / _BUBBLESBOTHLAYERS;

                X = bS.OffsetLeft + _DIAMETER + tempX * _DIAMETER;
            }
            Y = bS.OffsetTop + _RADIUS + tempY * (float)Math.Sqrt(3) * _RADIUS;

            return new Vector2(X, Y);
        }

        #endregion

        #region Parameters
        public BubbleColor BubbleColor
        {
            get { return bubbleColor; }
        }

        public Group Group
        {
            get { return group; }
            set { group = value; }
        }

        public Boolean HasGroup
        {
            get { return group != null; }
        }

        public Vector2 MiddlePoint
        {
            get { return middlePoint; }
            set { middlePoint = value; }
        }

        #endregion
    }

    class MovingBubble : Bubble 
    {
        CollisionChecker collisionChecker;
        Boolean isLaunched = false;


        public MovingBubble(UIState uiState, InputState inputState, CollisionChecker collisionChecker,  Vector2 middlePoint, Vector2 speed, Texture2D texture, BubbleColor bubbleColor) : base(uiState, inputState, middlePoint,speed, texture, bubbleColor )
        {
            this.collisionChecker = collisionChecker;
        }

        public override void Update(GameTime gameTime)
        {
                middlePoint += speed * (float)(0.06 * gameTime.ElapsedGameTime.Milliseconds);
                collisionChecker.Check(this);
                AdjustDrawingRectangle();
        }

        public void LaunchBubble(Object sender)
        {
            if (sender is BubbleShooter)
            {
                if (inputState.Y < ((BubbleShooter)sender).Height)
                {
                    Vector2 temp = new Vector2();
                    temp.X = inputState.X - middlePoint.X;
                    temp.Y = inputState.Y - middlePoint.Y;
                    temp.Normalize();
                    speed = temp * _SPEEDFACTOR;
                    isLaunched = true;
                    ((BubbleShooter)sender).Click -= this.LaunchBubble;
                }
            }            
        }

        public Vector2 SetIntoGrid(BubbleShooter bS)
        {
            int tempX, tempY;
            tempY = ((int)middlePoint.Y - bS.OffsetTop)  / (int)(Math.Sqrt(3)* _RADIUS);

            middlePoint.Y = bS.OffsetTop +  _RADIUS + tempY * (float)Math.Sqrt(3) * _RADIUS;
            if (tempY % 2 != 0)
            {
                tempX = ((int)middlePoint.X - bS.OffsetLeft - _RADIUS) / _DIAMETER;
                middlePoint.X = bS.OffsetLeft + _DIAMETER + tempX * _DIAMETER;
            }
            else
            {
                tempX = ((int)middlePoint.X - bS.OffsetLeft) / _DIAMETER;
                middlePoint.X = bS.OffsetLeft + _RADIUS + tempX * _DIAMETER;
            }
            AdjustDrawingRectangle();
            return new Vector2(tempX, tempY);
        }


        public Boolean IsMoving
        {
            get { return speed.X != 0 || speed.Y != 0; }
        }

        public Boolean IsLaunched
        {
            get { return isLaunched; }
        }

        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public int Radius
        {
            get { return _RADIUS; }
        }


    }

    class GridBubble : Bubble
    {
        public GridBubble(UIState uiState, InputState inputState, Vector2 middlePoint, Texture2D texture, BubbleColor bubbleColor) : base(uiState, inputState, middlePoint, Vector2.Zero, texture, bubbleColor )
        {
        }
        public GridBubble(Bubble bubble): base(bubble)
        {
        }


    }

    class DeadBubble : Bubble
    {
        Boolean exploded;
        int deadTimer;
        const int _DEADYSPEED = 18;

        public static int _MAXDEADTIMEREXPL = 18;
        public static int _MAXDEADTIMERNOTEXPL = 35;

        public DeadBubble(Bubble bubble, Boolean exploded): base(bubble)
        {
            this.deadTimer = 0;
            this.exploded = exploded;
            if (!exploded)
                this.speed.Y = _DEADYSPEED;
        }
        public override void Update(GameTime gameTime)
        {
            if(exploded)
            {
                deadTimer++;
                sourceRectangle.Y = deadTimer / 3 * _TEXTUREWIDTH;
            }
            else
            {
                middlePoint += speed * (float)(0.06 * gameTime.ElapsedGameTime.Milliseconds);
                AdjustDrawingRectangle();
                deadTimer++;
            }
        }
        public override void Draw()
        {
            uiState.SpriteBatch.Draw(texture, drawingRectangle, sourceRectangle, Color.White);
        }

        public Boolean Exploded
        {
            get { return exploded; }
        }
        public int DeadTimer
        {
            get { return deadTimer; }
        }
    }

}
