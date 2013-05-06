using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BubbleShooter;

namespace BubbleShooter.Classes
{
    class GroupChecker
    {
        delegate void BubbleChecker(int i, Bubble b);
        Dictionary<int, GridBubble> gridBubbles;
        List<DeadBubble> deadBubbles;
        List<Group> groups = new List<Group>();
        List<Group> deadGroups = new List<Group>();

        public GroupChecker(Dictionary<int, GridBubble> stoppedBubbles, List<DeadBubble> deadBubbles)
        {
            this.gridBubbles = stoppedBubbles;
            this.deadBubbles = deadBubbles;
        }

        public void CheckAll()
        {
            foreach (KeyValuePair<int, GridBubble> kvp in gridBubbles)
            {
                {
                    kvp.Value.Group = new Group(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<int, GridBubble> kvp in gridBubbles.OrderBy(k => k.Key))
            {
                if (kvp.Key < MovingBubble._BUBBLESEVENLAYER)
                {
                    kvp.Value.Group.LegalGroup = true;
                }
                if (!groups.Contains(kvp.Value.Group))
                {
                    groups.Add(kvp.Value.Group);
                    CheckEverything(kvp.Key, kvp.Value);
                }
            }


            int i = 0;
            foreach (Group g in groups)
            {
                i++;
            }

        }

        public void ClearStoppedGroups()
        {
            foreach (KeyValuePair<int, GridBubble> kvp in gridBubbles)
            {
                kvp.Value.Group = null;
            }
            groups.Clear();
        }

        public void KillMembers(Group g, BubbleShooter bS, Boolean explode)
        {
            Group temp = new Group();
            DeadBubble tempBubble;
            if (g.NumberOfGroupMembers <= 3)
            {
                bS.AdjustScore(g.NumberOfGroupMembers * 10);
            }
            else
                bS.AdjustScore(g.NumberOfGroupMembers * (g.NumberOfGroupMembers - 1) * 5);
            foreach(KeyValuePair<int, Bubble> kvp in g.GroupMembers)
            {
                tempBubble = new DeadBubble(kvp.Value, explode);
                deadBubbles.Add(tempBubble);
                temp.AddToGroup(kvp.Key, tempBubble);
                gridBubbles.Remove(kvp.Key);
            }
            g = temp;
        }

        public void DestroyMembers(Group g)
        {
            foreach (KeyValuePair<int, Bubble> kvp in g.GroupMembers)
            {
                deadBubbles.Remove((DeadBubble) kvp.Value);
            }
        }
        public void KillFlying(BubbleShooter bS)
        {
            foreach (Group g in groups)
            {
                if (!g.LegalGroup)
                    KillMembers(g, bS, false);
            }
        }

        public void CheckColoredBubble(int i, Bubble b)
        {
            if (gridBubbles.ContainsKey(i) && gridBubbles[i] != null && gridBubbles[i].BubbleColor==b.BubbleColor&&gridBubbles[i].Group!=b.Group)
            {
                b.Group.AddToGroup(i, gridBubbles[i]);
                CheckColor(i, gridBubbles[i]);
            }
        }

        public void CheckBubble(int i, Bubble b)
        {
            if (gridBubbles.ContainsKey(i) && gridBubbles[i] != null && gridBubbles[i].Group != b.Group)
            {
                b.Group.AddToGroup(i, gridBubbles[i]);
                CheckEverything(i, gridBubbles[i]);
            }
        }

        private void Check(int i, Bubble b, BubbleChecker bc)
        {
            if (i % Bubble._BUBBLESBOTHLAYERS == 0)                          //if Bubble is the first of a "firstlayer"
            {
                bc(i + 1, b);                          //check right.
                bc(i + Bubble._BUBBLESEVENLAYER, b);         //check under to the right
                bc(i - Bubble._BUBBLESUNEVENLAYER, b);        //check above to the right
            }
            else
            {
                if ((i - Bubble._BUBBLESUNEVENLAYER) % Bubble._BUBBLESBOTHLAYERS == 0)  //if Bubble is the last of a "firstlayer"
                {
                    bc(i - 1, b);                          //check left.
                    bc(i - Bubble._BUBBLESEVENLAYER, b);         //check above to the left
                    bc(i + Bubble._BUBBLESUNEVENLAYER, b);        //check under to the left
                }
                else
                {
                    if ((i - Bubble._BUBBLESEVENLAYER) % Bubble._BUBBLESBOTHLAYERS == 0)   //if Bubble is the first of a "secondlayer"
                    {
                        bc(i + 1, b);                          //check right.
                        bc(i + Bubble._BUBBLESEVENLAYER, b);         //check under to the right
                        bc(i - Bubble._BUBBLESUNEVENLAYER, b);        //check above to the right
                        bc(i - Bubble._BUBBLESEVENLAYER, b);         //check above to the left
                        bc(i + Bubble._BUBBLESUNEVENLAYER, b);        //check under to the left
                    }
                    else
                    {
                        if ((i - 2*Bubble._BUBBLESUNEVENLAYER) % Bubble._BUBBLESBOTHLAYERS == 0)   //if Bubble is the last of a "secondlayer"
                        {
                            bc(i - 1, b);                          //check left.
                            bc(i + Bubble._BUBBLESEVENLAYER, b);         //check under to the right
                            bc(i - Bubble._BUBBLESUNEVENLAYER, b);        //check above to the right
                            bc(i - Bubble._BUBBLESEVENLAYER, b);         //check above to the left
                            bc(i + Bubble._BUBBLESUNEVENLAYER, b);        //check under to the left
                        }
                        else                                                //if Bubble is the random bubble not on the side
                        {
                            bc(i + 1, b);                          //check right.
                            bc(i - 1, b);                          //check right.
                            bc(i + Bubble._BUBBLESEVENLAYER, b);         //check under to the right
                            bc(i - Bubble._BUBBLESUNEVENLAYER, b);        //check above to the right
                            bc(i - Bubble._BUBBLESEVENLAYER, b);         //check above to the left
                            bc(i + Bubble._BUBBLESUNEVENLAYER, b);        //check under to the left
                        }
                    }
                }
            }
        }

        public void CheckColor(int i, Bubble b)
        {
            Check(i, b, CheckColoredBubble);
        }

        public void CheckEverything(int i, Bubble b)
        {
            Check(i, b, CheckBubble);
        }
    }
}


