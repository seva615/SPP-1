using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;

namespace Trace{
    public interface IThreadNode{
        [JsonIgnore]
        Thread Thread { get; }

        [JsonIgnore]
        TimeSpan DeltaTime { get; }

        [JsonPropertyName("name")]
        string ThreadName => Thread.Name ?? Thread.GetHashCode().ToString();

        [JsonPropertyName("time")]
        string DeltaTimeString => DeltaTime.ToString();

        [JsonPropertyName("methods")]
        IEnumerable<IMethod> Methods { get; }
    }
}
