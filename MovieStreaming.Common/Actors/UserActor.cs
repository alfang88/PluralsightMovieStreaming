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
                        "UserActor {User} cannot start playing another movie before stopping existing one",
                        _userId);
                });
            Receive<StopMovieMessage>(message => StopPlayingCurrentMovie());

            _logger.Info("UserActor {User} behaviour has now become Playing", _userId);
        }

        private void Stopped()
        {
            Receive<PlayMovieMessage>(message => StartPlayingMovie(message.MovieTitle));
            Receive<StopMovieMessage>(
                message =>
                {
                    _logger.Warning("UserActor {User} cannot stop if nothing is playing", _userId);
                });

            _logger.Info("UserActor {User} behaviour has now become Stopped", _userId);
        }

        private void StopPlayingCurrentMovie()
        {
            _logger.Info("UserActor {User} has stopped watching {Movie}",
                _userId,
                _currentlyWatching);

            _currentlyWatching = null;

            Become(Stopped);
        }

        private void StartPlayingMovie(string title)
        {
            _currentlyWatching = title;

            _logger.Info("UserActor {User} is currently watching '{Movie}'",
                _userId,
                _currentlyWatching);

            Context.ActorSelection("/user/Playback/PlaybackStatistics/MoviePlayCounter")
                .Tell(new IncrementPlayCountMessage(title));

            Context.ActorSelection("/user/Playback/PlaybackStatistics/TrendingMovies")
                .Tell(new IncrementPlayCountMessage(title));

            Become(Playing);
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            _logger.Debug("UserActor {User} PreStart", _userId);
        }

        protected override void PostStop()
        {
            _logger.Debug("UserActor {User} PostStop", _userId);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _logger.Debug("UserActor {User} PreRestart because: {Reason}", _userId, reason);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            _logger.Debug("UserActor {User} PostRestart because: {Reason}", _userId, reason);

            base.PostRestart(reason);
        }

        #endregion
    }
}
