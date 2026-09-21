namespace BibliotecaApi.Data;

using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Livro> Livros => Set<Livro>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();
}