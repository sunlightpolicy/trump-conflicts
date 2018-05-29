USE Trump
GO


UPDATE Conflict SET Description = 'TEST DESCRIPTION FOR THE CONFLICT (Friendly Name): ' + Name WHERE Description = ''  




UPDATE Conflict SET Description = REPLACE (Description, '&#39;', '')

UPDATE Conflict SET Notes = REPLACE (Notes, '&#39;', '')

