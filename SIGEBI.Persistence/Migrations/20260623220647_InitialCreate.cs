using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGEBI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalogo");

            migrationBuilder.EnsureSchema(
                name: "prestamos");

            migrationBuilder.EnsureSchema(
                name: "penalizaciones");

            migrationBuilder.EnsureSchema(
                name: "notificaciones");

            migrationBuilder.EnsureSchema(
                name: "configuracion");

            migrationBuilder.EnsureSchema(
                name: "auditoria");

            migrationBuilder.EnsureSchema(
                name: "seguridad");

            migrationBuilder.CreateTable(
                name: "Autores",
                schema: "catalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                schema: "catalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParametrosSistema",
                schema: "configuracion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoliticasPrestamo",
                schema: "configuracion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoMiembro = table.Column<int>(type: "int", nullable: false),
                    MaximoPrestamosActivos = table.Column<int>(type: "int", nullable: false),
                    DiasDuracionPrestamo = table.Column<int>(type: "int", nullable: false),
                    HorasReservaTemporal = table.Column<int>(type: "int", nullable: false),
                    DiasSuspensionPorDiaRetraso = table.Column<int>(type: "int", nullable: false),
                    PermitePrestamoConVencidos = table.Column<bool>(type: "bit", nullable: false),
                    PenalizaRetraso = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliticasPrestamo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecursosBibliograficos",
                schema: "catalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoInterno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Editorial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AnioPublicacion = table.Column<int>(type: "int", nullable: true),
                    Edicion = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursosBibliograficos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "seguridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "seguridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Matricula = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NumeroEmpleado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ejemplares",
                schema: "catalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecursoBibliograficoId = table.Column<int>(type: "int", nullable: false),
                    CodigoInterno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstadoFisico = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ejemplares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ejemplares_RecursosBibliograficos_RecursoBibliograficoId",
                        column: x => x.RecursoBibliograficoId,
                        principalSchema: "catalogo",
                        principalTable: "RecursosBibliograficos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecursosAutores",
                schema: "catalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecursoBibliograficoId = table.Column<int>(type: "int", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursosAutores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecursosAutores_Autores_AutorId",
                        column: x => x.AutorId,
                        principalSchema: "catalogo",
                        principalTable: "Autores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecursosAutores_RecursosBibliograficos_RecursoBibliograficoId",
                        column: x => x.RecursoBibliograficoId,
                        principalSchema: "catalogo",
                        principalTable: "RecursosBibliograficos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecursosCategorias",
                schema: "catalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecursoBibliograficoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursosCategorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecursosCategorias_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalSchema: "catalogo",
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecursosCategorias_RecursosBibliograficos_RecursoBibliograficoId",
                        column: x => x.RecursoBibliograficoId,
                        principalSchema: "catalogo",
                        principalTable: "RecursosBibliograficos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                schema: "notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioDestinatarioId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntidadReferencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntidadReferenciaId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioDestinatarioId",
                        column: x => x.UsuarioDestinatarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAuditoria",
                schema: "auditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    Modulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EntidadAfectada = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntidadAfectadaId = table.Column<int>(type: "int", nullable: true),
                    Resultado = table.Column<int>(type: "int", nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DireccionIp = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Origen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAuditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosAuditoria_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosRoles",
                schema: "seguridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalSchema: "seguridad",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEstadosEjemplar",
                schema: "catalogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EjemplarId = table.Column<int>(type: "int", nullable: false),
                    EstadoAnterior = table.Column<int>(type: "int", nullable: true),
                    EstadoNuevo = table.Column<int>(type: "int", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioResponsableId = table.Column<int>(type: "int", nullable: true),
                    Motivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEstadosEjemplar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEstadosEjemplar_Ejemplares_EjemplarId",
                        column: x => x.EjemplarId,
                        principalSchema: "catalogo",
                        principalTable: "Ejemplares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesPrestamo",
                schema: "prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EjemplarId = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaExpiracionSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRechazo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCompletada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioAprobadorId = table.Column<int>(type: "int", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesPrestamo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesPrestamo_Ejemplares_EjemplarId",
                        column: x => x.EjemplarId,
                        principalSchema: "catalogo",
                        principalTable: "Ejemplares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesPrestamo_Usuarios_UsuarioAprobadorId",
                        column: x => x.UsuarioAprobadorId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesPrestamo_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prestamos",
                schema: "prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EjemplarId = table.Column<int>(type: "int", nullable: false),
                    SolicitudPrestamoId = table.Column<int>(type: "int", nullable: true),
                    UsuarioBibliotecarioId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaLimiteDevolucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDevolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prestamos_Ejemplares_EjemplarId",
                        column: x => x.EjemplarId,
                        principalSchema: "catalogo",
                        principalTable: "Ejemplares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_SolicitudesPrestamo_SolicitudPrestamoId",
                        column: x => x.SolicitudPrestamoId,
                        principalSchema: "prestamos",
                        principalTable: "SolicitudesPrestamo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_Usuarios_UsuarioBibliotecarioId",
                        column: x => x.UsuarioBibliotecarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservasTemporales",
                schema: "prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudPrestamoId = table.Column<int>(type: "int", nullable: false),
                    EjemplarId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasTemporales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasTemporales_Ejemplares_EjemplarId",
                        column: x => x.EjemplarId,
                        principalSchema: "catalogo",
                        principalTable: "Ejemplares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservasTemporales_SolicitudesPrestamo_SolicitudPrestamoId",
                        column: x => x.SolicitudPrestamoId,
                        principalSchema: "prestamos",
                        principalTable: "SolicitudesPrestamo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Devoluciones",
                schema: "prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrestamoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioBibliotecarioId = table.Column<int>(type: "int", nullable: false),
                    FechaDevolucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FueTardia = table.Column<bool>(type: "bit", nullable: false),
                    DiasRetraso = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devoluciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devoluciones_Prestamos_PrestamoId",
                        column: x => x.PrestamoId,
                        principalSchema: "prestamos",
                        principalTable: "Prestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devoluciones_Usuarios_UsuarioBibliotecarioId",
                        column: x => x.UsuarioBibliotecarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidenciasEjemplar",
                schema: "penalizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EjemplarId = table.Column<int>(type: "int", nullable: false),
                    PrestamoId = table.Column<int>(type: "int", nullable: true),
                    UsuarioReportaId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cerrada = table.Column<bool>(type: "bit", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCierreId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenciasEjemplar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidenciasEjemplar_Ejemplares_EjemplarId",
                        column: x => x.EjemplarId,
                        principalSchema: "catalogo",
                        principalTable: "Ejemplares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciasEjemplar_Prestamos_PrestamoId",
                        column: x => x.PrestamoId,
                        principalSchema: "prestamos",
                        principalTable: "Prestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciasEjemplar_Usuarios_UsuarioCierreId",
                        column: x => x.UsuarioCierreId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciasEjemplar_Usuarios_UsuarioReportaId",
                        column: x => x.UsuarioReportaId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Penalizaciones",
                schema: "penalizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    PrestamoId = table.Column<int>(type: "int", nullable: true),
                    DiasSuspension = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioResolutorId = table.Column<int>(type: "int", nullable: true),
                    MotivoResolucion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penalizaciones_Prestamos_PrestamoId",
                        column: x => x.PrestamoId,
                        principalSchema: "prestamos",
                        principalTable: "Prestamos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalizaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Penalizaciones_Usuarios_UsuarioResolutorId",
                        column: x => x.UsuarioResolutorId,
                        principalSchema: "seguridad",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Autores_Nombre",
                schema: "catalogo",
                table: "Autores",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                schema: "catalogo",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devoluciones_FechaDevolucion",
                schema: "prestamos",
                table: "Devoluciones",
                column: "FechaDevolucion");

            migrationBuilder.CreateIndex(
                name: "IX_Devoluciones_PrestamoId",
                schema: "prestamos",
                table: "Devoluciones",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_Devoluciones_UsuarioBibliotecarioId",
                schema: "prestamos",
                table: "Devoluciones",
                column: "UsuarioBibliotecarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Ejemplares_CodigoInterno",
                schema: "catalogo",
                table: "Ejemplares",
                column: "CodigoInterno",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ejemplares_RecursoBibliograficoId",
                schema: "catalogo",
                table: "Ejemplares",
                column: "RecursoBibliograficoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstadosEjemplar_EjemplarId",
                schema: "catalogo",
                table: "HistorialEstadosEjemplar",
                column: "EjemplarId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasEjemplar_Cerrada",
                schema: "penalizaciones",
                table: "IncidenciasEjemplar",
                column: "Cerrada");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasEjemplar_EjemplarId",
                schema: "penalizaciones",
                table: "IncidenciasEjemplar",
                column: "EjemplarId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasEjemplar_PrestamoId",
                schema: "penalizaciones",
                table: "IncidenciasEjemplar",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasEjemplar_Tipo",
                schema: "penalizaciones",
                table: "IncidenciasEjemplar",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasEjemplar_UsuarioCierreId",
                schema: "penalizaciones",
                table: "IncidenciasEjemplar",
                column: "UsuarioCierreId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasEjemplar_UsuarioReportaId",
                schema: "penalizaciones",
                table: "IncidenciasEjemplar",
                column: "UsuarioReportaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_Estado",
                schema: "notificaciones",
                table: "Notificaciones",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_FechaEnvio",
                schema: "notificaciones",
                table: "Notificaciones",
                column: "FechaEnvio");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioDestinatarioId",
                schema: "notificaciones",
                table: "Notificaciones",
                column: "UsuarioDestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosSistema_Clave",
                schema: "configuracion",
                table: "ParametrosSistema",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_Estado",
                schema: "penalizaciones",
                table: "Penalizaciones",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_FechaFin",
                schema: "penalizaciones",
                table: "Penalizaciones",
                column: "FechaFin");

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_PrestamoId",
                schema: "penalizaciones",
                table: "Penalizaciones",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_UsuarioId",
                schema: "penalizaciones",
                table: "Penalizaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalizaciones_UsuarioResolutorId",
                schema: "penalizaciones",
                table: "Penalizaciones",
                column: "UsuarioResolutorId");

            migrationBuilder.CreateIndex(
                name: "IX_PoliticasPrestamo_TipoMiembro",
                schema: "configuracion",
                table: "PoliticasPrestamo",
                column: "TipoMiembro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_EjemplarId",
                schema: "prestamos",
                table: "Prestamos",
                column: "EjemplarId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_Estado",
                schema: "prestamos",
                table: "Prestamos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_FechaLimiteDevolucion",
                schema: "prestamos",
                table: "Prestamos",
                column: "FechaLimiteDevolucion");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_SolicitudPrestamoId",
                schema: "prestamos",
                table: "Prestamos",
                column: "SolicitudPrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_UsuarioBibliotecarioId",
                schema: "prestamos",
                table: "Prestamos",
                column: "UsuarioBibliotecarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_UsuarioId",
                schema: "prestamos",
                table: "Prestamos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosAutores_AutorId",
                schema: "catalogo",
                table: "RecursosAutores",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosAutores_RecursoBibliograficoId_AutorId",
                schema: "catalogo",
                table: "RecursosAutores",
                columns: new[] { "RecursoBibliograficoId", "AutorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecursosBibliograficos_CodigoInterno",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                column: "CodigoInterno",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecursosBibliograficos_Isbn",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                column: "Isbn",
                unique: true,
                filter: "[Isbn] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosBibliograficos_Titulo",
                schema: "catalogo",
                table: "RecursosBibliograficos",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosCategorias_CategoriaId",
                schema: "catalogo",
                table: "RecursosCategorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosCategorias_RecursoBibliograficoId_CategoriaId",
                schema: "catalogo",
                table: "RecursosCategorias",
                columns: new[] { "RecursoBibliograficoId", "CategoriaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAuditoria_Accion",
                schema: "auditoria",
                table: "RegistrosAuditoria",
                column: "Accion");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAuditoria_FechaRegistro",
                schema: "auditoria",
                table: "RegistrosAuditoria",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAuditoria_Modulo",
                schema: "auditoria",
                table: "RegistrosAuditoria",
                column: "Modulo");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAuditoria_UsuarioId",
                schema: "auditoria",
                table: "RegistrosAuditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_EjemplarId",
                schema: "prestamos",
                table: "ReservasTemporales",
                column: "EjemplarId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_Estado",
                schema: "prestamos",
                table: "ReservasTemporales",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_FechaExpiracion",
                schema: "prestamos",
                table: "ReservasTemporales",
                column: "FechaExpiracion");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_SolicitudPrestamoId",
                schema: "prestamos",
                table: "ReservasTemporales",
                column: "SolicitudPrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                schema: "seguridad",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_EjemplarId",
                schema: "prestamos",
                table: "SolicitudesPrestamo",
                column: "EjemplarId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_Estado",
                schema: "prestamos",
                table: "SolicitudesPrestamo",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_FechaExpiracionSolicitud",
                schema: "prestamos",
                table: "SolicitudesPrestamo",
                column: "FechaExpiracionSolicitud");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_UsuarioAprobadorId",
                schema: "prestamos",
                table: "SolicitudesPrestamo",
                column: "UsuarioAprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesPrestamo_UsuarioId",
                schema: "prestamos",
                table: "SolicitudesPrestamo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                schema: "seguridad",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Matricula",
                schema: "seguridad",
                table: "Usuarios",
                column: "Matricula");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NumeroEmpleado",
                schema: "seguridad",
                table: "Usuarios",
                column: "NumeroEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_RolId",
                schema: "seguridad",
                table: "UsuariosRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_UsuarioId_RolId",
                schema: "seguridad",
                table: "UsuariosRoles",
                columns: new[] { "UsuarioId", "RolId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devoluciones",
                schema: "prestamos");

            migrationBuilder.DropTable(
                name: "HistorialEstadosEjemplar",
                schema: "catalogo");

            migrationBuilder.DropTable(
                name: "IncidenciasEjemplar",
                schema: "penalizaciones");

            migrationBuilder.DropTable(
                name: "Notificaciones",
                schema: "notificaciones");

            migrationBuilder.DropTable(
                name: "ParametrosSistema",
                schema: "configuracion");

            migrationBuilder.DropTable(
                name: "Penalizaciones",
                schema: "penalizaciones");

            migrationBuilder.DropTable(
                name: "PoliticasPrestamo",
                schema: "configuracion");

            migrationBuilder.DropTable(
                name: "RecursosAutores",
                schema: "catalogo");

            migrationBuilder.DropTable(
                name: "RecursosCategorias",
                schema: "catalogo");

            migrationBuilder.DropTable(
                name: "RegistrosAuditoria",
                schema: "auditoria");

            migrationBuilder.DropTable(
                name: "ReservasTemporales",
                schema: "prestamos");

            migrationBuilder.DropTable(
                name: "UsuariosRoles",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Prestamos",
                schema: "prestamos");

            migrationBuilder.DropTable(
                name: "Autores",
                schema: "catalogo");

            migrationBuilder.DropTable(
                name: "Categorias",
                schema: "catalogo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "SolicitudesPrestamo",
                schema: "prestamos");

            migrationBuilder.DropTable(
                name: "Ejemplares",
                schema: "catalogo");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "RecursosBibliograficos",
                schema: "catalogo");
        }
    }
}
