using System;
using Akka.Actor;
using Serilog;

namespace MovieStreaming.Remote
{
    class Program
    {
        private static ActorSystem MovieStreamingActorSystem;
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341")
                .MinimumLevel.Information()
                .CreateLogger();

            Serilog.Log.Logger = logger;

            MovieStreamingActorSystem = ActorSystem.Create("MovieStreamingActorSystem");

            Console.ReadKey();
            MovieStreamingActorSystem.Terminate();
        }
    }
}
