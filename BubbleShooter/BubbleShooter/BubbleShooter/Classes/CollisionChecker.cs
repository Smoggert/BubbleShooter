using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BubbleShooter;

namespace BubbleShooter.Classes
{
    class CollisionChecker
    {
        List<Side> sides = new List<Side>();
        Dictionary<int, GridBubble> stoppedBubbles;

        public CollisionChecker(Dictionary<int, GridBubble> stoppedBubbles)
        {
            this.stoppedBubbles = stoppedBubbles;
        }

        public void AddSide(Side s)
        {
            sides.Add(s);
        }

        public void Check(MovingBubble b)
        {
            foreach (Side s in sides)
            {
                float d;
                d = Vector2.Dot(b.MiddlePoint, s.Normal) - s.Distance * s.Normal.X - s.Distance * s.Normal.Y - b.Radius ;
                if (d < 0)
                {
                    if (s is SpeedSide)
                    {
                        b.MiddlePoint -= s.Normal * d;
                        b.Speed = ((SpeedSide)s).SpeedSetter;
                    }
                    else
                    b.MiddlePoint -= s.Normal * 2 * d;
                    b.Speed -= 2*s.Normal * Vector2.Dot(b.Speed , s.Normal);

                }
            }

            foreach (KeyValuePair<int,GridBubble> kvp in stoppedBubbles)
            {
                float d;
                d = (kvp.Value.MiddlePoint - b.MiddlePoint).Length() - (2*Bubble._RADIUS);
                if (d < 0)
                {
                    b.Speed = new Vector2(0, 0);

                }
            }



        }
    }
}
