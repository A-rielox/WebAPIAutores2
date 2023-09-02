namespace WebApiAutores.Entidades;

public class Comentario
{
    public int Id { get; set; }
    public string Contenido { get; set; }
    public int LibroId { get; set; } // si se mapea


    public Libro Libro { get; set; }
}
