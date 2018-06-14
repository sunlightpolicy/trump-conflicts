USE Trump 
GO



ALTER TABLE Story ADD Notes varchar(max) NOT NULL DEFAULT ''
GO

ALTER TABLE Story ADD InternalNotes varchar(max) NOT NULL DEFAULT ''
GO


INSERT INTO StoryStatus VALUES ('To Discuss')
INSERT INTO StoryStatus VALUES ('Waiting for review')
