﻿namespace MassTransit
{
    using GrpcTransport.Contracts;
    using GrpcTransport.Topology;


    public interface IGrpcMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IGrpcMessagePublishTopology
        where TMessage : class
    {
        ExchangeType ExchangeType { get; }
    }


    public interface IGrpcMessagePublishTopology
    {
        /// <summary>
        /// Apply the message topology to the builder, including any implemented types
        /// </summary>
        /// <param name="builder">The topology builder</param>
        void Apply(IGrpcPublishTopologyBuilder builder);
    }
}
