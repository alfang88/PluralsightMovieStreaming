using System;
using System.Threading;
using Akka.Actor;
using MovieStreaming.Common;
using MovieStreaming.Common.Actors;
using MovieStreaming.Common.Messages;
using Serilog;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using MovieStreaming.Common.Statistics;

namespace MovieStreaming
{
    class Program
    {
        private static ActorSystem MovieStreamingActorSystem;

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SimpleTrendingMovieAnalyzer>().As<ITrendingMovieAnalyzer>();
            builder.RegisterType<TrendingMoviesActor>();

            var container = builder.Build();

            MovieStreamingActorSystem = ActorSystem.Create("MovieStreamingActorSystem");

            IDependencyResolver resolver = new AutoFacDependencyResolver(container, MovieStreamingActorSystem);

            MovieStreamingActorSystem.ActorOf(Props.Create<PlaybackActor>(), "Playback");

            var logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341")
                .MinimumLevel.Information()
                .CreateLogger();

            Serilog.Log.Logger = logger;

            do
            {
                ShortPause();

                Console.WriteLine();
                Console.WriteLine("enter a command and hit enter");

                var command = Console.ReadLine();

                if (command.StartsWith("play"))
                {
                    var userId = int.Parse(command.Split(',')[1]);
                    var movieTitle = command.Split(',')[2];

                    var message = new PlayMovieMessage(movieTitle, userId);
                    MovieStreamingActorSystem.ActorSelection("/user/Playback/UserCoordinator").Tell(message);
                }

                if (command.StartsWith("stop"))
                {
                    var userId = int.Parse(command.Split(',')[1]);

                    var message = new StopMovieMessage(userId);
                    MovieStreamingActorSystem.ActorSelection("/user/Playback/UserCoordinator").Tell(message);
                }

                if (command != "exit") continue;
                MovieStreamingActorSystem.Dispose();
                MovieStreamingActorSystem.Terminate();
                Console.ReadKey();
                Environment.Exit(1);
            } while (true);
        }

        private static void ShortPause()
        {
            Thread.Sleep(450);
        }
    }
}
