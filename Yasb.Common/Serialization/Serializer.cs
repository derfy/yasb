using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Yasb.Common.Messaging;

namespace Yasb.Common.Serialization
{
   
    public class Serializer : ISerializer
    {
        private JsonSerializer _serializer;
        public Serializer(JsonConverter[] jsonConverters)
        {
            _serializer = new JsonSerializer() { NullValueHandling=NullValueHandling.Ignore};
            foreach (var converter in jsonConverters)
            {
                _serializer.Converters.Add(converter);
            }
        }

        /// <summary>
        /// Serializes the given object graph as a byte array.
        /// </summary>
        /// <typeparam name="T">The type of object graph to serialize, inferred by the
        /// compiler from the passed-in <paramref name="graph"/>.</typeparam>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="graph">The object graph to serialize.</param>
        /// <returns>The byte array containing the serialized object graph.</returns>
        public byte[] Serialize<T>(T graph)
        {
            Guard.NotNull(() => graph, graph);

            using (var stream = new MemoryStream())
            {
                Serialize<T>(stream, graph);

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes an object graph of type <typeparamref name="T"/> from 
        /// the given byte array.
        /// </summary>
        /// <typeparam name="T">The type of object graph to deserialize.</typeparam>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="serialized">The serialized byte array.</param>
        public  T Deserialize<T>(byte[] serialized)
        {
            Guard.NotNull(() => serialized, serialized);

            if (serialized.Length == 0)
                return default(T);

            using (var stream = new MemoryStream(serialized))
            {
                return Deserialize<T>(stream);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                var reader = new JsonTextReader(streamReader);
                return _serializer.Deserialize<T>(reader);
            }
        }


        private void Serialize<T>(Stream stream, T graph)
        {
             using (var streamWriter = new StreamWriter(stream, Encoding.Default))
            {
                var writer = new JsonTextWriter(streamWriter);
                _serializer.Serialize(writer, graph);
            }
            
        }

    }
}
