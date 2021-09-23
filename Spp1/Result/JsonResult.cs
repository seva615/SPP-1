using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Trace;

namespace TraceResult{
    public class JsonResult : ITraceResult{
        public void Save(Stream output, IEnumerable<IThreadNode> traceResult){
            using var writer = new Utf8JsonWriter(output);
            JsonSerializer.Serialize(writer, traceResult);
        }
    }
}
