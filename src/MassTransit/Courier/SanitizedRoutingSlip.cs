namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Internals;
    using Messages;


    /// <summary>
    /// A sanitized routing slip is one that has been read from and ensured to be safe for use, cleaning up any
    /// missing or null properties, as well as making it safe to avoid excessive validity checks across the solution
    /// </summary>
    public class SanitizedRoutingSlip :
        RoutingSlip
    {
        readonly SerializerContext _serializerContext;

        public SanitizedRoutingSlip(ConsumeContext<RoutingSlip> context)
        {
            _serializerContext = context.SerializerContext;

            var routingSlip = context.Message;

            TrackingNumber = routingSlip.TrackingNumber;
            CreateTimestamp = routingSlip.CreateTimestamp;

            Itinerary = (routingSlip.Itinerary ?? Enumerable.Empty<Activity>())
                .Select(x => (Activity)new ActivityImpl(x))
                .ToList();

            ActivityLogs = (routingSlip.ActivityLogs ?? Enumerable.Empty<ActivityLog>())
                .Select(x => (ActivityLog)new ActivityLogImpl(x))
                .ToList();

            CompensateLogs = (routingSlip.CompensateLogs ?? Enumerable.Empty<CompensateLog>())
                .Select(x => (CompensateLog)new CompensateLogImpl(x))
                .ToList();

            Variables = routingSlip.Variables ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            ActivityExceptions = (routingSlip.ActivityExceptions ?? Enumerable.Empty<ActivityException>())
                .Select(x => (ActivityException)new ActivityExceptionImpl(x))
                .ToList();

            Subscriptions = (routingSlip.Subscriptions ?? Enumerable.Empty<Subscription>())
                .Select(x => (Subscription)new SubscriptionImpl(x))
                .ToList();
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime CreateTimestamp { get; private set; }
        public IList<Activity> Itinerary { get; private set; }
        public IList<ActivityLog> ActivityLogs { get; private set; }
        public IList<CompensateLog> CompensateLogs { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
        public IList<ActivityException> ActivityExceptions { get; private set; }
        public IList<Subscription> Subscriptions { get; private set; }

        public T GetActivityArguments<T>()
            where T : class
        {
            try
            {
                if (Itinerary == null)
                    throw new ArgumentException("Itinerary not found in the routing slip");

                if (Itinerary.Count <= 0)
                    throw new ArgumentException("Activity not found in the routing slip");

                var activity = Itinerary[0];

                IDictionary<string, object> argumentsDictionary = Variables.Count > 0
                    ? Variables.MergeLeft(activity.Arguments)
                    : activity.Arguments;

                return _serializerContext.DeserializeObject<T>(argumentsDictionary);
            }
            catch (Exception ex)
            {
                throw new RoutingSlipArgumentException("The activity arguments could not be read", ex);
            }
        }

        public T GetCompensateLogData<T>()
            where T : class
        {
            try
            {
                if (CompensateLogs == null)
                    throw new ArgumentException("CompensateLogs not found in the routing slip");

                if (CompensateLogs.Count <= 0)
                    throw new ArgumentException("CompensateLog not found in the routing slip");

                var compensateLog = CompensateLogs[CompensateLogs.Count - 1];

                IDictionary<string, object> argumentsDictionary = Variables.Count > 0
                    ? Variables.MergeLeft(compensateLog.Data)
                    : compensateLog.Data;

                return _serializerContext.DeserializeObject<T>(argumentsDictionary);
            }
            catch (Exception ex)
            {
                throw new RoutingSlipArgumentException("The compensate log could not be read", ex);
            }
        }
    }
}
