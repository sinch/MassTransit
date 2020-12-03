﻿namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;


    public class TransportReadyFilter<T> :
        IFilter<T>
        where T : class, PipeContext
    {
        readonly ReceiveEndpointContext _context;

        public TransportReadyFilter(ReceiveEndpointContext context)
        {
            _context = context;
        }

        public async Task Send(T context, IPipe<T> next)
        {
            await _context.TransportObservers.NotifyReady(_context.InputAddress).ConfigureAwait(false);

            var agent = new Agent();
            agent.SetReady();

            _context.AddAgent(agent);

            await next.Send(context).ConfigureAwait(false);

            await agent.Completed.ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("transportReady");
        }
    }
}
