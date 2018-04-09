USE Trump
GO


EXEC DropView 'BusinessOwnershipView'
GO

CREATE VIEW BusinessOwnershipView 
AS

SELECT 
	bo.OwneeID
	, bo.OwnerId
	, b.Name Owner
	, bo.OwnershipPercentage 
FROM BusinessOwnership bo
JOIN Business b ON bo.OwnerID = b.ID
