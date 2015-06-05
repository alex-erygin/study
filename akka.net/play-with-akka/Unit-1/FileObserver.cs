using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    ///     Turns <see cref="FileSystemWatcher" /> events about a specific file into messages for <see cref="TailActor" />.
    /// </summary>
    public class FileObserver
    {
        private readonly string _absoluteFilePath;

        private readonly string _fileDir;

        private readonly string _fileNameOnly;
        private readonly ActorRef _tailActor;

        private IDisposable _observable;
        private FileSystemWatcher _watcher;


        public FileObserver(ActorRef tailActor, string absoluteFilePath)
        {
            _tailActor = tailActor;
            _absoluteFilePath = absoluteFilePath;
            _fileDir = Path.GetDirectoryName(_absoluteFilePath);
            _fileNameOnly = Path.GetFileName(_absoluteFilePath);
        }

        public void Start()
        {
            _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly)
                {
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                };

            _observable = new[]
                {
                    Observable.FromEventPattern<FileSystemEventArgs>(_watcher, "Changed")
                              .Select(x => new FileEvent {FileName = x.EventArgs.Name}),
                    Observable.FromEventPattern<ErrorEventArgs>(_watcher, "Error")
                              .Select(x => new FileEvent {ErrorMessage = x.EventArgs.GetException().Message})
                }.Merge()
                 .Finally(_watcher.Dispose)
                 .Subscribe(Observer.Create<FileEvent>(fileEvent =>
                     {
                         if (!string.IsNullOrEmpty(fileEvent.ErrorMessage))
                         {
                             _tailActor.Tell(new TailActor.FileError(_fileNameOnly, fileEvent.ErrorMessage),
                                             ActorRef.NoSender);
                             return;
                         }

                         _tailActor.Tell(new TailActor.FileWrite(fileEvent.FileName), ActorRef.NoSender);
                     }));
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        private class FileEvent
        {
            public string ErrorMessage { get; set; }

            public string FileName { get; set; }
        }
    }
}