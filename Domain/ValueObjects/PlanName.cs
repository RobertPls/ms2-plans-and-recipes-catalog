using Catalog.Shared.Core;
using Catalog.Shared.Rules;

namespace Catalog.Domain.ValueObjects
{
    public record PlanName : ValueObject
    {
        public string Name { get; init; }

        public PlanName(string name)
        {
            CheckRule(new StringNotNullOrEmptyRule(name));
            if (name.Length > 200)
                throw new BussinessRuleValidationException("Plan name cannot be more than 200 characters");
            Name = name;
        }

        public static implicit operator string(PlanName value) => value.Name;

        public static implicit operator PlanName(string name) => new(name);
    }
}
