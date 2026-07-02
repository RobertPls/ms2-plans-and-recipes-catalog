namespace Catalog.Domain.ValueObjects
{
    public readonly record struct AlimentoId(Guid Value)
    {
        public static AlimentoId New() => new(Guid.NewGuid());
        public static AlimentoId From(Guid value) => new(value);
    }
}
