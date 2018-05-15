USE Trump
GO

-- These will error the first time - ignore error
CREATE PROCEDURE [dbo].[DropFunction](@Function AS varchar(500)) 
AS
BEGIN
	DECLARE @sql nvarchar(500)
	SET @sql = 'IF OBJECT_ID(''' + @Function + ''', ''FN'') IS NOT NULL BEGIN DROP Function ' + @Function + ' END'
	EXEC sp_executesql @sql
END	

GO


EXEC DropView 'StoryConflictView' 
GO
CREATE VIEW StoryConflictView
AS
SELECT	
	c.Id ConflictId
	, s.ID StoryID
	, c.Name Conflict
	, c.Description ConflictDescription
	, c.DateChanged ConflictUpdateDate
	, c.Notes ConflictNotes
	, 'Donald Trump' FamilyMember -- !!
	, 'Active' ConflictStatus -- !!
	, s.MediaOutlet
	, s.Date 
	, s.Headline
	, s.Link
	, s.Status
	, ISNULL(ethicsCount, 0) EthicsCount
FROM StoryConflict sc 
JOIN StoryView s ON sc.StoryID = s.ID 
JOIN Conflict c ON sc.ConflictID = c.ID
LEFT JOIN (SELECT COUNT(*) ethicsCount, ConflictID FROM BusinessConflictView GROUP BY ConflictID) e ON e.ConflictID = c.ID
GO




CREATE FUNCTION dbo.GetConflictNames(@ID int) 
RETURNS VARCHAR(Max) 
AS
BEGIN 
	DECLARE @ConflictNames VARCHAR(MAX);
	
	SELECT @ConflictNames = COALESCE(@ConflictNames + ', ' ,'') + c.Conflict
	FROM StoryConflictView c 
	WHERE c.StoryID = @ID 
		
	IF @ConflictNames IS NULL BEGIN SET @ConflictNames = '' END
	   
	RETURN @ConflictNames
END;
GO




EXEC DropView 'StoryEditView'
GO

CREATE VIEW [dbo].[StoryEditView]
AS
SELECT
	s.Link
	, mo.Name MediaOutlet
	, dbo.GetConflictNames(s.ID) Conflicts
	, s.Date
	, s.Headline 
	, ss.Name Status
	, su.LastName + ', ' + su.FirstName EnteredBy
FROM Story s
JOIN MediaOutlet mo ON s.MediaOutletID = mo.ID
JOIN StoryStatus ss ON s.StoryStatusID = ss.ID
JOIN SystemUser su ON s.EditorID = su.ID
GO


