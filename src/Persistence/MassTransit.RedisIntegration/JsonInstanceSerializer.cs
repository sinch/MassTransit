namespace MassTransit.RedisIntegration
{
    using System.Text;
    using Newtonsoft.Json;
    using Saga;
    using Serialization;

    internal class JsonInstanceSerializer : ISagaInstanceSerializer
    {
        public byte[] Serialize<TSaga>(TSaga instance) where TSaga : class, ISaga =>
            Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(instance, typeof(TSaga), JsonMessageSerializer.SerializerSettings));

        public TSaga Deserialize<TSaga>(byte[] data) where TSaga : class, ISaga =>
            JsonConvert.DeserializeObject<TSaga>(Encoding.UTF8.GetString(data), JsonMessageSerializer.DeserializerSettings);
    }
}
