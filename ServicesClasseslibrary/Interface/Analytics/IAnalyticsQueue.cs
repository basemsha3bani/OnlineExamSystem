using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.Analytics
{
     public interface IAnalyticsQueue
    {
        void Enqueue(AnalyticsJob job);
        bool TryDequeue(out AnalyticsJob job);
    }
}
