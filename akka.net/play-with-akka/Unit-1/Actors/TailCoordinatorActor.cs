using System;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor : ReceiveActor
    {
        #region Messages

        /// <summary>
        /// Starting tailing the file at user-specified path.
        /// </summary>
        public class StartTail
        {
            public StartTail(string filePath, ActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }

            public string FilePath { get; private set; }

            public ActorRef ReporterActor { get; private set; }
        }

        /// <summary>
        /// Stp tailing da file at user-specified path
        /// </summary>
        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; private set; }
        }


        #endregion

        public TailCoordinatorActor()
        {
            Receive<StartTail>(msg => Context.ActorOf(Props.Create(() => new TailActor(msg.ReporterActor, msg.FilePath))));
        }

        // TailCoordinatorActor.cs
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, // maxNumberOfRetries
                TimeSpan.FromSeconds(30), // duration
                decider: x =>
                {
                    //Maybe we consider ArithmeticException to not be application critical
                    //so we just ignore the error and keep going.
                    if (x is ArithmeticException) return Directive.Resume;

                    //Error that we cannot recover from, stop the failing actor
                    else if (x is NotSupportedException) return Directive.Stop;

                    //In all other cases, just restart the failing actor
                    else return Directive.Restart;
                });
        }
    }
}
