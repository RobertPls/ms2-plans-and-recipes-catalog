namespace Catalog.Domain.Factory.Receta
{
    using Receta = Catalog.Domain.Model.Recetas.Receta;

    public interface IRecetaFactory
    {
        Receta Create(string nombre, string instrucciones);
    }
}
