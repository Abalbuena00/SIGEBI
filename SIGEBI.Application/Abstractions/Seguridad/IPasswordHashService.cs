namespace SIGEBI.Application.Abstractions.Seguridad;

public interface IPasswordHashService
{
    string HashPassword(string password);
}