using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Sandbox.Game.World;

namespace HaE_PBLimiter
{
    public class Player
    {
        public long identityId;

        private string _name;
        [XmlIgnore()]
        public string Name { get { return _name; } set { _name = value; PBLimiter_Logic.Save(); } }

        private double _personalMaxMs;
        public double PersonalMaxMs { get { return _personalMaxMs; } set { _personalMaxMs = value; PBLimiter_Logic.Save(); } }

        private bool _overrideEnabled;
        public bool OverrideEnabled { get { return _overrideEnabled; } set { _overrideEnabled = value; PBLimiter_Logic.Save(); } }

        [XmlIgnore()]
        public double ms;

        public Player() { }

        public void Reset()
        {
            ms = 0;
        }
    }
}
