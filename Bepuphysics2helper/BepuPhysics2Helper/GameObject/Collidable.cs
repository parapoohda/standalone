using System;
using System.Collections.Generic;
using System.Text;

namespace Com.bepuphysics2_for_nelalen.GameObject
{
    public struct ColliderBepu
    {
        public enum Type { Collidable, Unit, Characer, PlayerCharacer }
        public Type type;

        public bool isPassThrough;
        public int bodyHandle;

    }
}
