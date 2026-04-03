namespace InovaBank.Domain.Primitives;

public sealed record PagedResult<T>(
    IEnumerable<T> Items,
    int Pagina,
    int TamanhoPagina,
    long TotalRegistros)
{
    public int TotalPaginas => (int)Math.Ceiling(TotalRegistros / (double)TamanhoPagina);
    public bool TemProximaPagina => Pagina < TotalPaginas;
    public bool TemPaginaAnterior => Pagina > 1;
}
