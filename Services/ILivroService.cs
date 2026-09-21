namespace BibliotecaApi.Services;

using BibliotecaApi.Domain.Entities;

public interface ILivroService
{
    Task<Livro?> GetByIdAsync(int id);
    Task<List<Livro>> GetAllAsync();
    Task<Livro> CreateAsync(Livro livro);
    Task UpdateAsync(Livro livro);
    Task DeleteAsync(int id);
}