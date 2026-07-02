namespace Catalog.Domain.ValueObjects
{
    public readonly record struct RecetaId(Guid Value)
    {
        public static RecetaId New() => new(Guid.NewGuid());
        public static RecetaId From(Guid value) => new(value);
    }
}
