USE [Trump]
GO


EXEC DropView 'SystemUserView'
GO


CREATE VIEW [dbo].[SystemUserView]
AS
SELECT
	su.ID 
	, su.FirstName + ' '  + su.LastName UserName
	, su.LastName + ', ' + su.FirstName LastFirst
	, su.Email UserEmail
	, ug.Name UserGroup
FROM SystemUser su
JOIN UserGroup ug ON su.UserGroupID = ug.ID
GO


UPDATE SystemUser SET FirstName = 'Lynn' WHERE Email = 'lwalsh@sunlightfoundation.com'
UPDATE SystemUser SET LastName = 'Walsh' WHERE Email = 'lwalsh@sunlightfoundation.com'
