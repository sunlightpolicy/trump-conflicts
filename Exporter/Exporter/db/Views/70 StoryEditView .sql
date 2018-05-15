USE Trump 
GO


EXEC DropView 'StoryEditView'
GO

CREATE VIEW [dbo].[StoryEditView]
AS
SELECT
	s.Link
	, mo.Name MediaOutlet
	--, s.ID 
	, s.Date
	, s.Headline 
	, ss.Name Status
	, su.LastName + ', ' + su.FirstName EnteredBy
FROM Story s
JOIN MediaOutlet mo ON s.MediaOutletID = mo.ID
JOIN StoryStatus ss ON s.StoryStatusID = ss.ID
JOIN SystemUser su ON s.EditorID = su.ID
GO
