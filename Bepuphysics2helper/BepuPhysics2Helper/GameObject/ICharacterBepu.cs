using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics2Helper.Characters;

namespace Com.bepuphysics2_for_nelalen.GameObject
{
    public interface IUnitBepu
    {
        float CalculateDistant();
        void CalculateVelocity();
        void SetCharacterInput(CharacterControllers characterCTs, int bodyHandle, Simulation simulation);
        Capsule Shape();
        int GetCharId();
    }
}