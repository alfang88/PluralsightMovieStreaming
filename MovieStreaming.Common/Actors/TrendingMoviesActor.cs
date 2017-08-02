using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
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
        }
    }
}
