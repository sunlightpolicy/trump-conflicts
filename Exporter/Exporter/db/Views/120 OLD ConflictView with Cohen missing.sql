USE [Trump]
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
	
	UNION ALL
	SELECT ID, 'Donald Trump Jr.' FROM Conflict WHERE (Junior = 1)  
	UNION ALL
	SELECT ID, 'Eric Trump' FROM Conflict WHERE (Eric = 1)  
	UNION ALL
	SELECT ID, 'Melania Trump' FROM Conflict WHERE (Melania = 1)  
	UNION ALL
	SELECT ID, 'Donald Trump' FROM Conflict WHERE (Trump = 1)  
	UNION ALL
	SELECT ID, 'Ivanka Trump' FROM Conflict WHERE (Ivanka = 1)  
	UNION ALL
	SELECT ID, 'Jared Kushner' FROM Conflict WHERE (Jared= 1)  
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




SELECT * FROM ConflictView WHERe COnflict Like '%Cohen%'


SELECT * FROM (


SELECT bc.ConflictId, fm.Name FamilyMember 
	FROM BusinessConflict bc 
	JOIN Business b ON b.ID = bc.BusinessID
	JOIN FamilyMemberBusiness fmb ON fmb.BusinessID = b.ID 
	JOIN FamilyMember fm ON fmb.FamilyMemberID = fm.ID
	
	UNION ALL
	SELECT ID, 'Donald Trump Jr.' FROM Conflict WHERE (Junior = 1)  
	UNION ALL
	SELECT ID, 'Eric Trump' FROM Conflict WHERE (Eric = 1)  
	UNION ALL
	SELECT ID, 'Melania Trump' FROM Conflict WHERE (Melania = 1)  
	UNION ALL
	SELECT ID, 'Donald Trump' FROM Conflict WHERE (Trump = 1)  
	UNION ALL
	SELECT ID, 'Ivanka Trump' FROM Conflict WHERE (Ivanka = 1)  
	UNION ALL
	SELECT ID, 'Jared Kushner' FROM Conflict WHERE (Jared= 1)  

	) a
	WHERE ConflictID = 449


	SELECT * FROM Conflict WHERE Name Like '%cohen%'