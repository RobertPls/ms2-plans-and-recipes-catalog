using Shared.Core;
using Shared.Rules;

namespace Catalog.Domain.ValueObjects
{
    public record AlimentoName : ValueObject
    {
        public string Name { get; init; }

        public AlimentoName(string name)
        {
            CheckRule(new StringNotNullOrEmptyRule(name));
            if (name.Length > 100)
                throw new BussinessRuleValidationException("Alimento name cannot be more than 100 characters");
            Name = name;
        }

        public static implicit operator string(AlimentoName value) => value.Name;

        public static implicit operator AlimentoName(string name) => new(name);
    }
}
