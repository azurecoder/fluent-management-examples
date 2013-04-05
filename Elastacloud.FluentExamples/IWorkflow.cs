using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.FluentExamples
{
    public interface IWorkflow
    {
        void PreActionSteps();
        void PostActionSteps();
        void DoWork();
    }
}
