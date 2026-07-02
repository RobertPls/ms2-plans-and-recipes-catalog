using Catalog.Shared.Core;
using Catalog.Shared.Rules;

namespace Catalog.Domain.ValueObjects
{
    public record CategoriaName : ValueObject
    {
        public string Name { get; init; }

        public CategoriaName(string name)
        {
            CheckRule(new StringNotNullOrEmptyRule(name));
            if (name.Length > 50)
                throw new BussinessRuleValidationException("Categoria name cannot be more than 50 characters");
            Name = name;
        }

        public static implicit operator string(CategoriaName value) => value.Name;

        public static implicit operator CategoriaName(string name) => new(name);
    }
}
