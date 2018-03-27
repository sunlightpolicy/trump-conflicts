	

-- Make a directory c:\TrumpDb first to hold the db files, otherwise nothing will work 


IF EXISTS (SELECT * FROM sys.databases WHERE name = 'Trump')
BEGIN 
	--ALTER DATABASE Trump SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	ALTER DATABASE Trump SET OFFLINE
	EXEC sp_detach_db 'Trump', 'true'
	DROP DATABASE Trump
END


CREATE DATABASE Trump
 CONTAINMENT = NONE
 ON  PRIMARY
( NAME = N'Trump', FILENAME = N'C:\TrumpDb\Trump.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON
( NAME = N'Trump_log', FILENAME = N'C:\TrumpDb\Trump_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

USE Trump
GO

CREATE PROCEDURE [dbo].[DropTable](@table AS varchar(500)) 
AS
BEGIN
	DECLARE @sql nvarchar(500)
	SET @sql = 'IF OBJECT_ID(''' + @table + ''', ''U'') IS NOT NULL BEGIN DROP TABLE ' + @table + ' END'
	EXEC sp_executesql @sql
END	

GO	
	
CREATE PROCEDURE [dbo].[DropView](@view AS varchar(500)) 
AS
BEGIN
	DECLARE @sql nvarchar(500)
	SET @sql = 'IF OBJECT_ID(''' + @view + ''', ''V'') IS NOT NULL BEGIN DROP VIEW ' + @view + ' END'
	EXEC sp_executesql @sql
END	

