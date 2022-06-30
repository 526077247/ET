using System;
using System.Collections.Generic;
#if NOT_UNITY
using System.ComponentModel;
#endif
using System.IO;

namespace ET
{
    public static class ProtobufHelper
    {
	    public static void Init()
        {
        }

        public static object FromBytes(Type type, byte[] bytes, int index, int count)
        {
	        object o = Nino.Serialization.Deserializer.DeserializeWhthoutGenerated(type, bytes,index,count);
	        if (o is ISupportInitialize supportInitialize)
	        {
		        supportInitialize.EndInit();
	        }
	        return o;
        }
        public static byte[] ToBytes(object message)
        {
	        return Nino.Serialization.Serializer.SerializeWithoutGenerated(message.GetType(),message);
        }

        public static void ToStream(object message, MemoryStream stream)
        {
	        var bytes =  Nino.Serialization.Serializer.SerializeWithoutGenerated(message.GetType(),message);
	        stream.Write(bytes,0,bytes.Length);
        }

        public static object FromStream(Type type, MemoryStream stream)
        {
	        var bytes = new byte[stream.Length - stream.Position];
	        stream.Read(bytes, 0, bytes.Length);
	        object o = Nino.Serialization.Deserializer.DeserializeWhthoutGenerated(type, bytes,0,bytes.Length);
	        if (o is ISupportInitialize supportInitialize)
	        {
		        supportInitialize.EndInit();
	        }
	        return o;
        }
    }
}