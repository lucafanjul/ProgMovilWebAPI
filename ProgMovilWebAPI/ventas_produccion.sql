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

