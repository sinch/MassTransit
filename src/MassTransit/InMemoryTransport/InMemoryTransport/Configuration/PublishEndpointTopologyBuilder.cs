namespace MassTransit.InMemoryTransport.Configuration
{
    using System;
    using Fabric;


    public class PublishEndpointTopologyBuilder :
        IInMemoryPublishTopologyBuilder
    {
        [Flags]
        public enum Options
        {
            FlattenHierarchy = 0,
            MaintainHierarchy = 1
        }


        readonly IMessageFabric _messageFabric;
        readonly Options _options;

        public PublishEndpointTopologyBuilder(IMessageFabric messageFabric, Options options = Options.MaintainHierarchy)
        {
            _messageFabric = messageFabric;
            _options = options;
        }

        public string ExchangeName { get; set; }

        public IInMemoryPublishTopologyBuilder CreateImplementedBuilder()
        {
            if (_options.HasFlag(Options.MaintainHierarchy))
                return new ImplementedBuilder(this, _options);

            return this;
        }

        public void ExchangeBind(string source, string destination)
        {
            _messageFabric.ExchangeBind(source, destination);
        }

        public void QueueBind(string source, string destination)
        {
            _messageFabric.QueueBind(source, destination);
        }

        public void ExchangeDeclare(string name)
        {
            _messageFabric.ExchangeDeclare(name);
        }

        public void QueueDeclare(string name, int concurrencyLimit)
        {
            _messageFabric.QueueDeclare(name, concurrencyLimit);
        }


        protected class ImplementedBuilder :
            IInMemoryPublishTopologyBuilder
        {
            readonly IInMemoryPublishTopologyBuilder _builder;
            readonly Options _options;
            string _exchangeName;

            public ImplementedBuilder(IInMemoryPublishTopologyBuilder builder, Options options)
            {
                _builder = builder;
                _options = options;
            }

            public string ExchangeName
            {
                get => _exchangeName;
                set
                {
                    _exchangeName = value;
                    if (_builder.ExchangeName != null)
                        _builder.ExchangeBind(_builder.ExchangeName, _exchangeName);
                }
            }

            public void ExchangeBind(string source, string destination)
            {
                _builder.ExchangeBind(source, destination);
            }

            public void QueueBind(string source, string destination)
            {
                _builder.QueueBind(source, destination);
            }

            public void ExchangeDeclare(string name)
            {
                _builder.ExchangeDeclare(name);
            }

            public void QueueDeclare(string name, int concurrencyLimit)
            {
                _builder.QueueDeclare(name, concurrencyLimit);
            }

            public IInMemoryPublishTopologyBuilder CreateImplementedBuilder()
            {
                if (_options.HasFlag(Options.MaintainHierarchy))
                    return new ImplementedBuilder(this, _options);

                return this;
            }
        }
    }
}
