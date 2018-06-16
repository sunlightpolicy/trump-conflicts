USE [Trump]
GO

EXEC DropView 'StoryConflictView'
GO

CREATE VIEW [dbo].[StoryConflictView]
AS
SELECT	
	c.Id ConflictId
	, s.ID StoryID
	, c.Name Conflict
	, c.Description ConflictDescription
	, ISNULL(c.DateChanged, '2000-01-01') ConflictUpdateDate
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


