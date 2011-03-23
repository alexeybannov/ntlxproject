using System;

namespace ASC.Core.Caching
{
    class TrustInterval
    {
        private TimeSpan interval;


        public DateTime StartTime
        {
            get;
            private set;
        }

        public bool Started
        {
            get { return interval == default(TimeSpan); }
        }

        public bool Expired
        {
            get { return interval == default(TimeSpan) || interval < (DateTime.UtcNow - StartTime).Duration(); }
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
