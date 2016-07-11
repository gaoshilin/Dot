using System;

namespace Dot.Serializing
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);

        object Deserialize(string json, Type type);

        T Deserialize<T>(string json) where T : class;
    }
}