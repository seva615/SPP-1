using System.Collections.Generic;
using System.IO;
using Trace;

namespace TraceResult{
    public interface ITraceResult{
        void Save(Stream output, IEnumerable<IThreadNode> traceResult);
    }
}
