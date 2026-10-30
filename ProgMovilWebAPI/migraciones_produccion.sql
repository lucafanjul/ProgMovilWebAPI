IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260828200515_Inicial'
)
BEGIN
    CREATE TABLE [Categorias] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260828200515_Inicial'
)
BEGIN
    CREATE TABLE [Productos] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NOT NULL,
        [Stock] int NOT NULL,
        [Precio] decimal(18,2) NOT NULL,
        [CategoriaId] int NOT NULL,
        CONSTRAINT [PK_Productos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Productos_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260828200515_Inicial'
)
BEGIN
    CREATE INDEX [IX_Productos_CategoriaId] ON [Productos] ([CategoriaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260828200515_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260828200515_Inicial', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260828203954_PrecisionPrecio'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260828203954_PrecisionPrecio', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260828212901_AgregarEntidadImagenes'
)
BEGIN
    CREATE TABLE [Imagenes] (
        [Id] int NOT NULL IDENTITY,
        [NombreOriginal] nvarchar(max) NOT NULL,
        [NombreArchivo] nvarchar(max) NOT NULL,
        [RutaRelativa] nvarchar(max) NOT NULL,
        [TipoContenido] nvarchar(max) NOT NULL,
        [TamanoBytes] bigint NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [ProductoId] int NOT NULL,
        CONSTRAINT [PK_Imagenes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Imagenes_Productos_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Productos] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260828212901_AgregarEntidadImagenes'
)
BEGIN
    CREATE INDEX [IX_Imagenes_ProductoId] ON [Imagenes] ([ProductoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260828212901_AgregarEntidadImagenes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260828212901_AgregarEntidadImagenes', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907203108_AgregarUsuarios'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(450) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Rol] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907203108_AgregarUsuarios'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Username] ON [Usuarios] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907203108_AgregarUsuarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260907203108_AgregarUsuarios', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908193424_AgregarProveedores'
)
BEGIN
    CREATE TABLE [Proveedores] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Telefono] nvarchar(max) NOT NULL,
        [Direccion] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Proveedores] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908193424_AgregarProveedores'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908193424_AgregarProveedores', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908195058_AgregarClientes'
)
BEGIN
    CREATE TABLE [Clientes] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Telefono] nvarchar(max) NOT NULL,
        [Direccion] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908195058_AgregarClientes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908195058_AgregarClientes', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908200127_AgregarIngresosProductos'
)
BEGIN
    CREATE TABLE [IngresosProductos] (
        [Id] int NOT NULL IDENTITY,
        [ProductoId] int NOT NULL,
        [ProveedorId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        CONSTRAINT [PK_IngresosProductos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_IngresosProductos_Productos_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Productos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_IngresosProductos_Proveedores_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedores] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908200127_AgregarIngresosProductos'
)
BEGIN
    CREATE INDEX [IX_IngresosProductos_ProductoId] ON [IngresosProductos] ([ProductoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908200127_AgregarIngresosProductos'
)
BEGIN
    CREATE INDEX [IX_IngresosProductos_ProveedorId] ON [IngresosProductos] ([ProveedorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908200127_AgregarIngresosProductos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908200127_AgregarIngresosProductos', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908201828_AgregarSalidasProductos'
)
BEGIN
    CREATE TABLE [SalidasProductos] (
        [Id] int NOT NULL IDENTITY,
        [ProductoId] int NOT NULL,
        [ClienteId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        CONSTRAINT [PK_SalidasProductos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SalidasProductos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SalidasProductos_Productos_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Productos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908201828_AgregarSalidasProductos'
)
BEGIN
    CREATE INDEX [IX_SalidasProductos_ClienteId] ON [SalidasProductos] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908201828_AgregarSalidasProductos'
)
BEGIN
    CREATE INDEX [IX_SalidasProductos_ProductoId] ON [SalidasProductos] ([ProductoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908201828_AgregarSalidasProductos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908201828_AgregarSalidasProductos', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915194605_ImagenUnicaPorProducto'
)
BEGIN
    DROP INDEX [IX_Imagenes_ProductoId] ON [Imagenes];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915194605_ImagenUnicaPorProducto'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Imagenes_ProductoId] ON [Imagenes] ([ProductoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260915194605_ImagenUnicaPorProducto'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260915194605_ImagenUnicaPorProducto', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026214708_RelacionarUsuariosConMovimientos'
)
BEGIN
    ALTER TABLE [SalidasProductos] ADD [UsuarioId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026214708_RelacionarUsuariosConMovimientos'
)
BEGIN
    ALTER TABLE [IngresosProductos] ADD [UsuarioId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026214708_RelacionarUsuariosConMovimientos'
)
BEGIN
    CREATE INDEX [IX_SalidasProductos_UsuarioId] ON [SalidasProductos] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026214708_RelacionarUsuariosConMovimientos'
)
BEGIN
    CREATE INDEX [IX_IngresosProductos_UsuarioId] ON [IngresosProductos] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026214708_RelacionarUsuariosConMovimientos'
)
BEGIN
    ALTER TABLE [IngresosProductos] ADD CONSTRAINT [FK_IngresosProductos_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026214708_RelacionarUsuariosConMovimientos'
)
BEGIN
    ALTER TABLE [SalidasProductos] ADD CONSTRAINT [FK_SalidasProductos_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261026214708_RelacionarUsuariosConMovimientos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261026214708_RelacionarUsuariosConMovimientos', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027001749_AgregarImagenUrlAProductos'
)
BEGIN
    ALTER TABLE [Productos] ADD [ImagenUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261027001749_AgregarImagenUrlAProductos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261027001749_AgregarImagenUrlAProductos', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029232043_AgregarSistemaVentas'
)
BEGIN
    CREATE TABLE [Ventas] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Total] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Ventas] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Venta_Total] CHECK ([Total] >= 0),
        CONSTRAINT [FK_Ventas_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029232043_AgregarSistemaVentas'
)
BEGIN
    CREATE TABLE [DetallesVenta] (
        [Id] int NOT NULL IDENTITY,
        [VentaId] int NOT NULL,
        [ProductoId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] decimal(18,2) NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_DetallesVenta] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_DetalleVenta_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [FK_DetallesVenta_Productos_ProductoId] FOREIGN KEY ([ProductoId]) REFERENCES [Productos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetallesVenta_Ventas_VentaId] FOREIGN KEY ([VentaId]) REFERENCES [Ventas] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029232043_AgregarSistemaVentas'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_ProductoId] ON [DetallesVenta] ([ProductoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029232043_AgregarSistemaVentas'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_VentaId] ON [DetallesVenta] ([VentaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029232043_AgregarSistemaVentas'
)
BEGIN
    CREATE INDEX [IX_Ventas_UsuarioId] ON [Ventas] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029232043_AgregarSistemaVentas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261029232043_AgregarSistemaVentas', N'10.0.11');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029235440_RelacionUsuarioCliente'
)
BEGIN
    ALTER TABLE [Clientes] ADD [UsuarioId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029235440_RelacionUsuarioCliente'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Clientes_UsuarioId] ON [Clientes] ([UsuarioId]) WHERE [UsuarioId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029235440_RelacionUsuarioCliente'
)
BEGIN
    ALTER TABLE [Clientes] ADD CONSTRAINT [FK_Clientes_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261029235440_RelacionUsuarioCliente'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261029235440_RelacionUsuarioCliente', N'10.0.11');
END;

COMMIT;
GO

