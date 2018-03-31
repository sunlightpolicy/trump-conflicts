USE Trump
GO

EXEC DropView 'SystemUserView'
GO
CREATE VIEW SystemUserView
AS
SELECT
	su.ID 
	, su.LastName + ', ' + su.FirstName UserName
	, su.Email UserEmail
	, ug.Name UserGroup
FROM SystemUser su
JOIN UserGroup ug ON su.UserGroupID = ug.ID
GO

 
