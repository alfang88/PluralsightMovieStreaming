using System;
using Akka.Actor;
using Akka.Event;
using MovieStreaming.Common.Messages;

namespace MovieStreaming.Common.Actors
{
    public class PlaybackActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public PlaybackActor()
        {
            Context.ActorOf(Props.Create<UserCoordinatorActor>(), "UserCoordinator");
            Context.ActorOf(Props.Create<PlaybackStatisticsActor>(), "PlaybackStatistics");
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            _logger.Debug("PlaybackActor PreStart");
        }

        protected override void PostStop()
        {
            _logger.Debug("PlaybackActor PostStop");
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
