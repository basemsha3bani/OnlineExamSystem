using Domain.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.Analytics
{
     public interface IAnalyticsQueue
    {
        void Enqueue(AnalyticsJobDTO job);
        bool TryDequeue(out AnalyticsJobDTO job);
    }
}
