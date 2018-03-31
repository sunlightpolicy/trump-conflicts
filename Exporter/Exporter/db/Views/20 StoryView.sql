USE Trump
GO

EXEC DropView 'StoryView'
GO
CREATE VIEW StoryView
AS
SELECT
	s.ID  
	, mo.Name MediaOutlet
	, s.Date
	, s.Headline 
	, s.Link
	, ss.Name Status
	, su.UserName
	, su.UserEmail
	, su.UserGroup
FROM Story s
JOIN MediaOutlet mo ON s.MediaOutletID = mo.ID
JOIN StoryStatus ss ON s.StoryStatusID = ss.ID
JOIN SystemUserView su ON s.EditorID = su.ID
GO



 
