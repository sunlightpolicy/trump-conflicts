


RESTORE FILELISTONLY
FROM DISK = N'd:\db\Trump.bak'
GO


-- "Trump2" will be the name of the restored database (see 4 names below)


ALTER DATABASE Trump3
SET SINGLE_USER WITH
ROLLBACK IMMEDIATE
 
----Restore Database
RESTORE DATABASE Trump3
FROM DISK = N'd:\db\Trump.bak'
WITH MOVE 'Trump' TO 'C:\TrumpDb\Trump3.mdf',
MOVE 'Trump_log' TO 'C:\TrumpDb\Trump3_ldf.mdf'
 
