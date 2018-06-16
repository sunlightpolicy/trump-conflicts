USE Trump
GO


--SELECT * FROM TrumpRussia..TopicView ORDER BY TopicID, Date

--CREATE VIEW [dbo].[TopicView]
--AS

--SELECT
--	t.ID TopicID
--	, t.Name Topic 
--	, s.Description
--	, s.Link
--	, s.Title
--	, s.Date
--	, s.DateDescription
--FROM Story s
--JOIN Topic t ON s.TopicID = t.ID

EXEC DropView 'ConflictTimelineView'
GO

CREATE VIEW [dbo].[ConflictTimelineView]
AS

SELECT
	c.ID
	, c.Name Conflict
	, c.Description ConlfictDescription
	, m.Name MediaOutlet
	, s.Link
	, s.Headline
	, s.Date
FROM 
	Story s
JOIN StoryConflict sc ON sc.StoryID = s.ID
JOIN Conflict c ON sc.ConflictID = c.ID 
JOIN MediaOutlet m ON s.MediaOutletID = m.ID



SELECT * 
FROM ConflictTimelineView
ORDER BY 
	Conflict, MediaOutlet