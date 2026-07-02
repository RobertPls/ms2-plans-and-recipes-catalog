namespace Catalog.Domain.Factory.Receta
{
    public class RecetaFactory : IRecetaFactory
    {
        public Model.Recetas.Receta Create(string nombre, string instrucciones)
        {
            return new Model.Recetas.Receta(nombre, instrucciones);
        }
    }
}
