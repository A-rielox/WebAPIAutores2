using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades;

public class Libro
{
    public int Id { get; set; }

    [Required]
    [StringLength(maximumLength: 250)]
    [PrimeraLetraMayuscula]
    public string Titulo { get; set; }

    public List<Comentario> Comentarios { get; set; }
    public List<AutorLibro> AutoresLibros { get; set; }
}
