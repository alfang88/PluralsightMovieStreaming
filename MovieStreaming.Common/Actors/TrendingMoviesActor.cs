using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;
using MovieStreaming.Common.Messages;
using MovieStreaming.Common.Statistics;

namespace MovieStreaming.Common.Actors
{
    public class TrendingMoviesActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly ITrendingMovieAnalyzer _trendAnalyzer;

        private readonly Queue<string> _recentlyPlayedMovies;
        private const int NumberOfRecentMoviesToAnalyze = 5;

        public TrendingMoviesActor()
        {
            _recentlyPlayedMovies = new Queue<string>(NumberOfRecentMoviesToAnalyze);
            _trendAnalyzer = new SimpleTrendingMovieAnalyzer();

            Receive<IncrementPlayCountMessage>(message => HandleIncrementMessage(message));
        }

        private void HandleIncrementMessage(IncrementPlayCountMessage message)
        {
            var recentlyPlayedMoviesBufferIsFull = _recentlyPlayedMovies.Count == NumberOfRecentMoviesToAnalyze;
            if (recentlyPlayedMoviesBufferIsFull)
            {
                // remove oldest movie title
                _recentlyPlayedMovies.Dequeue();
            }

            _recentlyPlayedMovies.Enqueue(message.MovieTitle);

            LogInfo();

            var topMovie = _trendAnalyzer.CalculateMostPopularMovie(_recentlyPlayedMovies);

            _logger.Info($"TrendingMoviesActor most popular movie trending now is {topMovie}");
        }

        private void LogInfo()
        {
            var sb = new StringBuilder();

            sb.AppendLine("TrendingMovieActor Recently played movies:");

            foreach (var movie in _recentlyPlayedMovies)
            {
                sb.AppendLine(movie);
            }

            _logger.Info(sb.ToString());
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            _logger.Debug("TrendingMovieActor PreStart");
        }

        protected override void PostStop()
        {
            _logger.Debug("TrendingMovieActor PostStop");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _logger.Debug($"TrendingMovieActor PreRestart because {reason}");

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            _logger.Debug($"TrendingMovieActor PostRestart because {reason}");

            base.PostRestart(reason);
        }

        #endregion
    }
}
