using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Nino.Serialization;

namespace ET
{
    public class NinoRegister
    {
        static NinoRegister()
        {
            Serializer.AddCustomImporter<Entity>((val, writer) =>
            {
                writer.Write(val.ToBson());

            });
            Deserializer.AddCustomExporter<Entity>((reader) =>
            {
                var str = reader.ReadString();
                return BsonSerializer.Deserialize<Entity>(str);
            });
        }
        
        public static void Init()
        {
            
        }
    }
}