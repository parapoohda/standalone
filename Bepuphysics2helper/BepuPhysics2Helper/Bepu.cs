using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuPhysics2Helper.Characters;
using BepuUtilities;
using BepuUtilities.Memory;
using Com.bepuphysics2_for_nelalen.GameObject;
using DemoContentLoader;
using DemoRenderer;
using DemoRenderer.UI;
using DemoUtilities;
using OpenTK.Input;

namespace BepuPhysics2Helper
{
    //Bepu for nelalen
    public class Bepu
    {
        CharacterControllers characterCTs;

        public Simulation Simulation { get; internal set; }
        public BufferPool BufferPool { get; private set; }
        public SimpleThreadDispatcher ThreadDispatcher { get; internal set; }


        // <charId,Character>
        private SortedDictionary<int, UnitBepu> characters = new SortedDictionary<int, UnitBepu>();
        public SortedDictionary<int, UnitBepu> Characters => characters;
        // <handleId, Character(that walk)>
        private SortedDictionary<int, UnitBepu> walkCharacters = new SortedDictionary<int, UnitBepu>();
        private List<int> walkRemoveCharacters = new List<int>();
        // <handleId, Character>
        private SortedDictionary<int, UnitBepu> handleUnits = new SortedDictionary<int, UnitBepu>();
        public void Initialize(ContentArchive content, Camera camera)
        {
            
            camera.Position = new Vector3(20, 10, 20);
            camera.Yaw = MathF.PI;
            camera.Pitch = 0;
            Initialize();

            //CreateCharacter(new Vector3(0f,0f,0f),0,"test1");
            //CreateCharacter(new Vector3(1f,0f,0f),0,"test2");
        }
        public void Initialize()
        {
            InitializeMap();
            var character = new UnitBepu(0, 0, "test0", this, new Vector3(0, 5, -2));
            CreateCharacter(character);
            characters.TryGetValue(0,out characterMainCamera);
            character = new UnitBepu(1, 0, "test1", this, new Vector3(0, 4, 0));
            CreateCharacter(character);
            AddTargetToCharacter(new Vector3(20, 0, 20),character);
            
        }
        public void InitializeMap()
        {
            ThreadDispatcher = new SimpleThreadDispatcher(Environment.ProcessorCount);
            BufferPool = new BufferPool();
            characterCTs = new CharacterControllers(BufferPool);
            var collider = new BodyProperty<ColliderBepu>();
            Simulation = Simulation.Create(BufferPool, new CharacterNarrowphaseCallbacks(characterCTs), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)));
            Simulation.Statics.Add(new StaticDescription(new Vector3(0, 0, 0), new CollidableDescription(Simulation.Shapes.Add(new Box(200, 1, 200)), 0.1f)));

        }
        //CharacterBepu character;
        int bodyHandle;
        public void CreateCharacter(UnitBepu character)
        {
            int unitId = character.UnitId;
            //Vector3 position = character.Position;
            string name = character.Name;
            var shape = new Capsule(0.5f, 1);
            var shapeIndex = Simulation.Shapes.Add(shape);
            bodyHandle = Simulation.Bodies.Add(BodyDescription.CreateDynamic(character.GetStartPosition(), new BodyInertia { InverseMass = 1f / 1f  }, new CollidableDescription(shapeIndex, 0.1f), new BodyActivityDescription(shape.Radius * 0.02f)));
            //CharacterBepu character = new CharacterBepu(unitId ,name ,this ,position);
            character.SetCharacterInput(characterCTs, bodyHandle,Simulation);
            characters.Add(character.UnitId,character);
            handleUnits.TryAdd<int, UnitBepu>(bodyHandle,character);
            //Console.WriteLine($"characters[0]: {characters}");

            //character = new CharacterInput(characters, position, new Capsule(0.5f, 1), 0.1f, 1, 20, 100, 6, 4, MathF.PI * 0.4f);
        }
        public void AddTargetToCharacter(Vector3 target, UnitBepu character)
        {
            character.Target = target;
            walkCharacters.TryAdd<int, UnitBepu>(character.UnitId, character);
        }
        public UnitBepu GetHandleUnit(int handle)
        {
            handleUnits.TryGetValue(handle, out UnitBepu value);
            return value;
        }
        public void Remove(UnitBepu character)
        {
            Console.WriteLine("Remove character");
            var unitId = character.UnitId;
            var handle = character.collider.bodyHandle;
            if (characters.ContainsKey(unitId))
            {
                characters.Remove(unitId);
            }
            if (walkCharacters.ContainsKey(handle))
            {
                walkRemoveCharacters.Add(handle);
            }
        }
        static Key MoveForward = Key.W;
        static Key MoveBackward = Key.S;
        static Key MoveRight = Key.D;
        static Key MoveLeft = Key.A;
        int i = 0;
        
        public void Update()
        {
            i++;
            if (i % 10 == 0) {
                characters.TryGetValue(1, out UnitBepu character);
                //Vector3 Velo = character.VelocityInBepu.Linear;
                //Console.WriteLine($"x: {Velo.X} y: {Velo.Y} z: {Velo.Z}");
                Vector3 Position = character.Position;
                Console.WriteLine($"x: {Position.X} y: {Position.Y} z: {Position.Z}");
            }
            foreach (var pair in walkCharacters)
            {
                var character = pair.Value;
#if DEBUG
                if (i % 120 == 0)
                {
                    Console.WriteLine($"{character.IsCalculateVelocityYet}");
                    Console.WriteLine($"{character.Velocity}");
                    Console.WriteLine($"{character.MoveSpeed}");
                }
#endif 
                var distant = character.CalculateDistant();
                if (true || distant > 0.2)
                {

                    if (!character.IsCalculateVelocityYet)
                    {
                        character.CalculateVelocity();
                    }
                    var movementDirection1 = new Vector2(0, 1);
                    const float simulationDt = 1 / 60f;
                    character.CharacterInputt.UpdateCharacterGoals(movementDirection1, character.MoveSpeed, simulationDt);
                }
                else
                {
#if DEBUG
                    Console.WriteLine($"remove from walk");
#endif
                }
            }
#if DEBUG
            //Console.WriteLine($"walkRemoveCharacters.Count {walkRemoveCharacters.Count}");
#endif

            foreach (var handle in walkRemoveCharacters)
            {
                if (walkCharacters.ContainsKey(handle))
                {
                    walkCharacters.Remove(handle);
                    //simulation.Bodies.Remove(handle);
                    //handleUnits.Remove(handle);
                }
            }

            Simulation.Timestep(1 / 60f, ThreadDispatcher);
        }

        public void UpdateMap()
        {
            i++;
            
            foreach (var pair in walkCharacters)
            {
                var character = pair.Value;
#if DEBUG
                if (i % 120 == 0)
                {
                    Console.WriteLine($"{character.IsCalculateVelocityYet}");
                    Console.WriteLine($"{character.Velocity}");
                    Console.WriteLine($"{character.MoveSpeed}");
                }
#endif 
                var distant = character.CalculateDistant();
                if (true || distant > 0.2)
                {

                    if (!character.IsCalculateVelocityYet)
                    {
                        character.CalculateVelocity();
                    }
                    var movementDirection1 = new Vector2(0, 1);
                    const float simulationDt = 1 / 60f;
                    character.CharacterInputt.UpdateCharacterGoals(movementDirection1, character.MoveSpeed, simulationDt);
                }
                else
                {
#if DEBUG
                    Console.WriteLine($"remove from walk");
#endif
                }
            }
#if DEBUG
            //Console.WriteLine($"walkRemoveCharacters.Count {walkRemoveCharacters.Count}");
#endif

            foreach (var handle in walkRemoveCharacters)
            {
                if (walkCharacters.ContainsKey(handle))
                {
                    walkCharacters.Remove(handle);
                    //simulation.Bodies.Remove(handle);
                    //handleUnits.Remove(handle);
                }
            }

            Simulation.Timestep(1 / 60f, ThreadDispatcher);
        }
        public void Render(Renderer renderer, Camera camera, Input input, TextBuilder text, Font font)
        {
            float textHeight = 16;
            var position = new Vector2(32, renderer.Surface.Resolution.Y - textHeight * 9);
            //renderer.TextBatcher.Write(text.Clear().Append("Toggle character: C"), position, textHeight, new Vector3(1), font);
            position.Y += textHeight * 1.2f;
            characters.TryGetValue(0, out var character);
            characterMainCamera.CharacterInputt.RenderControls(position, textHeight, renderer.TextBatcher, text, font);
            character.CharacterInputt.UpdateCameraPosition(camera);
        }

        protected void OnDispose()
        {

        }
        bool disposed;
        private UnitBepu characterMainCamera;

        internal void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                OnDispose();
                Simulation.Dispose();
                BufferPool.Clear();
                ThreadDispatcher.Dispose();
            }
        }
    }

    struct MolCallbacks : INarrowPhaseCallbacks
    {
        public BodyProperty<ColliderBepu> Collider;
        public CharacterControllers Characters;
        public void Initialize(Simulation simulation)
        {
            Collider.Initialize(simulation.Bodies);
            Characters.Initialize(simulation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b)
        {
            //It's impossible for two statics to collide, and pairs are sorted such that bodies always come before statics.
            /*if (b.Mobility != CollidableMobility.Static)
            {
                return SubgroupCollisionFilter.AllowCollision(Properties[a.Handle].Filter, Properties[b.Handle].Filter);
            }*/

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
        {
            return true;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void CreateMaterial(CollidablePair pair, out PairMaterialProperties pairMaterial)
        {
            pairMaterial.FrictionCoefficient = Properties[pair.A.Handle].Friction;
            if (pair.B.Mobility != CollidableMobility.Static)
            {
                //If two bodies collide, just average the friction.
                pairMaterial.FrictionCoefficient = (pairMaterial.FrictionCoefficient + Properties[pair.B.Handle].Friction) * 0.5f;
            }
            pairMaterial.MaximumRecoveryVelocity = 2f;
            pairMaterial.SpringSettings = new SpringSettings(30, 1);
        }*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void GetMaterial(out PairMaterialProperties pairMaterial)
        {
            pairMaterial = new PairMaterialProperties { FrictionCoefficient = 1, MaximumRecoveryVelocity = 2, SpringSettings = new SpringSettings(30, 1) };
        }

         [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : struct, IContactManifold<TManifold>
        {
            pairMaterial = new PairMaterialProperties { FrictionCoefficient = 1, MaximumRecoveryVelocity = 2, SpringSettings = new SpringSettings(30, 1) };
            Characters.TryReportContacts(pair, ref manifold, workerIndex, ref pairMaterial);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ConvexContactManifold* manifold)
        {
            //Console.WriteLine("ConfigureContactManifold3");

            return true;
        }

        public void Dispose()
        {
            Collider.Dispose();
            
        }
    }

    public struct CharacterNarrowphaseCallbacks : INarrowPhaseCallbacks
    {
        public CharacterControllers Characters;

        public CharacterNarrowphaseCallbacks(CharacterControllers characters)
        {
            Characters = characters;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void GetMaterial(out PairMaterialProperties pairMaterial)
        {
            pairMaterial = new PairMaterialProperties { FrictionCoefficient = 1, MaximumRecoveryVelocity = 2, SpringSettings = new SpringSettings(30, 1) };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, ConvexContactManifold* manifold, out PairMaterialProperties pairMaterial)
        {
            GetMaterial(out pairMaterial);
            Characters.TryReportContacts(pair, ref *manifold, workerIndex, ref pairMaterial);
            return true;
        }

        

        public void Dispose()
        {
            Characters.Dispose();
        }

        public void Initialize(Simulation simulation)
        {
            Characters.Initialize(simulation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : struct, IContactManifold<TManifold>
        {
            pairMaterial = new PairMaterialProperties { FrictionCoefficient = 1, MaximumRecoveryVelocity = 2, SpringSettings = new SpringSettings(30, 1) };
            Characters.TryReportContacts(pair, ref manifold, workerIndex, ref pairMaterial);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
        {
            return true;
        }
    }
    public struct DemoPoseIntegratorCallbacks : IPoseIntegratorCallbacks
    {
        public Vector3 Gravity;
        public float LinearDamping;
        public float AngularDamping;
        Vector3 gravityDt;
        float linearDampingDt;
        float angularDampingDt;

        public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

        public DemoPoseIntegratorCallbacks(Vector3 gravity, float linearDamping = .03f, float angularDamping = .03f) : this()
        {
            Gravity = gravity;
            LinearDamping = linearDamping;
            AngularDamping = angularDamping;
        }

        public void PrepareForIntegration(float dt)
        {
            //No reason to recalculate gravity * dt for every body; just cache it ahead of time.
            gravityDt = Gravity * dt;
            //Since this doesn't use per-body damping, we can precalculate everything.
            linearDampingDt = MathF.Pow(MathHelper.Clamp(1 - LinearDamping, 0, 1), dt);
            angularDampingDt = MathF.Pow(MathHelper.Clamp(1 - AngularDamping, 0, 1), dt);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IntegrateVelocity(int bodyIndex, in RigidPose pose, in BodyInertia localInertia, int workerIndex, ref BodyVelocity velocity)
        {
            //Note that we avoid accelerating kinematics. Kinematics are any body with an inverse mass of zero (so a mass of ~infinity). No force can move them.
            if (localInertia.InverseMass > 0)
            {
                velocity.Linear = (velocity.Linear + gravityDt) * linearDampingDt;
                velocity.Angular = velocity.Angular * angularDampingDt;
            }
            //Implementation sidenote: Why aren't kinematics all bundled together separately from dynamics to avoid this per-body condition?
            //Because kinematics can have a velocity- that is what distinguishes them from a static object. The solver must read velocities of all bodies involved in a constraint.
            //Under ideal conditions, those bodies will be near in memory to increase the chances of a cache hit. If kinematics are separately bundled, the the number of cache
            //misses necessarily increases. Slowing down the solver in order to speed up the pose integrator is a really, really bad trade, especially when the benefit is a few ALU ops.

            //Note that you CAN technically modify the pose in IntegrateVelocity. The PoseIntegrator has already integrated the previous velocity into the position, but you can modify it again
            //if you really wanted to.
            //This is also a handy spot to implement things like position dependent gravity or per-body damping.
        }

    }
}
