namespace BibliotecaApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Services;

public record EmprestarRequest(int LivroId, string UsuarioNome);

[ApiController]
[Route("api/[controller]")]
public class EmprestimoController : ControllerBase
{
    private readonly IEmprestimoService _service;

    public EmprestimoController(IEmprestimoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Emprestimo>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Emprestimo>> GetById(int id)
    {
        var emprestimo = await _service.GetByIdAsync(id);
        if (emprestimo == null) return NotFound();
        return Ok(emprestimo);
    }

    [HttpPost]
    public async Task<ActionResult<Emprestimo>> Emprestar(EmprestarRequest request)
    {
        var emprestimo = await _service.EmprestarAsync(request.LivroId, request.UsuarioNome);
        return CreatedAtAction(nameof(GetById), new { id = emprestimo.Id }, emprestimo);
    }

    [HttpPost("{id}/devolver")]
    public async Task<IActionResult> Devolver(int id)
    {
        await _service.DevolverAsync(id);
        return NoContent();
    }
}