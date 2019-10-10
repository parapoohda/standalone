using System;

namespace MapManagerServer
{
    public class Gate : IGate
    {
        private long gateId;
        private IMapManageServer mapManagerServer;
        private INetPeer peer;

        public Gate()
        {
            this.gateId = 0;
        }
        public Gate(long gateId, IMapManageServer mapManagerServer, INetPeer peer)
        {
            this.gateId = gateId;
            this.mapManagerServer = mapManagerServer;
            this.peer = peer;
        }

        public long GetId()
        {
            return gateId;
        }
    }
}