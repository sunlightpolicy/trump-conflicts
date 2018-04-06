USE Trump
GO


EXEC DropView 'BusinessConflictView'
GO
CREATE VIEW BusinessConflictView
AS
SELECT
	bc.ConflictID ConflictID 
	, c.Name Conflict
	, c.Description ConflictDescription
	-- Notes, EditDate?
	, b.FamilyMember
	, b.Business
	, b.Description  
	, b.ConflictStatus
	, b.EthicsDocument
	, b.EthicsDocumentDate
	, b.EthicsDocumentLink
FROM BusinessConflict bc		
JOIN BusinessView b ON bc.BusinessID = b.Id 
JOIN Conflict c ON bc.ConflictID = c.Id 



--SELECT * FROM BusinessConflictView
--ORDER BY 
--	ConflictID
--	, FamilyMember
--	, Business
--	, EthicsDocument

	 


