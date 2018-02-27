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


GO

