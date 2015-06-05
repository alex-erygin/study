using System.IO;
using Akka.Actor;

namespace WinTail
{
    public class FileValidatorActor : ReceiveActor
    {
        private readonly ActorRef _consoleWriterActor;

        public FileValidatorActor(ActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
            
            Receive<string>(msg =>
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        _consoleWriterActor.Tell(new Messages.NullInputError("Null provided, dawg!!!"));
                        Sender.Tell(new Messages.ContinueProcesing());
                    }
                    else
                    {
                        var valid = IsFileUri(msg);
                        if (valid)
                        {
                            _consoleWriterActor.Tell(new Messages.InputSuccess(string.Format("Starting processing for {0}", msg)));
                            Context.ActorSelection("/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));
                        }
                        else
                        {
                            _consoleWriterActor.Tell(string.Format("{0} is not an existing URI on disk.", msg));
                            Sender.Tell(new Messages.ContinueProcesing());
                        }
                    }
                });

            ReceiveAny(x =>
                {
                    _consoleWriterActor.Tell(new Messages.NullInputError("Null provided, dawg!!!"));
                    Sender.Tell(new Messages.ContinueProcesing());
                });
        }

        /// <summary>
        /// Checks if file exists at path provided by user.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}