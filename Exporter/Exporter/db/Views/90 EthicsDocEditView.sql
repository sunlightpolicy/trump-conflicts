USE [Trump]
GO


EXEC DropView 'BusinessOwnershipEditView'
GO


CREATE VIEW [dbo].[BusinessOwnershipEditView] 
AS

WITH Ownerships (OwneeID, Owner, LineNum, OwnershipPercentage)
AS (
SELECT 
	bo.OwneeID
	--, ownee.Name Ownee
	--, bo.OwnerID
	, owner.Name Owner
	, row_number() over(PARTITION BY bo.OwneeID ORDER BY bo.OwneeID) AS LineNum
	, bo.OwnershipPercentage 
FROM BusinessOwnership bo
JOIN Business owner ON bo.OwnerID = owner.ID
JOIN Business ownee ON bo.OwneeID = ownee.ID
--ORDER BY OwneeID
)
SELECT DISTINCT
	o.OwneeID
	, ISNULL(o1.Owner, '') Owner1  
	, ISNULL(o1.OwnershipPercentage , 0) OwnershipPercentage1   
	, ISNULL(o2.Owner, '')  Owner2
	, ISNULL(o2.OwnershipPercentage , 0) OwnershipPercentage2  
	, ISNULL(o3.Owner, '')  Owner3
	, ISNULL(o3.OwnershipPercentage , 0) OwnershipPercentage3 
FROM Ownerships o
LEFT JOIN Ownerships o1 ON o1.OwneeID = o.OwneeID AND o1.LineNum = 1 
LEFT JOIN Ownerships o2 ON o2.OwneeID = o.OwneeID AND o2.LineNum = 2 
LEFT JOIN Ownerships o3 ON o3.OwneeID = o.OwneeID AND o3.LineNum = 3 
GO



EXEC DropView 'BusinessEthicsDocEditView'
GO


CREATE VIEW [dbo].[BusinessEthicsDocEditView] 
AS

WITH Submissions (BusinessID, Link, LineNum)
AS (
SELECT 
	edb.BusinessID
	, ed.Link 
	, row_number() over(PARTITION BY edb.BusinessID ORDER BY EthicsDocumentID) AS LineNum
FROM EthicsDocumentBusiness edb
JOIN EthicsDocument ed ON ed.ID = edb.EthicsDocumentID

)
SELECT DISTINCT
	b.ID
	, ISNULL(s1.Link, '') Doc1  
	, ISNULL(s2.Link, '') Doc2
	, ISNULL(s3.Link, '') Doc3
FROM Business b 
LEFT JOIN Submissions s1 ON s1.BusinessID = b.ID AND s1.LineNum = 1 
LEFT JOIN Submissions s2 ON s2.BusinessID = b.ID AND s2.LineNum = 2 
LEFT JOIN Submissions s3 ON s3.BusinessID = b.ID AND s3.LineNum = 3 
GO


EXEC DropView 'EthicsDocEditView'
GO


CREATE VIEW [dbo].[EthicsDocEditView]
AS

SELECT
	fmb.ID ID
	, ISNULL(owner1, '') Owner1
	, ISNULL(OwnershipPercentage1, '') OwnershipPercentage1 
	, ISNULL(owner2, '') Owner2
	, ISNULL(OwnershipPercentage2, '') OwnershipPercentage2 
	, ISNULL(owner3, '') Owner3
	, ISNULL(OwnershipPercentage3, '') OwnershipPercentage3 
	, fm.Name FamilyMember
	, b.Name Business
	, ISNULL(c.Name, '') Conflict
	, fmb.Description  
	, fmcs.Name ConflictStatus
	, ISNULL(Doc1, '') EthicsDoc1
	, ISNULL(Doc2, '') EthicsDoc2
	, ISNULL(Doc3, '') EthicsDoc3
FROM EthicsDocumentBusiness edb
JOIN Business b ON edb.BusinessID = b.ID 	

--JOIN EthicsDocument ed ON edb.EthicsDocumentID = ed.ID 
JOIN FamilyMemberBusiness fmb ON edb.BusinessID = fmb.BusinessID
JOIN FamilyMember fm ON fmb.FamilyMemberID = fm.ID 
JOIN FamilyMemberConflictStatus fmcs ON fmb.FamilyMemberConflictStatusID = fmcs.ID 
LEFT JOIN BusinessOwnershipEditView bo ON bo.OwneeID = b.ID
LEFT JOIN BusinessConflict bc ON bc.BusinessID = b.ID
LEFT JOIN Conflict c ON bc.ConflictID = c.ID
LEFT JOIN BusinessEthicsDocEditView ed ON ed.ID = b.ID
GO


UPDATE BusinessOwnership SET OwnerID = 1 WHERE ID = 60

DELETE FROM Business WHERE Name = '>'


-- For AirTable

--SELECT Name 
--FROM Business 
--ORDER BY Name -- 1265

--SELECT DISTINCT 
--	ID
--	, Conflict
--	, Business
--	, FamilyMember
--	, Owner1
--	, OwnershipPercentage1
--	, Owner2
--	, OwnershipPercentage2
--	, Owner3
--	, OwnershipPercentage3
--	, '' Owner4
--	, 0 OwnershipPercentage4
--	, Description
--	, ConflictStatus
--	, EthicsDoc1
--	, EthicsDoc2
--	, EThicsDoc3
--FROM EthicsDocEditView 
--ORDER BY FamilyMember, Business -- 1538

