USE Trump 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF OBJECT_ID('FK_UserGroup_Person') IS NOT NULL 
	BEGIN
		ALTER TABLE UserGroup DROP CONSTRAINT FK_UserGroup_Person
	END

IF OBJECT_ID('FK_Person_UserGroup') IS NOT NULL 
	BEGIN
		ALTER TABLE Person DROP CONSTRAINT FK_Person_UserGroup
	END


EXEC DropTable 'Story'
GO
EXEC DropTable 'Conflict'
GO
EXEC DropTable 'ConflictingEntity'
GO
EXEC DropTable 'FamilyMember'
GO
EXEC DropTable 'Status'
GO
EXEC DropTable 'UserGroup'
GO
EXEC DropTable 'Person'
GO



CREATE TABLE [dbo].[UserGroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) UNIQUE NOT NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_UserGroup_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL CONSTRAINT [DF_UserGroup_EditorID]  DEFAULT ((1)),
 CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO




CREATE TABLE [dbo].[Person](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserGroupID] [int] NOT NULL CONSTRAINT [DF_Person_UserGroupID]  DEFAULT ((1)),
	[Email] [varchar](200) UNIQUE NOT NULL,
	[FirstName] [varchar](200) NOT NULL,
	[LastName] [varchar](200) NOT NULL,
	[EditTime] [datetime2](6) NOT NULL DEFAULT (getdate()),
	[EditorID] [int] NOT NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO Person VALUES (1, 'hniles@sunlightfoundation.com', 'Niles', 'Hillary', GetDate(), 1)

INSERT INTO UserGroup VALUES ('Administrator', GetDate(), 1) 
INSERT INTO UserGroup VALUES ('Editor', GetDate(), 1) 
INSERT INTO UserGroup VALUES ('Viewer', GetDate(), 1) 



-- Now that Person exists add FK from UserGroup to Person

ALTER TABLE [dbo].[UserGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserGroup_Person] FOREIGN KEY([EditorID])
REFERENCES [dbo].[Person] ([ID])
GO

ALTER TABLE [dbo].[UserGroup] CHECK CONSTRAINT [FK_UserGroup_Person]
GO


SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Person] FOREIGN KEY([EditorID])
REFERENCES [dbo].[Person] ([ID])
GO

ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_Person]
GO

ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_UserGroup] FOREIGN KEY([UserGroupID])
REFERENCES [dbo].[UserGroup] ([ID])
GO

ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_UserGroup]
GO



EXEC DropTable 'FamilyMember'
GO
CREATE TABLE [dbo].[FamilyMember](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) UNIQUE NOT NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_FamilyMember_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_FamilyMember] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC DropTable 'ConflictingEntity'
GO
CREATE TABLE [dbo].[ConflictingEntity](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) UNIQUE NOT NULL,
	[Description] [varchar](MAX) NOT NULL DEFAULT '',
	[FamilyMemberID] [int] NOT NULL REFERENCES FamilyMember (ID),
	[DateChanged] [date] NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_ConflictingEntity_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_ConflictingEntity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



EXEC DropTable ''
GO
CREATE TABLE [dbo].[Status](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](20) UNIQUE NOT NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Status_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO Status VALUES ('Submitted', GetDate(), 1)
INSERT INTO Status VALUES ('Potential', GetDate(), 1)
INSERT INTO Status VALUES ('Active', GetDate(), 1)
INSERT INTO Status VALUES ('Resolved', GetDate(), 1)



EXEC DropTable 'Source'
GO
CREATE TABLE [dbo].[Source](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) UNIQUE NOT NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Source_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Source] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



INSERT INTO FamilyMember VALUES ('Donald Trump', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Donald Trump Jr.', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Ivanka Trump', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Eric Trump', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Melania Trump', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Jared Kushner', GetDate(), 1)





EXEC DropTable 'Story'
GO
EXEC DropTable 'Conflict'
GO

CREATE TABLE [dbo].[Conflict](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ConflictingEntityID] [int] NOT NULL REFERENCES ConflictingEntity (ID),
	[StatusID] [int] NOT NULL REFERENCES Status (ID),
	[DateChanged] [date] NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Conflict_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Conflict] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Story](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceID] [int] NOT NULL REFERENCES Source (ID),
	[ConflictID] [int] NOT NULL REFERENCES Conflict (ID),
	[Headline] [varchar](1000) NULL,
	[DateChanged] [date] NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Story_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Story] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




