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
	, b.Name Business
	, ed.Title EthicsDocument
	, ed.Date EthicsDocumentDate
	, ed.Link EthicsDocumentLink	
FROM EthicsDocumentBusiness edb
JOIN Business b ON edb.BusinessID = b.ID 	
JOIN EthicsDocument ed ON edb.EthicsDocumentID = ed.ID 
JOIN FamilyMemberBusiness fmb ON edb.BusinessID = fmb.BusinessID
JOIN FamilyMember fm ON fmb.FamilyMemberID = fm.ID 
JOIN FamilyMemberConflictStatus fmcs ON fmb.FamilyMemberConflictStatusID = fmcs.ID 
--ORDER BY
--	fm.Name
--	, fmb.Description  
--	, fmcs.Name 
--	, b.Name 
--	, ed.Title 
	 
	
GO

--SELECT * FROM BusinessView WHERE Business = 'Trump Marks Beverages LLC'
--SELECT * FROM BusinessView WHERE Business Like '%Beverage%'

