using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BubbleShooter;

namespace BubbleShooter.Classes
{
    class Side
    {
        Vector2 normal;
        float distance;

        public Side(float distance , Vector2 normal)
        {
            this.distance = distance;
            this.normal = Vector2.Normalize(normal);
        }

        public virtual Vector2 Normal
        {
            get { return normal; }
        }

        public virtual float Distance
        {
            get { return distance; }
        }
    }

    class SpeedSide : Side
    {
        Vector2 speedSetter;

        public SpeedSide(float distance, Vector2 normal) : base(distance, normal)
        {
            this.speedSetter = new Vector2(0,0);
        }

        public SpeedSide(float distance, Vector2 normal, Vector2 speedSetter) : base(distance, normal)
        {
            this.speedSetter = speedSetter;
        }


        public Vector2 SpeedSetter
        {
            get
            {
                return speedSetter;
            }
        }
    }
}
