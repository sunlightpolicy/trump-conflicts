USE Trump
GO


UPDATE Conflict SET Description = 'TEST DESCRIPTION FOR THE CONFLICT (Friendly Name): ' + Name WHERE Description = ''  

UPDATE Conflict SET Description = '' WHERE Description Like 'TEST DESCRIPTION FOR THE CONFLICT (Friendly Name):%'

UPDATE Conflict SET Notes = '' WHERE Notes = '>'


UPDATE Conflict SET Description = REPLACE (Description, '&#39;', '')

UPDATE Conflict SET Notes = REPLACE (Notes, '&#39;', '')

