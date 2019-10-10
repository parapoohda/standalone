using BepuUtilities;
using DemoContentLoader;
using DemoUtilities;
using OpenTK;
using System.Threading.Tasks;
using System.Threading;
using System;
using MapManagerServer;
using Com.Nelalen.GameObject;

namespace BepuPhysics2Helper
{
    class Program
    {
        //here
        static void Main(string[] args)
        {
            testMapAddCharacter();
            /*if (false)
            {
                Run();
            }
            else {
                var window = new Window("pretty cool multicolored window", new Int2((int)(DisplayDevice.Default.Width * 0.75f), (int)(DisplayDevice.Default.Height * 0.75f)), WindowMode.Windowed);
                var loop = new GameLoop(window);
                ContentArchive content;
                using (var stream = typeof(Program).Assembly.GetManifestResourceStream("bepuphysics2_for_nelalen.Demos.contentarchive"))
                {
                    content = ContentArchive.Load(stream);
                }
                var demo = new DemoHarness(loop, content);

                loop.Run(demo);
                loop.Dispose();
                window.Dispose();
            }*/
            
            
        
        }

        private static void testMapAddCharacter()
        {
            var map = new Map();
            var gate = new Gate();
            var player = new PlayerCharacter(1, 2, "test", 3, gate, map, new System.Numerics.Vector3(0, 1.5f, 0));
            var player2 = new PlayerCharacter(2, 3, "test2", 4, gate, map, new System.Numerics.Vector3(0, 1.5f, 0));
            map.AddPlayerCharacter(player);
            map.AddPlayerCharacter(player2);
        }

        static Bepu bepu;
        public static void Run()
        {
            bepu = new Bepu();
            //The buffer pool is a source of raw memory blobs for the engine to use.
            bepu.Initialize();
            Thread newThread = new Thread(PoolThread);
            newThread.Start();
            //Now take 100 time steps!
            

            //If you intend to reuse the BufferPool, disposing the simulation is a good idea- it returns all the buffers to the pool for reuse.
            //Here, we dispose it, but it's not really required; we immediately thereafter clear the BufferPool of all held memory.
            //Note that failing to dispose buffer pools can result in memory leaks.
        }

        private static void PoolThread()
        {
            while (true)
            {
                bepu.Update();
                Thread.Sleep(15);
            }
            
        }
    }
}