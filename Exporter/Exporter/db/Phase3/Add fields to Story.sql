USE Trump
GO


ALTER TABLE Story ADD Title [varchar](1000) NOT NULL DEFAULT ''	

EXEC sp_rename 'Story.Text', 'Body'

