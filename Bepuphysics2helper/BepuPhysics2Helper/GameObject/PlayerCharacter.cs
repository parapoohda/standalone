
using Com.bepuphysics2_for_nelalen.GameObject;
using Com.Nelalen.GameObject;
using MapManagerServer;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Com.Nelalen.GameObject
{
    public class PlayerCharacter : Character, IPlayer
    {
        private int clientId;
        private string name;
        private int unitId;
        private Map map;
        //private MapManager.Gate gate;
        //private Map map;
        private System.Numerics.Vector3 startPosition;
        private Gate gate;

        public PlayerCharacter(int characterId, int clientId, string name, int unitId,Gate gate, Map map, System.Numerics.Vector3 startPosition) : base(unitId,characterId, name, map, startPosition)
        {
            collider.type = ColliderBepu.Type.PlayerCharacer;
            this.clientId = clientId;
            this.name = name;
            this.unitId = unitId;
            this.map = map;
            this.startPosition = startPosition;
            this.gate = gate;
            //base.AddPlayerWhoSeeMe(clientId);
        }


        public object GetMap()
        {
            return map;
        }

        public Gate GetGate()
        {
            return gate;
        }

        public int GetClientId() {
            return clientId;
        }


    }

}
