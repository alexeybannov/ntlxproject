using System;
using System.Runtime.Serialization;

namespace ASC.Common.Threading.Progress
{
    public interface IProgressItem:ICloneable
    {
        object Id { get; set; }
        object Status { get; set; }
        object Error { get; set; }
        double Percentage { get; set; }
        bool IsCompleted { get; set; }

        void RunJob();
    }
}