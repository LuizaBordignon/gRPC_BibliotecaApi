namespace BibliotecaApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Services;

[ApiController]
[Route("api/[controller]")]
public class LivroController : ControllerBase
{
    private readonly ILivroService _service;

    public LivroController(ILivroService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Livro>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Livro>> GetById(int id)
    {
        var livro = await _service.GetByIdAsync(id);
        if (livro == null) return NotFound();
        return Ok(livro);
    }

    [HttpPost]
    public async Task<ActionResult<Livro>> Create(Livro livro)
    {
        var criado = await _service.CreateAsync(livro);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Livro livro)
    {
        livro.Id = id;
        await _service.UpdateAsync(livro);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}