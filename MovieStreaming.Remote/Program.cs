using System;
using Akka.Actor;

namespace MovieStreaming.Remote
{
    class Program
    {
        private static ActorSystem MovieStreamingActorSystem;
        static void Main(string[] args)
        {
            MovieStreamingActorSystem = ActorSystem.Create("MovieStreamingActorSystem");

            Console.ReadKey();
            MovieStreamingActorSystem.Terminate();
        }
    }
}
