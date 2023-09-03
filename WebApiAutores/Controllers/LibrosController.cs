using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores;
using WebAPIAutores.DTOs;
using AutoMapper;
using WebApiAutores.DTOs;

namespace WebAPIAutores.Controllers;

[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public LibrosController(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }



    ////////////////////////////////////
    ///////////////////////////////////////
    [HttpGet("{id:int}", Name = "obtenerLibro")]
    public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
    {
        var libro = await context.Libros.Include(l => l.AutoresLibros)
                                        .ThenInclude(al => al.Autor)
                                        .FirstOrDefaultAsync(x => x.Id == id);

        libro.AutoresLibros = libro.AutoresLibros.OrderBy(al => al.Orden).ToList();

        return mapper.Map<LibroDTOConAutores>(libro);
    }



    ////////////////////////////////////
    ///////////////////////////////////////
    [HttpPost]
    public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
    {
        if (libroCreacionDTO.AutoresIds == null)
        {
            return BadRequest("No se puede crear un libro sin autores");
        }

        var autoresIds = await context.Autores.Where(a => libroCreacionDTO.AutoresIds.Contains(a.Id))
                                              .Select(a => a.Id)
                                              .ToListAsync();

        if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
        {
            return BadRequest("No existe uno de los autores enviados");
        }

        var libro = mapper.Map<Libro>(libroCreacionDTO);

        if (libro.AutoresLibros != null)
        {
            for (int i = 0; i < libro.AutoresLibros.Count; i++)
            {
                libro.AutoresLibros[i].Orden = i;
            }
        }

        context.Add(libro);
        await context.SaveChangesAsync();

        var libroDTO = mapper.Map<LibroDTO>(libro);

        return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
    }
}