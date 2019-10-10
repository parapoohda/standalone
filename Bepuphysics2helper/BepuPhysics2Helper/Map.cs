
using BepuPhysics2Helper;
using Com.bepuphysics2_for_nelalen.GameObject;
using Com.Nelalen.GameObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MapManagerServer
{
    public class Map : IMap
    {
        public Map()
        {
            BepuSetup();
        }
        private Bepu bepu;
        private bool isSetUp;
        //<clientId,PlayerCharacter>
        private SortedDictionary<int, PlayerCharacter> playerCharacters = new SortedDictionary<int, PlayerCharacter>();
        public SortedDictionary<int, PlayerCharacter> PlayerCharacters => playerCharacters;
        //<charId,Character>
        private SortedDictionary<int, Character> characters = new SortedDictionary<int, Character>();
        public SortedDictionary<int, PlayerCharacter> PlayersByCharId => playersByCharId;
        //<charId,PlayerCharacter>
        private SortedDictionary<int, PlayerCharacter> playersByCharId = new SortedDictionary<int, PlayerCharacter>();
        public bool IsSetUp => isSetUp;

        public Bepu Bepu => bepu;

        public void BepuSetup()
        {
            try
            {
                bepu = new Bepu();
                bepu.InitializeMap();
                new Thread(PoolThread).Start();
                isSetUp = true;
            }
            catch (Exception)
            {
                isSetUp = false;
            }

        }
        private void PoolThread()
        {
            Console.WriteLine("PoolThread");
            while (true)
            {
                Console.WriteLine("bepu.Update");
                bepu.UpdateMap();
                Thread.Sleep(15);
            }
        }
        public void AddCharacter(Character character) {
            bepu.CreateCharacter(character);
            var charac = character as Unit;
            if (charac != null) {
                Console.WriteLine($"AddCharacter {character.Name}");
                character.PeriodicAABB();
            }
        }

        public void AddPlayerCharacter(PlayerCharacter character)
        {
            int clientId = character.GetClientId();

            if (playerCharacters.ContainsKey(clientId))
            {
                playerCharacters.Remove(clientId);
            }
            playerCharacters.Add(clientId, character);
            //bepu.CreateCharacter(character);
            AddCharacter(character);
        }

        public Character GetCharacter(int charId)
        {
            if (characters.ContainsKey(charId))
            {

                characters.TryGetValue(charId, out Character character);
                return character;
            }
            return null;
        }

        public PlayerCharacter GetPlayerByCharId(int charId)
        {
            if (characters.ContainsKey(charId))
            {

                playersByCharId.TryGetValue(charId, out PlayerCharacter player);
                return player;
            }
            return null;
        }
    }
}
