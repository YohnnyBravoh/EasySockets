using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EasySockets.Serializers
{
    internal sealed class BinarySerializer
    {
        /// <summary>
        /// Serializes an object into a byte array.
        /// </summary>
        /// <param name="data">The object to serialize into a byte array.</param>
        /// <returns></returns>
        public static byte[] Serialize(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            try
            {
                var bf = new BinaryFormatter();

                byte[] serialized;

                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, data);
                    serialized = ms.ToArray();
                }

                return serialized;
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (SecurityException)
            {
                throw;
            }
        }

        /// <summary>
        /// Deserializes a byte array into an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The serialized byte array to deserialize into an object.</param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            try
            {
                var bf = new BinaryFormatter();

                T deserialized;

                using (var ms = new MemoryStream(data))
                {
                    deserialized = (T)bf.Deserialize(ms);
                }

                return deserialized;
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (SecurityException)
            {
                throw;
            }
        }
    }
}
