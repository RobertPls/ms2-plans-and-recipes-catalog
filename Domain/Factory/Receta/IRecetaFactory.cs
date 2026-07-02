namespace Catalog.Domain.Factory.Receta
{
    public interface IRecetaFactory
    {
        Model.Recetas.Receta Create(string nombre, string instrucciones);
    }
}
