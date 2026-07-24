using SIGEBI.Application.Abstractions.Auditoria;
using SIGEBI.Application.Common;
using SIGEBI.Domain.Entities.Catalogo;
using SIGEBI.Domain.Enums;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Features.Catalogo;

public sealed class CrearRecursoBibliograficoHandler
{
    private readonly IRecursoBibliograficoRepository _recursoBibliograficoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUnitOfWork _unitOfWork;

    public CrearRecursoBibliograficoHandler(
        IRecursoBibliograficoRepository recursoBibliograficoRepository,
        IUsuarioRepository usuarioRepository,
        IAuditoriaService auditoriaService,
        IUnitOfWork unitOfWork)
    {
        _recursoBibliograficoRepository = recursoBibliograficoRepository;
        _usuarioRepository = usuarioRepository;
        _auditoriaService = auditoriaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<int>> HandleAsync(
        CrearRecursoBibliograficoCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validacion = ValidarCommand(command);

        if (!validacion.IsSuccess)
            return ApplicationResult<int>.Failure(validacion.Error!);

        var usuarioResponsable = await _usuarioRepository.ObtenerPorIdAsync(
            command.UsuarioResponsableId,
            cancellationToken);

        if (usuarioResponsable is null)
            return ApplicationResult<int>.Failure("El usuario responsable no fue encontrado.");

        if (usuarioResponsable.Estado != EstadoUsuario.Activo)
            return ApplicationResult<int>.Failure("El usuario responsable no se encuentra activo.");

        var recursoExistentePorCodigo = await _recursoBibliograficoRepository.ObtenerPorCodigoInternoAsync(
            command.CodigoInterno,
            cancellationToken);

        if (recursoExistentePorCodigo is not null)
            return ApplicationResult<int>.Failure("Ya existe un recurso bibliográfico con el mismo código interno.");

        if (!string.IsNullOrWhiteSpace(command.Isbn))
        {
            var recursoExistentePorIsbn = await _recursoBibliograficoRepository.ObtenerPorIsbnAsync(
                command.Isbn,
                cancellationToken);

            if (recursoExistentePorIsbn is not null)
                return ApplicationResult<int>.Failure("Ya existe un recurso bibliográfico con el mismo ISBN.");
        }

        var recurso = new RecursoBibliografico(
            command.CodigoInterno,
            command.Titulo,
            command.Isbn,
            command.Editorial,
            command.AnioPublicacion,
            command.Edicion);

        await _recursoBibliograficoRepository.AgregarAsync(
            recurso,
            cancellationToken);

        await _auditoriaService.RegistrarAsync(
            usuarioId: command.UsuarioResponsableId,
            modulo: "Catálogo",
            accion: "Crear recurso bibliográfico",
            resultado: ResultadoAuditoria.Exitoso,
            entidadAfectada: "RecursoBibliografico",
            entidadAfectadaId: null,
            detalle: $"Se creó el recurso bibliográfico {recurso.CodigoInterno} - {recurso.Titulo}.",
            origen: "Aplicación institucional",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<int>.Success(recurso.Id);
    }

    private static ApplicationResult ValidarCommand(
        CrearRecursoBibliograficoCommand command)
    {
        if (command.UsuarioResponsableId <= 0)
            return ApplicationResult.Failure("El usuario responsable es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.CodigoInterno))
            return ApplicationResult.Failure("El código interno es obligatorio.");

        if (string.IsNullOrWhiteSpace(command.Titulo))
            return ApplicationResult.Failure("El título es obligatorio.");

        return ApplicationResult.Success();
    }
}