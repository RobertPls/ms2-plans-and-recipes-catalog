namespace Catalog.Domain.Factory.Receta
{
    using Receta = Catalog.Domain.Model.Recetas.Receta;

    public class RecetaFactory : IRecetaFactory
    {
        public Receta Create(string nombre, string instrucciones)
        {
            return new Receta(nombre, instrucciones);
        }
    }
}
