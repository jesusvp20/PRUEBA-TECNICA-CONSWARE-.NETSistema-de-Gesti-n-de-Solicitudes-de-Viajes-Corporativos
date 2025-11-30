-- =====================================================
-- Script: TravelRequestDB
-- Descripción: Base de datos para API de viajes corporativos
-- Autor: jesusvp20
-- Fecha: 2025-11-30
-- =====================================================

-- Crear base de datos (ejecutar primero si no existe)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TravelRequestDB')
BEGIN
    CREATE DATABASE TravelRequestDB;
END
GO

USE TravelRequestDB;
GO

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
CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Correo] nvarchar(100) NOT NULL,
    [Contraseña] nvarchar(max) NOT NULL,
    [Rol] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);

CREATE TABLE [CodigosRecuperacion] (
    [Id] uniqueidentifier NOT NULL,
    [Correo] nvarchar(max) NOT NULL,
    [Codigo] nvarchar(10) NOT NULL,
    [FechaGeneracion] datetime2 NOT NULL,
    [FechaExpiracion] datetime2 NOT NULL,
    [EstaUsado] bit NOT NULL,
    [Activo] bit NOT NULL,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_CodigosRecuperacion] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CodigosRecuperacion_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [SolicitudesViaje] (
    [Id] int NOT NULL IDENTITY,
    [CiudadOrigen] nvarchar(100) NOT NULL,
    [CiudadDestino] nvarchar(100) NOT NULL,
    [FechaIda] datetime2 NOT NULL,
    [FechaRegreso] datetime2 NOT NULL,
    [Justificacion] nvarchar(500) NOT NULL,
    [Estado] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_SolicitudesViaje] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_SolicitudViaje_Fechas] CHECK ([FechaRegreso] > [FechaIda]),
    CONSTRAINT [FK_SolicitudesViaje_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_CodigosRecuperacion_UsuarioId] ON [CodigosRecuperacion] ([UsuarioId]);

CREATE INDEX [IX_SolicitudesViaje_UsuarioId] ON [SolicitudesViaje] ([UsuarioId]);

CREATE UNIQUE INDEX [IX_Usuarios_Correo] ON [Usuarios] ([Correo]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251129232355_InitialCreate', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
DROP INDEX [IX_CodigosRecuperacion_UsuarioId] ON [CodigosRecuperacion];

CREATE INDEX [IX_CodigosRecuperacion_UsuarioId] ON [CodigosRecuperacion] ([UsuarioId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251130002723_FixCodigoRecuperacion', N'10.0.0');

COMMIT;
GO

