using System.Collections.Generic;

namespace Trace
{
    public interface ITracer
    {
        void StartTrace();

        void StopTrace();

        IEnumerable<IThreadNode> GetResult();
    }
}
