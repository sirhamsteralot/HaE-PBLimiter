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
        private string _name;
        public string Name { get { return _name; } set { _name = value;} }

        private ulong _steamId;
        public ulong SteamId { get => _steamId; set => _steamId = value; }

        private double _personalMaxMs;
        public double PersonalMaxMs { get { return _personalMaxMs; } set { _personalMaxMs = value;} }

        private bool _overrideEnabled;
        public bool OverrideEnabled { get { return _overrideEnabled; } set { _overrideEnabled = value;} }


        [XmlIgnore()]
        public double ms;

        public Player() { }

        public Player(string name, ulong steamId, double personalMaxMs, bool overrideEnabled)
        {
            _name = name;
            _personalMaxMs = personalMaxMs;
            _overrideEnabled = overrideEnabled;
            _steamId = steamId;
        }

        public void Reset()
        {
            ms = 0;
        }
    }
}
