namespace Catalog.Shared.Core
{
    public abstract record IntegrationEvent
    {
        public DateTime OccurredOn { get; }
        public Guid EventId { get; }

        protected IntegrationEvent()
        {
            OccurredOn = DateTime.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}