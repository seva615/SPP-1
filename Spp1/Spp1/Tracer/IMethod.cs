using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Trace{
    public interface IMethod{
        [JsonIgnore]
        Type Class { get; }

        [JsonIgnore]
        MethodBase MethodBase { get; }

        [JsonIgnore]
        TimeSpan DeltaTime { get; }

        [JsonPropertyName("class")]
        string ClassName => Class.Name;

        [JsonPropertyName("name")]
        string MethodName => MethodBase.Name;

        [JsonPropertyName("time")]
        string DeltaTimeString => DeltaTime.ToString();

        [JsonPropertyName("methods")]
        IEnumerable<IMethod> Methods { get; }
    }
}
