using Catalog.Shared.Core;

namespace Catalog.Domain.ValueObjects
{
    public record DuracionPlan : ValueObject
    {
        public TipoDuracion Tipo { get; }

        public DuracionPlan(TipoDuracion tipo)
        {
            Tipo = tipo;
            if (!EsValida())
                throw new BussinessRuleValidationException("DuracionPlan must be QUINCENAL or MENSUAL");
        }

        public int Dias() => Tipo switch
        {
            TipoDuracion.QUINCENAL => 15,
            TipoDuracion.MENSUAL => 30,
            _ => throw new BussinessRuleValidationException("Invalid duration type")
        };

        public bool EsValida() => Tipo == TipoDuracion.QUINCENAL || Tipo == TipoDuracion.MENSUAL;
    }
}
