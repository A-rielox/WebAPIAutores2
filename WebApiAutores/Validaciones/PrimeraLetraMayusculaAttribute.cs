using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validaciones;

public class PrimeraLetraMayusculaAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return ValidationResult.Success; // si es nulo => hace nada, xq solo valida si hay un string => checa la 1ra letra
        }

        var primeraLetra = value.ToString()[0].ToString();

        if (primeraLetra != primeraLetra.ToUpper())
        {
            return new ValidationResult("La primera letra debe ser mayúscula");
        }

        return ValidationResult.Success;
    }
}
