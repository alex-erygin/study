using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class PerformanceCounterActor : UntypedActor
    {
        private readonly string _seriesName;

        private readonly Func<PerformanceCounter> _performanceCounterGenerator;

        private PerformanceCounter _counter;

        private readonly HashSet<ActorRef> _subscriptions;
        private readonly CancellationTokenSource _cancelPublishing;

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
        {
            _seriesName = seriesName;

            _performanceCounterGenerator = performanceCounterGenerator;

            _subscriptions = new HashSet<ActorRef>();

            _cancelPublishing = new CancellationTokenSource();
        }


        #region Actor lifecycle methods

        protected override void PreStart()
        {
            _counter = _performanceCounterGenerator();
            Context.System.Scheduler.Schedule(TimeSpan.FromMilliseconds(250),
                                              TimeSpan.FromMilliseconds(250), Self, new GatherMetrics(),
                                              _cancelPublishing.Token);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelPublishing.Cancel(false);
                _counter.Dispose();
            }
            catch
            {
            }
            finally
            {
                base.PostStop();
            }
        }

        #endregion


        protected override void OnReceive(object message)
        {
            if (message is GatherMetrics)
            {
                var metric = new Metric(_seriesName, _counter.NextValue());
                foreach (var subscriber in _subscriptions)
                {
                    subscriber.Tell(metric);
                }
            }
            else if (message is SubscribeCounter)
            {
                var sc = message as SubscribeCounter;
                _subscriptions.Add(sc.Subscriber);
            }
            else if (message is UnsubscribeCounter)
            {
                var uc = message as UnsubscribeCounter;
                _subscriptions.Remove(uc.Subscriber);
            }
        }
    }
}
