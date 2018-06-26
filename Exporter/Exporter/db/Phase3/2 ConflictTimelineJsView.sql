USE Trump
GO


EXEC DropView 'ConflictTimelineJsView'
GO

CREATE VIEW [dbo].[ConflictTimelineJsView]
AS

SELECT
	c.ID
	, c.Name Conflict
	, c.Description ConflictDescription
	, m.Name MediaOutlet
	, s.Link
	, s.Headline
	, s.TopImage Image
	, s.Date
	-- No "Group" could be family member?  
FROM 
	Story s
JOIN StoryConflict sc ON sc.StoryID = s.ID
JOIN Conflict c ON sc.ConflictID = c.ID 
JOIN MediaOutlet m ON s.MediaOutletID = m.ID

