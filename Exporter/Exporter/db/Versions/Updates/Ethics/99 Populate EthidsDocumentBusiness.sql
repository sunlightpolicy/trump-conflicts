USE [Trump]
GO


DELETE FROM EthicsDocumentBusiness
GO

INSERT INTO EthicsDocumentBusiness
SELECT DISTINCT
	1, BusinessID  
FROM FamilyMemberBusiness WHERE FamilyMemberID = (SELECT ID FROM FamilyMember WHERE Name = 'Donald Trump')


INSERT INTO EthicsDocumentBusiness
SELECT DISTINCT
	2, BusinessID  
FROM FamilyMemberBusiness WHERE FamilyMemberID = (SELECT ID FROM FamilyMember WHERE Name = 'Jared Kushner')


INSERT INTO EthicsDocumentBusiness
SELECT DISTINCT
	3, BusinessID  
FROM FamilyMemberBusiness WHERE FamilyMemberID = (SELECT ID FROM FamilyMember WHERE Name = 'Ivanka Trump')


