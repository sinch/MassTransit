namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    [Serializable]
    public class RoutingSlipCompletedMessage :
        RoutingSlipCompleted
    {
        public RoutingSlipCompletedMessage()
        {
        }

        public RoutingSlipCompletedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables)
        {
            Duration = duration;
            Timestamp = timestamp;

            TrackingNumber = trackingNumber;
            Variables = variables;
        }

        public Guid TrackingNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public IDictionary<string, object> Variables { get; set; }
    }
}
