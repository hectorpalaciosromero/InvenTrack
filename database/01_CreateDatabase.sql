USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InvenTrackDB')
BEGIN
    CREATE DATABASE InvenTrackDB;
END
GO

USE InvenTrackDB;
GO
