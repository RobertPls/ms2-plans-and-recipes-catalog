namespace Catalog.Tests.IntegrationTest.Setup
{
    public class Envelope
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }

    public class AlimentoCreateResponse : Envelope
    {
        public Guid Data { get; set; }
    }

    public class AlimentoData
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Categoria { get; set; } = null!;
    }

    public class AlimentoGetResponse : Envelope
    {
        public AlimentoData? Data { get; set; }
    }

    public class AlimentoPage
    {
        public List<AlimentoData> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class AlimentoListResponse : Envelope
    {
        public AlimentoPage? Data { get; set; }
    }

    public class RecetaCreateResponse : Envelope
    {
        public Guid Data { get; set; }
    }

    public class RecetaData
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class RecetaGetResponse : Envelope
    {
        public RecetaData? Data { get; set; }
    }

    public class RecetaPage
    {
        public List<RecetaData> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class RecetaListResponse : Envelope
    {
        public RecetaPage? Data { get; set; }
    }

    public class PlanCreateResponse : Envelope
    {
        public Guid Data { get; set; }
    }

    public class PlanData
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class PlanListResponse : Envelope
    {
        public List<PlanData>? Data { get; set; }
    }

    public class InfoNutricionalData
    {
        public decimal Cantidad { get; set; }
        public decimal Calorias { get; set; }
        public decimal Proteinas { get; set; }
        public decimal Carbohidratos { get; set; }
        public decimal Grasas { get; set; }
    }

    public class InfoNutricionalResponse : Envelope
    {
        public InfoNutricionalData? Data { get; set; }
    }
}