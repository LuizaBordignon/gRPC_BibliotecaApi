namespace BibliotecaApi.Repositories;

using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Data;

public class EmprestimoRepository : IEmprestimoRepository
{
    private readonly AppDbContext _context;

    public EmprestimoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Emprestimo?> GetByIdAsync(int id)
        => await _context.Emprestimos.FindAsync(id);

    public async Task<List<Emprestimo>> GetAllAsync()
        => await _context.Emprestimos.ToListAsync();

    public async Task<int> ContarAtivosPorUsuarioAsync(string usuarioNome)
        => await _context.Emprestimos
            .CountAsync(e => e.UsuarioNome == usuarioNome && !e.Devolvido);

    public async Task<int> ContarAtivosPorLivroAsync(int livroId)
        => await _context.Emprestimos
            .CountAsync(e => e.LivroId == livroId && !e.Devolvido);

    public async Task AddAsync(Emprestimo emprestimo)
    {
        _context.Emprestimos.Add(emprestimo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Emprestimo emprestimo)
    {
        _context.Emprestimos.Update(emprestimo);
        await _context.SaveChangesAsync();
    }
}