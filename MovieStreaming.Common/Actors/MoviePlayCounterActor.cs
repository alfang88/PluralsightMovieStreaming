using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using MovieStreaming.Common.Exceptions;
using MovieStreaming.Common.Messages;

namespace MovieStreaming.Common.Actors
{
    public class MoviePlayCounterActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        private readonly Dictionary<string, int> _moviePlayCounts;

        public MoviePlayCounterActor()
        {
            _moviePlayCounts = new Dictionary<string, int>();

            Receive<IncrementPlayCountMessage>(message => HandleIncrementMessage(message));
        }

        private void HandleIncrementMessage(IncrementPlayCountMessage message)
        {
            if (_moviePlayCounts.ContainsKey(message.MovieTitle))
            {
                _moviePlayCounts[message.MovieTitle]++;
            }
            else
            {
                _moviePlayCounts.Add(message.MovieTitle, 1);
            }

            if (message.MovieTitle == "Partial Recoil")
                throw new SimulatedTerribleMovieException(message.MovieTitle);

            if (message.MovieTitle == "Partial Recoil 2")
                throw new InvalidOperationException("Simulated exception");

            _logger.Info(
                $"MoviePlayCounterActor '{message.MovieTitle}' has been watched {_moviePlayCounts[message.MovieTitle]} times"
            );
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            _logger.Debug("MoviePlayCounterActor PreStart");
        }

        protected override void PostStop()
        {
            _logger.Debug("MoviePlayCounterActor PostStop");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _logger.Debug($"MoviePlayCounterActor PreRestart because: {reason.Message}");

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            _logger.Debug($"MoviePlayCounterActor PostRestart because: {reason.Message}");

            base.PostRestart(reason);
        }

        #endregion
    }


}
