using System.Collections.Generic;
using BepuPhysics2Helper;
using Com.Nelalen.GameObject;

namespace MapManagerServer
{
    public interface IMap
    {
        Bepu Bepu { get; }
        bool IsSetUp { get; }
        SortedDictionary<int, PlayerCharacter> PlayerCharacters { get; }
        SortedDictionary<int, PlayerCharacter> PlayersByCharId { get; }

        void AddCharacter(Character character);
        void AddPlayerCharacter(PlayerCharacter character);
        void BepuSetup();
        Character GetCharacter(int charId);
        PlayerCharacter GetPlayerByCharId(int charId);
    }
}