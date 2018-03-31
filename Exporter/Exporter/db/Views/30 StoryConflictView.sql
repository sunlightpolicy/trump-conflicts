USE Trump
GO


EXEC DropView 'StoryConflictView' 
GO
CREATE VIEW StoryConflictView
AS

SELECT	
	c.Name Conflict
	, c.Description ConflictDescription
	, c.DateChanged ConflictUpdateDate
	, c.Notes ConflictNotes
	, 'Donald Trump' FamilyMember -- !!
	, s.MediaOutlet
	, s.Date 
	, s.Headline
	, s.Link
	, s.Status
FROM StoryConflict sc 
JOIN StoryView s ON sc.StoryID = s.ID 
JOIN Conflict c ON sc.ConflictID = c.ID
GO


