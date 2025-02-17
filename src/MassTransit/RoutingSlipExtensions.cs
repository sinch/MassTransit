﻿namespace MassTransit
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;


    public static class RoutingSlipExtensions
    {
        static RoutingSlipExtensions()
        {
            MessageCorrelation.UseCorrelationId<RoutingSlip>(x => x.TrackingNumber);
        }

        /// <summary>
        /// Returns true if there are no remaining activities to be executed
        /// </summary>
        /// <param name="routingSlip"></param>
        /// <returns></returns>
        public static bool RanToCompletion(this RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary == null || routingSlip.Itinerary.Count == 0;
        }

        public static Uri GetNextExecuteAddress(this RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Select(x => x.Address).First();
        }

        public static Uri GetNextCompensateAddress(this RoutingSlip routingSlip)
        {
            return routingSlip.CompensateLogs.Select(x => x.Address).Last();
        }

        public static async Task Execute<T>(this T source, RoutingSlip routingSlip)
            where T : IPublishEndpoint, ISendEndpointProvider
        {
            if (routingSlip.RanToCompletion())
            {
                var timestamp = DateTime.UtcNow;
                var duration = timestamp - routingSlip.CreateTimestamp;

                IRoutingSlipEventPublisher publisher = new RoutingSlipEventPublisher(source, source, routingSlip);

                await publisher.PublishRoutingSlipCompleted(timestamp, duration, routingSlip.Variables).ConfigureAwait(false);
            }
            else
            {
                var endpoint = await source.GetSendEndpoint(routingSlip.GetNextExecuteAddress()).ConfigureAwait(false);

                await endpoint.Send(routingSlip).ConfigureAwait(false);
            }
        }
    }
}
