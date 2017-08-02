using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using MovieStreaming.Common.Messages;

namespace MovieStreaming.Common.Actors
{
    public class UserCoordinatorActor : ReceiveActor
    {
        private readonly Dictionary<int, IActorRef> _users;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public UserCoordinatorActor()
        {
            _users = new Dictionary<int, IActorRef>();

            Receive<PlayMovieMessage>(
                message =>
                {
                    CreateChildUserIfNotExists(message.UserId);

                    var childActorRef = _users[message.UserId];

                    childActorRef.Tell(message);
                }
            );

            Receive<StopMovieMessage>(
                message =>
                {
                    CreateChildUserIfNotExists(message.UserId);

                    var chilActorRef = _users[message.UserId];

                    chilActorRef.Tell(message);
                }
            );
        }

        private void CreateChildUserIfNotExists(int userId)
        {
            if (_users.ContainsKey(userId)) return;
            var newChildActorRef =
                Context.ActorOf(Props.Create(() => new UserActor(userId)), "User" + userId);

            _users.Add(userId, newChildActorRef);

            _logger.Debug($"UserCoordinatorActor created new child UserActor for {userId}");
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            _logger.Debug("UserCoordinatorActor PreStart");
        }

        protected override void PostStop()
        {
            _logger.Debug("UserCoordinatorActor PostStop");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _logger.Debug($"UserCoordinatorActor PreRestart because {reason}");

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            _logger.Debug($"UserCoordinatorActor PostRestart because {reason}");

            base.PostRestart(reason);
        }

        #endregion
    }
}
