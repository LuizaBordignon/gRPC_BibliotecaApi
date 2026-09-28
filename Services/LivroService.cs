namespace BibliotecaApi.Services;

using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Domain.Exceptions;
using BibliotecaApi.Repositories;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _repository;
    private readonly IEmprestimoRepository _emprestimoRepository;

    public LivroService(ILivroRepository repository, IEmprestimoRepository emprestimoRepository)
    {
        _repository = repository;
        _emprestimoRepository = emprestimoRepository;
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

        var ativos = await _emprestimoRepository.ContarAtivosPorLivroAsync(livro.Id);
        if (livro.ExemplaresTotais < ativos)
            throw new BusinessRuleException($"Livro possui {ativos} empréstimos ativos; o total de exemplares não pode ser menor que isso.");

        existente.Titulo = livro.Titulo;
        existente.Autor = livro.Autor;
        existente.ExemplaresTotais = livro.ExemplaresTotais;
        existente.ExemplaresDisponiveis = livro.ExemplaresTotais - ativos;
        await _repository.UpdateAsync(existente);
    }

    public async Task DeleteAsync(int id)
    {
        var existente = await _repository.GetByIdAsync(id);
        if (existente == null)
            throw new NotFoundException($"Livro {id} não encontrado.");
        await _repository.DeleteAsync(id);
    }
}