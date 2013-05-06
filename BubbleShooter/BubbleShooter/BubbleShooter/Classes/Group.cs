using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BubbleShooter;

namespace BubbleShooter.Classes
{
    class Group
    {
        int groupMembers;
        Boolean legalGroup = false;
        Dictionary<int, Bubble> group;

        public Group()
        {
            group = new Dictionary<int,Bubble>();
            groupMembers = 0;
        }

        public Group(int position, Bubble b)
        {
            group = new Dictionary<int, Bubble>();
            group.Add(position, b);
            groupMembers=1;
        }

        public void AddToGroup(int position, Bubble b)
        {
            group.Add(position, b);
            b.Group = this;
            groupMembers++;
        }

        public int NumberOfGroupMembers
        {
            get { return groupMembers; }
        }

        public Boolean LegalGroup
        {
            get { return legalGroup; }
            set { legalGroup = value; }
        }

        public Dictionary<int, Bubble> GroupMembers
        {
            get { return group; }
        }
    }
}
