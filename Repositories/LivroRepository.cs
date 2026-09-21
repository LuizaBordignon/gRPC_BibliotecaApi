namespace BibliotecaApi.Repositories;

using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Data;

public class LivroRepository : ILivroRepository
{
    private readonly AppDbContext _context;

    public LivroRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Livro?> GetByIdAsync(int id)
        => await _context.Livros.FindAsync(id);

    public async Task<List<Livro>> GetAllAsync()
        => await _context.Livros.ToListAsync();

    public async Task AddAsync(Livro livro)
    {
        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Livro livro)
    {
        _context.Livros.Update(livro);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro != null)
        {
            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();
        }
    }
}