USE Trump
GO

EXEC DropView 'BusinessView'
GO
CREATE VIEW BusinessView
AS

SELECT
	b.ID
	, fm.Name FamilyMember
	, fmb.Description  
	, fmcs.Name ConflictStatus
	, ed.Title EthicsDocument
	, ed.Date EthicsDocumentDate
	, ed.Link EthicsDocumentLink
	, b.Name Business
FROM EthicsDocumentBusiness edb
JOIN Business b ON edb.BusinessID = b.ID 	
JOIN EthicsDocument ed ON edb.EthicsDocumentID = ed.ID 
JOIN FamilyMemberBusiness fmb ON edb.BusinessID = fmb.BusinessID
JOIN FamilyMember fm ON fmb.FamilyMemberID = fm.ID 
JOIN FamilyMemberConflictStatus fmcs ON fmb.FamilyMemberConflictStatusID = fmcs.ID 
GO


SELECT Business, Count(*) FROM BusinessView GROUP BY Business HAVING Business LIKE '%Beverage%' 
ORDER BY Count(*) DESC

SELECT * FROM BusinessView WHERE Business = 'Trump Marks Beverages LLC'


SELECT * 
FROM BusinessConflict bc
JOIN BusinessView b ON bc.BusinessID = b.ID
WHERE Business = 'Trump Marks Beverages LLC'
