namespace BibliotecaApi.Services;

using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Domain.Exceptions;
using BibliotecaApi.Repositories;

public class EmprestimoService : IEmprestimoService
{
    private readonly IEmprestimoRepository _emprestimoRepository;
    private readonly ILivroRepository _livroRepository;

    public EmprestimoService(IEmprestimoRepository emprestimoRepository, ILivroRepository livroRepository)
    {
        _emprestimoRepository = emprestimoRepository;
        _livroRepository = livroRepository;
    }

    public Task<Emprestimo?> GetByIdAsync(int id) => _emprestimoRepository.GetByIdAsync(id);
    public Task<List<Emprestimo>> GetAllAsync() => _emprestimoRepository.GetAllAsync();

    public async Task<Emprestimo> EmprestarAsync(int livroId, string usuarioNome)
    {
        var livro = await _livroRepository.GetByIdAsync(livroId);
        if (livro == null)
            throw new NotFoundException($"Livro {livroId} não encontrado.");

        if (livro.ExemplaresDisponiveis <= 0)
            throw new BusinessRuleException("Livro sem exemplares disponíveis.");

        var ativos = await _emprestimoRepository.ContarAtivosPorUsuarioAsync(usuarioNome);
        if (ativos >= 3)
            throw new BusinessRuleException("Usuário já possui 3 empréstimos ativos.");

        var emprestimo = new Emprestimo
        {
            LivroId = livroId,
            UsuarioNome = usuarioNome,
            DataEmprestimo = DateTime.UtcNow,
            DataDevolucaoPrevista = DateTime.UtcNow.AddDays(14),
            Devolvido = false
        };

        livro.ExemplaresDisponiveis--;
        await _livroRepository.UpdateAsync(livro);
        await _emprestimoRepository.AddAsync(emprestimo);

        return emprestimo;
    }

    public async Task DevolverAsync(int emprestimoId)
    {
        var emprestimo = await _emprestimoRepository.GetByIdAsync(emprestimoId);
        if (emprestimo == null)
            throw new NotFoundException($"Empréstimo {emprestimoId} não encontrado.");

        if (emprestimo.Devolvido)
            throw new BusinessRuleException("Empréstimo já foi devolvido.");

        emprestimo.Devolvido = true;
        await _emprestimoRepository.UpdateAsync(emprestimo);

        var livro = await _livroRepository.GetByIdAsync(emprestimo.LivroId);
        if (livro != null)
        {
            livro.ExemplaresDisponiveis++;
            await _livroRepository.UpdateAsync(livro);
        }
    }
}