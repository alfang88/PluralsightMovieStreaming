using System;
using Akka.Actor;
using Akka.Event;
using MovieStreaming.Common.Messages;

namespace MovieStreaming.Common.Actors
{
    public class UserActor : ReceiveActor
    {
        private string _currentlyWatching;
        private readonly int _userId;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public UserActor(int userId)
        {
            _userId = userId;
            Stopped();
        }

        private void Playing()
        {
            Receive<PlayMovieMessage>(
                message =>
                {
                    _logger.Warning(
                        $"UserActor {_userId} cannot start playing another movie before stopping existing one"
                        );
                });
            Receive<StopMovieMessage>(message => StopPlayingCurrentMovie());

            _logger.Info($"UserActor {_userId} behaviour has now become Playing");
        }

        private void Stopped()
        {
            Receive<PlayMovieMessage>(message => StartPlayingMovie(message.MovieTitle));
            Receive<StopMovieMessage>(
                message =>
                {
                    _logger.Warning($"UserActor {_userId} cannot stop if nothing is playing");
                });

            _logger.Info($"UserActor {_userId} behaviour has now become Stopped");
        }

        private void StopPlayingCurrentMovie()
        {
            _logger.Info($"UserActor {_userId} has stopped watching {_currentlyWatching}");

            _currentlyWatching = null;

            Become(Stopped);
        }

        private void StartPlayingMovie(string title)
        {
            _currentlyWatching = title;

            _logger.Info($"UserActor {_userId} is currently watching '{_currentlyWatching}'");

            Context.ActorSelection("/user/Playback/PlaybackStatistics/MoviePlayCounter")
                .Tell(new IncrementPlayCountMessage(title));

            Context.ActorSelection("/user/Playback/PlaybackStatistics/TrendingMovies")
                .Tell(new IncrementPlayCountMessage(title));

            Become(Playing);
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            _logger.Debug($"UserActor {_userId} PreStart");
        }

        protected override void PostStop()
        {
            _logger.Debug($"UserActor {_userId} PostStop");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _logger.Debug($"UserActor {_userId} PreRestart because: {reason.Message}");

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            _logger.Debug($"UserActor {_userId} PostRestart because: {reason.Message}");

            base.PostRestart(reason);
        }

        #endregion
    }
}
