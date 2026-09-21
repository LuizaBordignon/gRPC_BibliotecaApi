namespace BibliotecaApi.Grpc;

using global::Grpc.Core;
using BibliotecaApi.Services;
using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Domain.Exceptions;

public class LivroGrpcService : LivroGrpc.LivroGrpcBase
{
    private readonly ILivroService _service;

    public LivroGrpcService(ILivroService service)
    {
        _service = service;
    }

    public override async Task<LivroResponse> GetLivro(LivroIdRequest request, ServerCallContext context)
    {
        var livro = await _service.GetByIdAsync(request.Id);
        if (livro == null)
            throw new NotFoundException($"Livro {request.Id} não encontrado.");

        return MapToResponse(livro);
    }

    public override async Task<LivroListResponse> ListLivros(LivroEmpty request, ServerCallContext context)
    {
        var livros = await _service.GetAllAsync();
        var response = new LivroListResponse();
        response.Livros.AddRange(livros.Select(MapToResponse));
        return response;
    }

    public override async Task<LivroResponse> CreateLivro(CreateLivroRequest request, ServerCallContext context)
    {
        var livro = new Livro
        {
            Titulo = request.Titulo,
            Autor = request.Autor,
            ExemplaresTotais = request.ExemplaresTotais
        };

        var criado = await _service.CreateAsync(livro);
        return MapToResponse(criado);
    }

    public override async Task<LivroEmpty> UpdateLivro(UpdateLivroRequest request, ServerCallContext context)
    {
        var livro = new Livro
        {
            Id = request.Id,
            Titulo = request.Titulo,
            Autor = request.Autor,
            ExemplaresTotais = request.ExemplaresTotais,
            ExemplaresDisponiveis = request.ExemplaresDisponiveis
        };

        await _service.UpdateAsync(livro);
        return new LivroEmpty();
    }

    public override async Task<LivroEmpty> DeleteLivro(LivroIdRequest request, ServerCallContext context)
    {
        await _service.DeleteAsync(request.Id);
        return new LivroEmpty();
    }

    private static LivroResponse MapToResponse(Livro livro) => new()
    {
        Id = livro.Id,
        Titulo = livro.Titulo,
        Autor = livro.Autor,
        ExemplaresTotais = livro.ExemplaresTotais,
        ExemplaresDisponiveis = livro.ExemplaresDisponiveis
    };
}