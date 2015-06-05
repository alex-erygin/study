using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace ConsoleZog
{
    class ObservationOperations
    {
        public void RunDemo()
        {
            new ObservingAGenericIEnumerable().Run();
            
            //new ObservingAnAsynchroniousOperation().Run();
            //new ObserverEvent_Simple().Run();
            //new ObservingEvent_Generic().Run();
        }

        class ObserverEvent_Simple
        {
            public static event EventHandler<EventArgs> SimpleEvent;

            public void Run()
            {
                Console.WriteLine("Setup observable");
                // To consume SimpleEvent as an IObservable:
                IObservable<EventPattern<EventArgs>> eventArgsObservable = Observable.FromEventPattern<EventArgs>(ev => SimpleEvent += ev, ev => SimpleEvent -= ev);

                // SimpleEvent is null until we subscribe
                Console.WriteLine(SimpleEvent == null ? "SimpleEvent == null" : "SimpleEvent != null");
                Console.WriteLine("Subscribe");
                //create two subscribers
                var bob = eventArgsObservable.Subscribe(args => Console.WriteLine("Получили событие от Боба"));
                var alice = eventArgsObservable.Subscribe(args => Console.WriteLine("Получили событие от Алисы"));

                //after subscribing the event has been added
                Console.WriteLine(SimpleEvent == null ? "SimpleEvent == null" : "SimpleEvent != null");

                Console.WriteLine("Raize event");
                if (SimpleEvent != null)
                {
                    SimpleEvent(null, EventArgs.Empty);
                }

                //Allow some time before unsubscribing or event may not happen
                Thread.Sleep(100);

                Console.WriteLine("Unsubscribe");
                bob.Dispose();
                alice.Dispose();

                // After unsubscribing the event handler has been removed
                Console.WriteLine(SimpleEvent == null ? "SimpleEvent == null" : "SimpleEvent != null");

                Console.ReadKey();
            }
        }

        class ObservingEvent_Generic
        {
            public class SomeEventArgs : EventArgs
            {
            }

            public static event EventHandler<SomeEventArgs> GenericEvent;

            public void Run()
            {
                IObservable<EventPattern<SomeEventArgs>> eventAsObservable = Observable.FromEventPattern<SomeEventArgs>(
                    ev => GenericEvent += ev,
                    ev => GenericEvent -= ev);

                using (var hulio = eventAsObservable.Subscribe(x => Console.WriteLine("Пришло " + x.EventArgs.GetType().Name)))
                {
                    GenericEvent(this, new SomeEventArgs());
                }
            }
        }
        
        class ObservingAnAsynchroniousOperation
        {
            public void Run()
            {
                //Wee will use Stream`s BeginRead and EndRead for this sample.
                Stream inputStream = Console.OpenStandardInput();

                /*
                 to convert an asynchronious operation that uses the IAsyncResult pattern to a function that returns an IObservable, use folowwing format.
                 for the generic arguments, specify the type of arguments of Begin* method, up to AsyncCallback.
                 if the * end method returns value, appent this as your final generic argument.
                 */
                var read = Observable.FromAsyncPattern<byte[], int, int, int>(inputStream.BeginRead, inputStream.EndRead);

                byte[] someBytes = new byte[10];
                IObservable<int> observable = read(someBytes, 0, 10);
                observable.Subscribe(x => Console.WriteLine(x));
            }
        }
    
        class ObservingAGenericIEnumerable
        {
            public void Run()
            {
                IEnumerable<int> someInts = new List<int> {1, 2, 3, 4, 5, 6, 7};

                IObservable<int> observable = someInts.ToObservable();


                var subscriber = observable.Subscribe(x => { Thread.Sleep(TimeSpan.FromSeconds(1));
                                                               Console.WriteLine(x);
                });
            }
        }
    
    }
}