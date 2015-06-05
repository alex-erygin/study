using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace ConsoleZog
{
    class AsyncBacgroundOperations
    {
        public void RunDemo()
        {
            StartBackgroundWork();

            DoLongRunningOperationAsync("Кот").Subscribe(Console.WriteLine);
            DoLongRunningOperationAsync("Пес").Subscribe(Console.WriteLine);
            DoLongRunningOperationAsync("Хулио").Subscribe(Console.WriteLine);

            ParallelExecution();

            CancellingDemo();

            Console.WriteLine("Main thread Completed");
            Console.ReadKey();
        }

        #region Asynchronous Background Operations

        #region Start - Run Code Asynchronously
        /// <summary>
        /// Usage - просто вызвать метод.
        /// </summary>
        public static async void StartBackgroundWork()
        {
            Console.WriteLine("Show use to start on background thread:");
            var observer = Observable.Start(() =>
                {
                    //This starts on bg thread
                    Console.WriteLine("From background thread. Does not block main thread.");
                    Console.WriteLine("Calculating...");
                    Thread.Sleep(1000);
                    Console.WriteLine("Background work completed.");
                });

            await observer.FirstAsync();
        }

        #endregion

        #region Run a method asynchronously on demand

        /*
        Execute a long-running method asynchronously. 
        The method does not start running until there is a subscriber. 
        The method is started every time the observable is created and subscribed, 
        so there could be more than one running at once.
        */


        /// <summary>
        /// Long running method.
        /// </summary>
        private static string DoLongRunningOperation(string param)
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
            return "Looong " + param + " from " + Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Usage - DoLongRunningOperationAsync("Кот").Subscribe(Console.WriteLine); (Каждый вызов стартует новую операцию, можно вызывать несколько штук и параллельно)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private static IObservable<string> DoLongRunningOperationAsync(string param)
        {
            return
                Observable.Create<string>(
                    o => Observable.ToAsync<string, string>(DoLongRunningOperation)(param).Subscribe(o));
        }

        #endregion

        #region CombineLatest - Parallel Execution
        
        /// <summary>
        /// Merges the specified observable sequences into one observable sequence by emitting a 
        /// list with the latest source elements whenever any of the observable sequences produces an element.
        ///
        /// Usage - просто вызвать.
        /// </summary>
        public static void ParallelExecution()
        {
            var observable = Observable.CombineLatest(
                Observable.Start(() => { Thread.Sleep(1000); Console.WriteLine("Первая таска работает на потоке " + Thread.CurrentThread.ManagedThreadId);}),
                Observable.Start(() => { Thread.Sleep(1000);Console.WriteLine("Вторая таска работает на потоке " + Thread.CurrentThread.ManagedThreadId);}),
                Observable.Start(() => { Thread.Sleep(1000);Console.WriteLine("Третья таска работает на потоке " + Thread.CurrentThread.ManagedThreadId);})
                ).Finally(()=>Console.WriteLine("Готово!"));
        }

        #endregion

        #region Create with disposable & scheduler - Cancelling an asynchronous operation


        /// <summary>
        /// This sample starts a background operation that generates a sequence of integers until it is canceled by the main thread. 
        /// To start the background operation new the Scheduler class is used and a CancellationTokenSource is indirectly created by a Observable.Create.
        /// Please check out the MSDN documentation on System.Threading.CancellationTokenSource to learn more about cancellation source.
        /// 
        /// Just call
        /// </summary>
        public static void CancellingDemo()
        {
            IObservable<int> ob = Observable.Create<int>(o =>
            {
                var cancel = new CancellationDisposable(); // internally creates a new CancellationTokenSource
                NewThreadScheduler.Default.Schedule(() =>
                {
                    int i = 0;
                    for (; ; )
                    {
                        Thread.Sleep(200);  // here we do the long lasting background operation
                        if (!cancel.Token.IsCancellationRequested)    // check cancel token periodically
                            o.OnNext(i++);
                        else
                        {
                            Console.WriteLine("Aborting because cancel event was signaled!");
                            o.OnCompleted();
                            return;
                        }
                    }
                });
                return cancel;
            });

            IDisposable subscription = ob.Subscribe(i => Console.WriteLine(i));
            Console.WriteLine("Press any key to cancel");
            Console.ReadKey();
            subscription.Dispose();
        }

        #endregion

        #endregion
    }
}