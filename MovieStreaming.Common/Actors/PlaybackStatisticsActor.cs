using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using MovieStreaming.Common.Exceptions;

namespace MovieStreaming.Common.Actors
{
    public class PlaybackStatisticsActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public PlaybackStatisticsActor()
        {
            Context.ActorOf(Props.Create<MoviePlayCounterActor>(), "MoviePlayCounter");

            Context.ActorOf(Context.DI().Props<TrendingMoviesActor>(), "TrendingMovies");
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                exception =>
                {
                    if (exception is ActorInitializationException)
                    {
                        _logger.Error(exception, 
                            $"PlaybackStatisticsActor supervisor strategy stopping child due to ActorInitializationException");

                        return Directive.Stop;
                    }

                    if (exception is SimulatedTerribleMovieException)
                    {
                        var terribleMovieEx = (SimulatedTerribleMovieException) exception;

                        _logger.Warning(
                            $"PlaybackStatisticsActor supervisor strategy resuming child due to terrible movie {terribleMovieEx.MovieTitle}");

                        return Directive.Resume;
                    }

                    _logger.Error(exception,
                        $"PlaybackStatisticsActor supervisor strategy restarting child due to unexpected results");
                    return Directive.Restart;
                }
                );
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            _logger.Debug("PlaybackStatisticsActor PreStart");
        }

        protected override void PostStop()
        {
            _logger.Debug("PlaybackStatisticsActor PostStop");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _logger.Debug($"PlaybackActor PreRestart because: {reason.Message}");

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            _logger.Debug($"PlaybackActor PostRestart because: {reason.Message}");

            base.PostRestart(reason);
        }

        #endregion
    }
}
