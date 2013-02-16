using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Yasb.Common.Serialization
{
    public static class SerializationExtensions
    {
        /// <summary>
        /// Deserializes an object graph of type <typeparamref name="T"/> from 
        /// the given byte array.
        /// </summary>
        /// <typeparam name="T">The type of object graph to deserialize.</typeparam>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="serialized">The serialized byte array.</param>
        public static T Deserialize<T>(this ISerializer serializer, byte[] serialized)
        {
            Guard.NotNull(() => serializer, serializer);
            Guard.NotNull(() => serialized, serialized);

            if (serialized.Length == 0)
                return default(T);

            using (var stream = new MemoryStream(serialized))
            {
                return serializer.Deserialize<T>(stream);
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
        public static byte[] Serialize<T>(this ISerializer serializer, T graph)
        {
            Guard.NotNull(() => serializer, serializer);
            Guard.NotNull(() => graph, graph);

            using (var stream = new MemoryStream())
            {
                serializer.Serialize<T>(stream, graph);

                return stream.ToArray();
            }
        }
    }
}
