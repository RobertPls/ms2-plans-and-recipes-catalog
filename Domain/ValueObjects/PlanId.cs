namespace Catalog.Domain.ValueObjects
{
    public readonly record struct PlanId(Guid Value)
    {
        public static PlanId New() => new(Guid.NewGuid());
        public static PlanId From(Guid value) => new(value);
    }
}
