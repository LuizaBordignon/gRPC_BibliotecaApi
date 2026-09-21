namespace BibliotecaApi.Grpc;

using global::Grpc.Core;
using BibliotecaApi.Services;
using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Domain.Exceptions;

public class EmprestimoGrpcService : EmprestimoGrpc.EmprestimoGrpcBase
{
    private readonly IEmprestimoService _service;

    public EmprestimoGrpcService(IEmprestimoService service)
    {
        _service = service;
    }

    public override async Task<EmprestimoResponse> GetEmprestimo(EmprestimoIdRequest request, ServerCallContext context)
    {
        var emprestimo = await _service.GetByIdAsync(request.Id);
        if (emprestimo == null)
            throw new NotFoundException($"Empréstimo {request.Id} não encontrado.");

        return MapToResponse(emprestimo);
    }

    public override async Task<EmprestimoListResponse> ListEmprestimos(EmprestimoEmpty request, ServerCallContext context)
    {
        var emprestimos = await _service.GetAllAsync();
        var response = new EmprestimoListResponse();
        response.Emprestimos.AddRange(emprestimos.Select(MapToResponse));
        return response;
    }

    public override async Task<EmprestimoResponse> Emprestar(EmprestarRequest request, ServerCallContext context)
    {
        var emprestimo = await _service.EmprestarAsync(request.LivroId, request.UsuarioNome);
        return MapToResponse(emprestimo);
    }

    public override async Task<EmprestimoEmpty> Devolver(EmprestimoIdRequest request, ServerCallContext context)
    {
        await _service.DevolverAsync(request.Id);
        return new EmprestimoEmpty();
    }

    private static EmprestimoResponse MapToResponse(Emprestimo emprestimo) => new()
    {
        Id = emprestimo.Id,
        LivroId = emprestimo.LivroId,
        UsuarioNome = emprestimo.UsuarioNome,
        DataEmprestimo = emprestimo.DataEmprestimo.ToString("o"),
        DataDevolucaoPrevista = emprestimo.DataDevolucaoPrevista.ToString("o"),
        Devolvido = emprestimo.Devolvido
    };
}