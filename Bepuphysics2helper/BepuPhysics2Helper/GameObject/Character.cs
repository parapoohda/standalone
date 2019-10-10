using BepuPhysics.Collidables;
using Com.bepuphysics2_for_nelalen.GameObject;
using MapManagerServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Com.Nelalen.GameObject
{
    public class PlayerInGates
    {
        public Gate gate;
        public List<PlayerCharacter> playerCharacters = new List<PlayerCharacter>();
        public PlayerInGates(Gate gate)
        {
            this.gate = gate;
        }
    }
    unsafe struct HitHandler : BepuUtilities.IBreakableForEach<CollidableReference>
    {
        public Character character;
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void SetCharacter(Character character) 
        {
            this.character = character;
        }
        public bool LoopBody(CollidableReference i)
        {
            Console.WriteLine("LoopBody");
            if (character.collider.type == ColliderBepu.Type.PlayerCharacer)
            {
                character.AddCharTolist(i.Handle);
            }
            return true;
        }
    }
    public class Character : Unit, ICharacter
    {
        private float walkSpeed = 1f;
        private float runSpeed = 2f;
        private System.Numerics.Vector3 sizeBB = new System.Numerics.Vector3(16, 16, 16);
        private Map map;
        private HashSet<PlayerInGates> playerInGates = new HashSet<PlayerInGates>();
        public List<Character> charactersWhoSeeMe = new List<Character>();
        public List<Character> CharactersWhoSeeMe => charactersWhoSeeMe;

        public HashSet<PlayerInGates> PlayerInGates => playerInGates;

        public Character(int unitId,int charId, string name, Map map, System.Numerics.Vector3 startPosition) : base(unitId,charId,  name, map, startPosition)
        {
            this.map = map;
            collider.isPassThrough = false;
            collider.type = ColliderBepu.Type.Characer;
            MoveSpeed = walkSpeed;
        }

        internal void AddTarget(Vector3 target)
        {
            throw new NotImplementedException();
        }


        public void AddCharTolist(int handle)
        {
            Console.WriteLine($"AddCharTolist {handle}");
            var character = Bepu.GetHandleUnit(handle);
            var charac = character as Character;
            if (charac != null)
            {
                AddPlayerWhoSeeMe(charac);
            }
            else {
                Console.WriteLine("null");
            }
            /*if (character.collider.type == Collider.Type.PlayerCharacer)
            {
                PlayerCharacter charac = (PlayerCharacter)character;
                AddPlayerWhoSeeMe(charac);
            }*/
        }

        
         public void AddPlayerWhoSeeMe(Character character)
        {
            Console.WriteLine("AddPlayerWhoSeeMe");
            if (!charactersWhoSeeMe.Contains(character)) {
                charactersWhoSeeMe.Add(character);
            }
            /*PlayerCharacter playerCharacter = map.getPlayerFromClientId(clientId);
            bool isGateYet = false;
            var gateId = playerCharacter.GetGate().GetId();
            if (playerInGates.Count > 0)
            {
                foreach (var gate in playerInGates)
                {
                    if (gateId == gate.gate.GetId())
                    {
                        isGateYet = true;
                        gate.playerCharacters.Add(playerCharacter);
                    }
                }
                if (!isGateYet)
                {
                    var newGate = playerCharacter.GetGate();
                    var playerInGate = new PlayerInGates(newGate);
                    playerInGates.Add(playerInGate);
                    playerInGate.playerCharacters.Add(playerCharacter);
                }
            }
            else
            {
                var newGate = playerCharacter.GetGate();
                var playerInGate = new PlayerInGates(newGate);
                playerInGates.Add(playerInGate);
                playerInGate.playerCharacters.Add(playerCharacter);
            }*/
        }
        

        public void InstantMove()
        {
            throw new NotImplementedException();
        }

        public void PeriodicAABB()
        {
            Console.WriteLine("PeriodicAABB");
            var hitHandler = new HitHandler { character = this };
            System.Numerics.Vector3 minBB = Position - (sizeBB / 2);
            System.Numerics.Vector3 maxBB = Position + (sizeBB / 2);
            var box = new BepuUtilities.BoundingBox(minBB, maxBB);
            Bepu.Simulation.BroadPhase.GetOverlaps(box, ref hitHandler);
        }

    }
       
}
