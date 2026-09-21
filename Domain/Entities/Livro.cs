namespace BibliotecaApi.Domain.Entities;

public class Livro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int ExemplaresTotais { get; set; }
    public int ExemplaresDisponiveis { get; set; }
}