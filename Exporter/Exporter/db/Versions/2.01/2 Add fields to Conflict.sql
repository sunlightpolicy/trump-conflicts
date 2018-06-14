USE Trump 
GO

EXEC DropTable 'ConflictPublicationStatus'
GO


CREATE TABLE [dbo].[ConflictPublicationStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) UNIQUE NOT NULL,
 CONSTRAINT [PK_ConflictPublicationStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO ConflictPublicationStatus VALUES ('');
INSERT INTO ConflictPublicationStatus VALUES ('Approved');
INSERT INTO ConflictPublicationStatus VALUES ('Waiting for Review');
INSERT INTO ConflictPublicationStatus VALUES ('To Discuss');


ALTER TABLE Conflict ADD InternalNotes varchar(max) NOT NULL DEFAULT ''

ALTER TABLE Conflict ADD ConflictPublicationStatusID int NOT NULL DEFAULT 1 REFERENCES ConflictPublicationStatus(ID)