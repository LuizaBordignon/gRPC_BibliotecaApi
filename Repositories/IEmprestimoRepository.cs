namespace BibliotecaApi.Repositories;

using BibliotecaApi.Domain.Entities;

public interface IEmprestimoRepository
{
    Task<Emprestimo?> GetByIdAsync(int id);
    Task<List<Emprestimo>> GetAllAsync();
    Task<int> ContarAtivosPorUsuarioAsync(string usuarioNome);
    Task AddAsync(Emprestimo emprestimo);
    Task UpdateAsync(Emprestimo emprestimo);
}