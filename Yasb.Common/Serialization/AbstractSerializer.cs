using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Yasb.Common.Serialization
{
    public abstract class AbstractSerializer 
    {

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
               
                Serialize(stream, graph);
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
        public T Deserialize<T>(byte[] serialized)
        {
            Guard.NotNull(() => serialized, serialized);

            if (serialized.Length == 0)
                return default(T);

            using (var stream = new MemoryStream(serialized))
            {
                return Deserialize<T>(stream);
               
            }
        }

        public abstract T Deserialize<T>(Stream stream);


        public abstract void Serialize<T>(Stream stream, T graph);

    }
}
