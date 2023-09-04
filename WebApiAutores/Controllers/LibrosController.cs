using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores;
using WebAPIAutores.DTOs;
using AutoMapper;
using WebApiAutores.DTOs;
using Microsoft.AspNetCore.JsonPatch;

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

        if(libro == null) return NotFound();

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

        AsignarOrdenAutores(libro);
        
        context.Add(libro);
        await context.SaveChangesAsync();

        var libroDTO = mapper.Map<LibroDTO>(libro);

        return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
    }


    ////////////////////////////////////
    ///////////////////////////////////////
    [HttpPut("{id:int}")] // id del libro
    public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
    {
        var libro = await context.Libros.Include(l => l.AutoresLibros)
                                        .FirstOrDefaultAsync(l => l.Id == id);

        if (libro == null) return NotFound();

        //                      -------------->
        libro = mapper.Map(libroCreacionDTO, libro);

        AsignarOrdenAutores(libro);

        await context.SaveChangesAsync();

        return NoContent();
    }


    ////////////////////////////////////
    ///////////////////////////////////////
    private void AsignarOrdenAutores(Libro libro)
    {
        if (libro.AutoresLibros != null)
        {
            for (int i = 0; i < libro.AutoresLibros.Count; i++)
            {
                libro.AutoresLibros[i].Orden = i;
            }
        }
    }


    ////////////////////////////////////
    ///////////////////////////////////////
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument) // necesita instalar Microsoft.AspNetCore.Mvc.NewtonsoftJson
    {
        if (patchDocument == null)
        {
            return BadRequest();
        }

        var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

        if (libroDB == null)
        {
            return NotFound();
        }

        var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);

        patchDocument.ApplyTo(libroDTO, ModelState);

        var esValido = TryValidateModel(libroDTO);

        if (!esValido)
        {
            return BadRequest(ModelState);
        }

        mapper.Map(libroDTO, libroDB);

        await context.SaveChangesAsync();
        return NoContent();
    }


    //////////////////////////////////////////
    /////////////////////////////////////////////
    [HttpDelete("{id:int}")] // api/libros/2
    public async Task<ActionResult> Delete(int id)
    {
        var existe = await context.Libros.AnyAsync(x => x.Id == id);

        if (!existe) return NotFound();

        context.Remove(new Libro() { Id = id });
        await context.SaveChangesAsync();

        return NoContent();
    }
}