USE [Trump]
GO

EXEC DropView 'StoryConflictView'
GO

CREATE VIEW [dbo].[StoryConflictView]
AS
SELECT DISTINCT	
	c.ID ConflictId
	, c.Name Conflict
	, c.Description ConflictDescription
	--, c.DateChanged ConflictUpdateDate
	--, c.Notes ConflictNotes
	, c.Slug
	--, 'Donald Trump' FamilyMember -- !!
	, fm.Name FamilyMember
	, 'Active' ConflictStatus -- !!
	, s.MediaOutlet
	, s.Date 
	, s.Headline
	, s.Link
	, s.Status
	, ISNULL(ethicsCount, 0) EthicsCount
FROM StoryConflict sc 
JOIN StoryView s ON sc.StoryID = s.ID 
JOIN Conflict c ON sc.ConflictID = c.ID
JOIN BusinessConflict bc ON bc.ConflictID = c.ID
JOIN Business b ON b.ID = bc.BusinessID
JOIN FamilyMemberBusiness fmb ON fmb.BusinessID = b.ID 
JOIN FamilyMember fm ON fmb.FamilyMemberID = fm.ID
LEFT JOIN (SELECT COUNT(*) ethicsCount, ConflictID FROM BusinessConflictView GROUP BY ConflictID) e ON e.ConflictID = c.ID

UNION ALL

SELECT 
	c.ConflictID 
	, conflict.Name
	, conflict.Description
	, conflict.Slug
	, FamilyMember
	, 'Active'
	, s.MediaOutlet
	, s.Date
	, s.Headline
	, s.Link
	, s.Status
	, 0 
FROM ( 
	SELECT ID ConflictID, 'Donald Trump Jr.' FamilyMember FROM Conflict WHERE (Junior = 1)  
	UNION ALL
	SELECT ID, 'Eric Trump' FROM Conflict WHERE (Eric = 1)  
	UNION ALL
	SELECT ID, 'Melania Trump' FROM Conflict WHERE (Melania = 1)
	) c
JOIN Conflict ON c.ConflictID = conflict.ID
JOIN StoryConflict sc ON sc.ConflictID = c.ConflictID
JOIN StoryView s ON s.ID = sc.StoryID
WHERE s.Status = 'Approved'





--WHERE s.Status = 'Approved'
--GO

--SELECT * FROM StoryConflict

--SELECT * FROM StoryConflictView

--SELECT 
--	c.ConflictID 
--	, conflict.Name
--	, conflict.Description
--	, conflict.Slug
--	, FamilyMember
--	, 'Active'
--	, s.MediaOutlet
--	, s.Date
--	, s.Headline
--	, s.Link
--	, s.Status
--	, 0 
--FROM ( 
--	SELECT ID ConflictID, 'Donald Trump Jr.' FamilyMember FROM Conflict WHERE (Junior = 1)  
--	UNION ALL
--	SELECT ID, 'Eric Trump' FROM Conflict WHERE (Eric = 1)  
--	UNION ALL
--	SELECT ID, 'Melania Trump' FROM Conflict WHERE (Melania = 1)
--	) c
--JOIN Conflict ON c.ConflictID = conflict.ID
--JOIN StoryConflict sc ON sc.ConflictID = c.ConflictID
--JOIN StoryView s ON s.ID = sc.StoryID
--WHERE s.Status = 'Approved'

--SELECT * FROM storyConflict

--SELECT * FROM Story WHERE Headline = 'White House says Melania isn’t in business. So why are her companies still active?' 

--DELETE FROM StoryConflict WHERE StoryID = 1133
--DELETE FROM Story WHERE ID = 1133 

 	  




--)



--	SELECT DISTINCT	
--	c.ID ConflictId
--	, c.Name Conflict
--	, c.Description ConflictDescription
--	--, c.DateChanged ConflictUpdateDate
--	--, c.Notes ConflictNotes
--	, c.Slug
--	--, 'Donald Trump' FamilyMember -- !!
--	, fm.Name FamilyMember
--	, 'Active' ConflictStatus -- !!
--	, s.MediaOutlet
--	, s.Date 
--	, s.Headline
--	, s.Link
--	, s.Status
--	, ISNULL(ethicsCount, 0) EthicsCount
--FROM StoryConflict sc 
--JOIN StoryView s ON sc.StoryID = s.ID 
--JOIN Conflict c ON sc.ConflictID = c.ID
--JOIN BusinessConflict bc ON bc.ConflictID = c.ID
--JOIN Business b ON b.ID = bc.BusinessID
--JOIN FamilyMemberBusiness fmb ON fmb.BusinessID = b.ID 
--JOIN FamilyMember fm ON fmb.FamilyMemberID = fm.ID
--LEFT JOIN (SELECT COUNT(*) ethicsCount, ConflictID FROM BusinessConflictView GROUP BY ConflictID) e ON e.ConflictID = c.ID
--WHERE s.Status = 'Approved'

-- 372