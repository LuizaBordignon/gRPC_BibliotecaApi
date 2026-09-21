namespace BibliotecaApi.Services;

using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Domain.Exceptions;
using BibliotecaApi.Repositories;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _repository;

    public LivroService(ILivroRepository repository)
    {
        _repository = repository;
    }

    public Task<Livro?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<List<Livro>> GetAllAsync() => _repository.GetAllAsync();

    public async Task<Livro> CreateAsync(Livro livro)
    {
        livro.ExemplaresDisponiveis = livro.ExemplaresTotais;
        await _repository.AddAsync(livro);
        return livro;
    }

    public async Task UpdateAsync(Livro livro)
    {
        var existente = await _repository.GetByIdAsync(livro.Id);
        if (existente == null)
            throw new NotFoundException($"Livro {livro.Id} não encontrado.");
        await _repository.UpdateAsync(livro);
    }

    public async Task DeleteAsync(int id)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente == null)
            throw new NotFoundException($"Livro {id} não encontrado.");
        await _repository.DeleteAsync(id);
    }
}