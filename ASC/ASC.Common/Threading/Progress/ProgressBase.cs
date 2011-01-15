using System;
using System.Runtime.Serialization;

namespace ASC.Common.Threading.Progress
{
    [DataContract(Namespace = "")]
    public abstract class ProgressBase:IProgressItem
    {
        private double _percentage;

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        [DataMember]
        public object Id { get; set; }
        [DataMember]
        public object Status { get; set; }
        [DataMember]
        public object Error { get; set; }

        [DataMember]
        public double Percentage
        {
            get { return Math.Min(100.0,Math.Max(0,_percentage)); }
            set { _percentage = value; }
        }

        [DataMember]
        public virtual bool IsCompleted { get; set; }

        public void RunJob()
        {
            try
            {
                Percentage = 0;
                DoJob();
            }
            catch (Exception e)
            {
                Error = e;
            }
            finally
            {
                Percentage = 100;
                IsCompleted = true;
            }
            
        }

        protected ProgressBase()
        {
            //Random id
            Id = Guid.NewGuid();
        }

        protected void ProgressAdd(double value)
        {
            Percentage += value;
        }

        protected int StepCount { get; set; }

        protected void StepDone()
        {
            if (StepCount>0)
            {
                Percentage += 100.0/StepCount;
            }
        }

        protected abstract void DoJob();
    }
}