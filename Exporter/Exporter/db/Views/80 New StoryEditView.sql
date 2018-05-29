USE Trump
GO


EXEC DropView 'StoryEditView'
GO

CREATE VIEW [dbo].[StoryEditView]
AS
SELECT
	s.Link [URL of Story]	
	, s.Headline 
	, mo.Name [Media Outlet]
	, dbo.GetConflictNames(s.ID) Conflicts
	, '' Notes
	, s.Date [Publication Date]

	, ss.Name [Pub Status]
	, su.LastName + ', ' + su.FirstName EnteredBy
FROM Story s
JOIN MediaOutlet mo ON s.MediaOutletID = mo.ID
JOIN StoryStatus ss ON s.StoryStatusID = ss.ID
JOIN SystemUser su ON s.EditorID = su.ID
--ORDER BY Date
GO
