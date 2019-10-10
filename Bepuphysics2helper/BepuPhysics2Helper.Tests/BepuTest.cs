using System;
using System.Numerics;
using Xunit;
using bepuphysics2_for_nelalen;
using Com.bepuphysics2_for_nelalen.GameObject;

namespace bepuphysics2_for_nelalen.Tests
{
    public class BepuTest
    {
        [Fact(DisplayName = "test initiate(is it have camera character yet?)")]
        public void TestInitiate()
        {
            var bepu = new Bepu();
            bepu.Initialize();
            var chars = bepu.Characters;
            //because it can seen(if choose correctly) so it have to have character with camera
            Assert.NotEmpty(chars);
            //because characterMainCamera get value from unitId 0
            chars.TryGetValue(0, out var character);
            Assert.NotNull(character);

        }

        [Fact(DisplayName = "test initiate(use for real)")]
        public void TestInitiateMap()
        {
            var bepu = new Bepu();
            bepu.InitializeMap();
            Assert.Empty(bepu.Characters);
            var count = bepu.Simulation.Statics.Count;
            //add ground to stand on
            Assert.Equal(1, count);
        }

        [Theory(DisplayName = "test add character")]
        [InlineData(0, "test1", 0, 0, 0)]
        public void TestAddCharacter(int unitId, string name, float x, float y, float z)
        {
            var bepu = new Bepu();
            bepu.InitializeMap();
            var character = new CharacterBepu(unitId, name, bepu, new Vector3(x, y, z));
            bepu.CreateCharacter(character);
            bepu.Characters.TryGetValue(unitId, out var getCharacter);
            Assert.Equal(character, getCharacter);
        }

        [Theory(DisplayName = "test add walk target to character")]
        [InlineData(0, "test1", 0, 1, 0, 1, 0)]
        public void TestAddTarget(int unitId, string name, float x, float y, float z, float xt, float zt)
        {
            var bepu = new Bepu();
            bepu.InitializeMap();
            var character = new CharacterBepu(unitId, name, bepu, new Vector3(x, y, z));
            bepu.CreateCharacter(character);
            var target = new Vector3(xt, 0, zt);
            bepu.AddTargetToCharacter(target, character);
            Assert.Equal(target, character.Target);
        }
        [Theory(DisplayName = "test walk")]
        [InlineData(0, "test1", 0, 1, 0, 1, 0)]
        public void TestWalk(int unitId, string name, float x, float y, float z, float xt, float zt)
        {
            int times = 100;
            var bepu = new Bepu();
            bepu.InitializeMap();
            var character = new CharacterBepu(unitId, name, bepu, new Vector3(x, y, z));
            bepu.CreateCharacter(character);
            var target = new Vector3(xt, 0, zt);
            bepu.AddTargetToCharacter(target, character);
            //Assert.Equal(target, character.Target);
            for (var i = 0; i < times; i++)
            {
                bepu.Update();
            }
            //Console.WriteLine(target.X);
            //if (xt > 0) Assert.Equal(x + (times * xt * 0.015), target.X);
            //if (zt > 0) Assert.Equal(z + (times * zt * 0.015), target.Z);
        }
        [Theory(DisplayName = "test walk")]
        [InlineData(2, "test1", 0, 1.49, 0, 1, 0)]
        public void test(int unitId, string name, float x, float y, float z, float xt, float zt)
        {
            int times = 2;
            var bepu = new Bepu();
            bepu.Initialize();
            var character = new CharacterBepu(unitId, name, bepu, new Vector3(x, y, z));
            bepu.CreateCharacter(character);
            var target = new Vector3(xt, 0, zt);
            bepu.AddTargetToCharacter(target, character);
            //Assert.Equal(target, character.Target);
            for (var i = 0; i < times; i++)
            {
                bepu.Update();
            }
            //it is accerate so this is wrong
            if (xt > 0) Assert.Equal(x + (times * xt * 0.015), character.Position.X);
            if (zt > 0) Assert.Equal(z + (times * zt * 0.015), character.Position.Z);
        }


    }
}
