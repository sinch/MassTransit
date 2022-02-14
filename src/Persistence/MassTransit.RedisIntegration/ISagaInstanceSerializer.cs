namespace MassTransit.RedisIntegration
{
    using Saga;

    public interface ISagaInstanceSerializer
    {
        byte[] Serialize<TSaga>(TSaga instance) where TSaga : class, ISaga;
        TSaga Deserialize<TSaga>(byte[] data) where TSaga : class, ISaga;
    }
}
