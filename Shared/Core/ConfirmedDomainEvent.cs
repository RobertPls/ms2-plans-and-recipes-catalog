namespace Catalog.Shared.Core
{
    public class ConfirmedDomainEvent
    {
        public DomainEvent DomainEvent { get; }
        public bool Confirmed { get; private set; }

        public ConfirmedDomainEvent(DomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public void MarkConfirmed()
        {
            Confirmed = true;
        }
    }
}