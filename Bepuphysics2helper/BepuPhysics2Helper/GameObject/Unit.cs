
using Com.bepuphysics2_for_nelalen.GameObject;
using MapManagerServer;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Com.Nelalen.GameObject
{
    public class Unit:UnitBepu,IUnit
    {
        public Unit(int unitId,int charId,string name,Map map,Vector3 startPosition) :base(unitId,charId, name, map.Bepu, startPosition) 
        { 
        }
    }
}
