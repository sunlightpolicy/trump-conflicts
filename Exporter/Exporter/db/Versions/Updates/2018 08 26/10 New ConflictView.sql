USE Trump 
GO

EXEC DropView 'ConflictView'
GO 

CREATE VIEW [dbo].[ConflictView] 
AS

WITH StoryCount (ConflictID, Stories) AS (
	SELECT ConflictID, Count(*) stories 
	FROM StoryView s 
	JOIN StoryConflict sc ON sc.StoryID = s.ID
	WHERE s.Status = 'Approved' 
	GROUP BY sc.ConflictID 
)

, LastStory (ConflictID, Last) AS (
	SELECT ConflictID, CONVERT(VARCHAR, MAX(Date), 107) Last 
	FROM StoryView s 
	JOIN StoryConflict sc ON sc.StoryID = s.ID
	WHERE s.Status = 'Approved' 
	GROUP BY sc.ConflictID 
)

, FamMember (ConflictID, FamilyMember) AS (
	SELECT bc.ConflictId, fm.Name FamilyMember 
	FROM BusinessConflict bc 
	JOIN Business b ON b.ID = bc.BusinessID
	JOIN FamilyMemberBusiness fmb ON fmb.BusinessID = b.ID 
	JOIN FamilyMember fm ON fmb.FamilyMemberID = fm.ID
)

-- Only Include conflicts that are linked to either a story or a conflicting entity
, LinkedConflict (ID) AS (
	SELECT ID FROM Conflict 
	WHERE 
		(ID IN (SELECT ConflictID FROM StoryConflict)) OR 
		(ID IN (SELECT ConflictID FROM BusinessConflict))
) 

SELECT DISTINCT
	c.Name Conflict
	, c.Slug
	
	, CASE CHARINDEX('.', c.Description)  -- determine if the sentence contains a full stop
		WHEN 0 THEN c.Description       -- if not return the whole sentence
		ELSE SUBSTRING(c.Description, 1, CHARINDEX('.', c.Description))  -- else first part
	  END AS ShortDescription
	, c.Description

	, ISNULL(sc.Stories, 0) Stories
	, ISNULL(ls.Last, '') LastStory
	, ISNULL(fm.FamilyMember, '') FamilyMember
	, cs.Name Status
	, cps.Name PublicationStatus

FROM Conflict c
JOIN LinkedConflict lc ON c.ID = lc.ID
JOIN ConflictStatus cs ON cs.ID = c.ConflictStatusID
JOIN ConflictPublicationStatus cps ON cps.ID = c.ConflictPublicationStatusID
LEFT JOIN StoryCount sc ON c.ID = sc.ConflictID 
LEFT JOIN LastStory ls ON c.ID = ls.ConflictID 
LEFT JOIN FamMember fm ON c.ID = fm.ConflictID 
WHERE c.ID <> 1
AND FamilyMember <> ''
AND cps.Name = 'Approved'
GO

--SELECT * FROM Conflict WHERE name = 'Alabama Public Schools' 


--SELECT * FROM ConflictPublicationStatus 



--SELECT ID, Name  FROM Conflict 
--WHERE 
--	(ID NOT IN (SELECT ConflictID FROM StoryConflict)) AND 
--	(ID NOT IN (SELECT ConflictID FROM BusinessConflict))
--ORDER BY Name	 


--SELECT * FROM Conflict ORDER BY Name Alabama

--SELECT * FROM Conflict 
--WHERE 
--	(ID IN (SELECT ConflictID FROM StoryConflict)) OR 
--	(ID IN (SELECT ConflictID FROM BusinessConflict)) 


--SELECT * FROM Conflict


--SELECT * FROM CONFLICT WHERE ID NOT IN (SELECT ConflictID FROM BusinessConflict) 





--SELECT * FROM Conflict WHERE ID = 582


--SELECT * FROM BusinessConflict WHERE ConflictID = 582


--SELECT ID FROM Conflict 
--	WHERE 
--		(ID IN (SELECT ConflictID FROM StoryConflict)) OR 
--		(ID IN (SELECT ConflictID FROM BusinessConflict))
--		ORDER BY ID



