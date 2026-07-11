namespace Catalog.Domain.ValueObjects
{
    public enum TipoTiempoComida
    {
        Desayuno = 1,
        MediaManana = 2,
        Almuerzo = 3,
        Merienda = 4,
        Cena = 5
    }

    public static class TipoTiempoComidaExtensions
    {
        public static string ToDisplayName(this TipoTiempoComida tipo) => tipo switch
        {
            TipoTiempoComida.Desayuno => "Desayuno",
            TipoTiempoComida.MediaManana => "Media Manana",
            TipoTiempoComida.Almuerzo => "Almuerzo",
            TipoTiempoComida.Merienda => "Merienda",
            TipoTiempoComida.Cena => "Cena",
            _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null)
        };
    }
}
