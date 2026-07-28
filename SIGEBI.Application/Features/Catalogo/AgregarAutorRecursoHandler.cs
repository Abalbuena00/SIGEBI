using SIGEBI.Application.Common;
using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class AgregarAutorRecursoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IAutorRepository _autorRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public AgregarAutorRecursoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IAutorRepository autorRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _autorRepository = autorRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> HandleAsync(
        AgregarAutorRecursoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (command.RecursoBibliograficoId <= 0)
            return ApplicationResult.Failure("El recurso bibliográfico es obligatorio.");

        if (command.AutorId <= 0)
            return ApplicationResult.Failure("El autor es obligatorio.");

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult.Failure("El usuario responsable no se encuentra activo.");

        var recurso = await _recursoBibliograficoRepository.ObtenerParaActualizarRelacionesAsync(
            command.RecursoBibliograficoId,
            cancellationToken);

        if (recurso is null)
            return ApplicationResult.Failure("El recurso bibliográfico no fue encontrado.");

        var autor = await _autorRepository.ObtenerPorIdAsync(
            command.AutorId,
            cancellationToken);

        if (autor is null)
            return ApplicationResult.Failure("El autor no fue encontrado.");

        if (recurso.Autores.Any(recursoAutor => recursoAutor.AutorId == autor.Id))
            return ApplicationResult.Failure("El autor ya está asociado a este recurso bibliográfico.");

        var recursoAutor = new RecursoAutor(
            recurso.Id,
            autor.Id);

        recurso.AgregarAutor(recursoAutor);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Cat\u00E1logo",
            accion: "Agregar autor a recurso bibliogr\u00E1fico",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "RecursoBibliografico",
            entidadAfectadaId: recurso.Id,
            detalle:
                $"Recurso: {recurso.CodigoInterno} - {recurso.Titulo}. " +
                $"Autor agregado: {autor.Nombre}.",
            origen: "Aplicaci\u00F3n institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult.Success();
    }
}
