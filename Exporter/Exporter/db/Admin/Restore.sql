


RESTORE FILELISTONLY
FROM DISK = N'C:\TrumpDb\Trump.bak'
GO


-- "Trump2" will be the name of the restored database (see 4 names below)


ALTER DATABASE Trump2
SET SINGLE_USER WITH
ROLLBACK IMMEDIATE
 
----Restore Database
RESTORE DATABASE Trump2
FROM DISK = N'C:\TrumpDb\Trump.bak'
WITH MOVE 'Trump' TO 'C:\TrumpDb\Trump2.mdf',
MOVE 'Trump_log' TO 'C:\TrumpDb\Trump2_ldf.mdf'
 


