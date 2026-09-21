namespace BibliotecaApi.Services;

using BibliotecaApi.Domain.Entities;

public interface IEmprestimoService
{
    Task<Emprestimo?> GetByIdAsync(int id);
    Task<List<Emprestimo>> GetAllAsync();
    Task<Emprestimo> EmprestarAsync(int livroId, string usuarioNome);
    Task DevolverAsync(int emprestimoId);
}