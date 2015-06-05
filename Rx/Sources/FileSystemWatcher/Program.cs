using System;
using System.IO;
using System.Reactive.Linq;

namespace Zog
{
    class Program
    {
        /// <summary>
        /// Смотрит на файловую систему и ловит события ФС, начиная со второго.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var watcher = new FileWatcher("d:\\", "*", TimeSpan.FromSeconds(1));
            watcher.Run()
                   .Skip(1)
                   .Subscribe(eventArg => Console.WriteLine(eventArg.ToString()));

            Console.WriteLine("Press da button to stop it");
            Console.ReadKey();
        }
    }


    public class FileChangedEvent
    {
        public string Name { get; set; }
        
        public string Action { get; set; }

        public override string ToString()
        {
            return Action + " " + Name;
        }
    }

    public class FileWatcher
    {
        private readonly string _path;
        private readonly string _filter;
        private readonly TimeSpan _throttle;

        public FileWatcher(string path, string filter, TimeSpan throttle)
        {
            _path = path;
            _filter = filter;
            _throttle = throttle;
        }

        public IObservable<FileChangedEvent> Run()
        {
            return Observable.Create<FileChangedEvent>(observer =>
                {
                    var watcher = new FileSystemWatcher(_path, _filter)
                        {
                            EnableRaisingEvents = true,
                            IncludeSubdirectories = true
                        };

                    var sources = new[]
                        {
                            Observable.FromEventPattern<FileSystemEventArgs>(watcher, "Created")
                                      .Select(x => new FileChangedEvent {Name = x.EventArgs.Name, Action = "Created"}),
                            Observable.FromEventPattern<FileSystemEventArgs>(watcher, "Changed")
                                      .Select(x => new FileChangedEvent {Name = x.EventArgs.Name, Action = "Changed"}),
                            Observable.FromEventPattern<RenamedEventArgs>(watcher, "Renamed")
                                      .Select(x => new FileChangedEvent {Name = x.EventArgs.Name, Action = "Renamed"}),
                            Observable.FromEventPattern<FileSystemEventArgs>(watcher, "Deleted")
                                      .Select(x => new FileChangedEvent {Name = x.EventArgs.Name, Action = "Deleted"}),
                            Observable.FromEventPattern<ErrorEventArgs>(watcher, "Error")
                                      .Select(
                                          x =>
                                          new FileChangedEvent {Action = "Error " + x.EventArgs.GetException().Message})
                        };

                    return sources
                        .Merge()
                        .Throttle(_throttle)
                        .Finally(watcher.Dispose)
                        .Subscribe(observer);
                });
        }
    }
}