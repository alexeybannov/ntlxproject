using System;

namespace ASC.Core.Caching
{
    class TrustInterval
    {
        private TimeSpan interval;


        public DateTime StartTime
        {
            get;
            set;
        }

        public bool Expired
        {
            get { return interval == default(TimeSpan) || interval > (DateTime.UtcNow - StartTime).Duration(); }
        }


        public static TrustInterval StartNew(TimeSpan interval)
        {
            var trustInterval = new TrustInterval();
            trustInterval.Start(interval);
            return trustInterval;
        }

        public void Start(TimeSpan interval)
        {
            this.interval = interval;
            StartTime = DateTime.UtcNow;
        }

        public void Stop()
        {
            interval = default(TimeSpan);
        }
    }
}
