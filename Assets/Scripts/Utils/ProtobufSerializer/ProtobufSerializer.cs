using Google.Protobuf;
using System;
using System.IO;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class ProtobufSerializer
    {
        public static byte[] Serialize<T>(T message) where T : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            using (var memoryStream = new MemoryStream())
            {
                message.WriteTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public static T Deserialize<T>(byte[] data) where T : IMessage<T>, new()
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            var message = new T();
            message.MergeFrom(data);
            return message;
        }
    }

}
