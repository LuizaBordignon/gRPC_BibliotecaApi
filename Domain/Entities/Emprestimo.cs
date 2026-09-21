namespace BibliotecaApi.Domain.Entities;

public class Emprestimo
{
    public int Id { get; set; }
    public int LivroId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public bool Devolvido { get; set; }
}