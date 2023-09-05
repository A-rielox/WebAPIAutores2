using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entidades;

public class Comentario
{
    public int Id { get; set; }
    public string Contenido { get; set; }
    public int LibroId { get; set; } // si se mapea


    public Libro Libro { get; set; }

    // p' relacion usuario-comentario 1-many
    public string UsuarioId { get; set; }
    public IdentityUser Usuario { get; set; }
}
