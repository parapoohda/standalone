
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Com.Nelalen.GameObject
{
    internal interface ICharacter
    {
        //void AddPlayerWhoSeeMe(PlayerCharacter playerCharacter);
        void InstantMove();
        //void Move(FlatBuffers.Gate.Vector3 target, FlatBuffers.Gate.PlayerCharacterMoveType moveType);
        void PeriodicAABB();
        Capsule Shape();
        void AddCharTolist(int handle);
    }
}