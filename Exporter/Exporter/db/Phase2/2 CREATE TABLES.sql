USE Trump 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

--IF OBJECT_ID('FK_UserGroup_Person') IS NOT NULL 
--	BEGIN
--		ALTER TABLE UserGroup DROP CONSTRAINT FK_UserGroup_Person
--	END


EXEC DropTable 'StoryConflict'
GO
EXEC DropTable 'Story'
GO
EXEC DropTable 'StoryStatus'
GO
EXEC DropTable 'BusinessConflict'
GO
EXEC DropTable 'BusinessOwnership'
GO
EXEC DropTable 'FamilyMemberBusiness'
GO
EXEC DropTable 'EthicsDocumentBusiness'
GO
EXEC DropTable 'Business'
GO
EXEC DropTable 'Conflict'
GO
EXEC DropTable 'EthicsDocument'
GO
EXEC DropTable 'FamilyMember'
GO
EXEC DropTable 'FamilyMemberConflictStatus'
GO
EXEC DropTable 'MediaOutlet'
GO
EXEC DropTable 'SystemUser'
GO
EXEC DropTable 'UserGroup'
GO



CREATE TABLE [dbo].[UserGroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) UNIQUE NOT NULL,
 CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SystemUser](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserGroupID] [int] NOT NULL REFERENCES UserGroup(ID)  DEFAULT ((1)),
	[Email] [varchar](200) UNIQUE NOT NULL,
	[FirstName] [varchar](200) NOT NULL,
	[LastName] [varchar](200) NOT NULL
 CONSTRAINT [PK_SystemUser] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO UserGroup VALUES ('Administrator') 
INSERT INTO UserGroup VALUES ('Editor') 

INSERT INTO SystemUser VALUES (1, 'hniles@sunlightfoundation.com', 'Niles', 'Hillary')
INSERT INTO SystemUser VALUES (1, 'lwalsh@sunlightfoundation.com', 'Walsh', 'Lynn') 


CREATE TABLE [dbo].[FamilyMember](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) UNIQUE NOT NULL,
 CONSTRAINT [PK_FamilyMember] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO FamilyMember VALUES ('Donald Trump')
INSERT INTO FamilyMember VALUES ('Donald Trump Jr.')
INSERT INTO FamilyMember VALUES ('Ivanka Trump')
INSERT INTO FamilyMember VALUES ('Eric Trump')
INSERT INTO FamilyMember VALUES ('Melania Trump')
INSERT INTO FamilyMember VALUES ('Jared Kushner')


CREATE TABLE [dbo].[Conflict](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) UNIQUE NOT NULL,
	[Description] [varchar](MAX) NOT NULL DEFAULT '',
	[Notes] [varchar](MAX) NOT NULL DEFAULT '',
	[DateChanged] [date] NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Conflict_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES SystemUser (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Conflict] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[FamilyMemberConflictStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](20) UNIQUE NOT NULL,
 CONSTRAINT [PK_FamilyMemberConflictStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO FamilyMemberConflictStatus VALUES ('Submitted')
INSERT INTO FamilyMemberConflictStatus VALUES ('Potential')
INSERT INTO FamilyMemberConflictStatus VALUES ('Active')
INSERT INTO FamilyMemberConflictStatus VALUES ('Resolved')


CREATE TABLE [dbo].[MediaOutlet](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) UNIQUE NOT NULL,
 CONSTRAINT [PK_MediaOutlet] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[StoryStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](20) NOT NULL,
 CONSTRAINT [PK_StoryStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO StoryStatus VALUES ('Submitted');
INSERT INTO StoryStatus VALUES ('Approved');
GO


CREATE TABLE [dbo].[Story](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MediaOutletID] [int] NOT NULL REFERENCES MediaOutlet (ID),
	[StoryStatusID] [int] NOT NULL REFERENCES StoryStatus (ID),
	[Link] varchar(1000) NOT NULL DEFAULT '',
	[Headline] [varchar](1000) NOT NULL DEFAULT '',
	[Date] [date] NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Story_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES SystemUser (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Story] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[StoryConflict](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StoryID] [int] NOT NULL REFERENCES Story (ID),
	[ConflictID] [int] NOT NULL REFERENCES Conflict (ID)
 CONSTRAINT [PK_StoryConflict] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[Business](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	Name Varchar(500) NOT NULL DEFAULT '',
 CONSTRAINT [PK_Business] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[FamilyMemberBusiness](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FamilyMemberID] [int] NOT NULL REFERENCES FamilyMember (ID),
	[BusinessID] [int] NOT NULL REFERENCES Business (ID),
	[FamilyMemberConflictStatusID] [int] NOT NULL REFERENCES FamilyMemberConflictStatus (ID),
	[Description] [varchar](MAX) NOT NULL DEFAULT '',
 CONSTRAINT [PK_FamilyMemberBusiness] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO Conflict VALUES ('Unknown', '', '', GetDate(), GetDate(), 1)
GO 
INSERT INTO Business VALUES ('') 
GO


CREATE TABLE [dbo].[BusinessConflict](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	--[ParentID] [int] NOT NULL REFERENCES Business (ID) DEFAULT 1,
	[ConflictID] [int] NOT NULL REFERENCES Conflict (ID),
	[BusinessID] [int] NOT NULL REFERENCES Business (ID),	
 CONSTRAINT [PK_BusinessConflict] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[BusinessOwnership](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OwnerID] [int] NOT NULL REFERENCES Business (ID),
	[OwneeID] [int] NOT NULL REFERENCES Business (ID),
	OwnershipPercentage varchar(10) NOT NULL DEFAULT '',	
 CONSTRAINT [PK_BusinessOwnership] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[EthicsDocument](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FamilyMemberID] [int] NOT NULL REFERENCES FamilyMember (ID),
	[Title] varchar(500) NOT NULL,
	[Link] varchar(500) NOT NULL, 
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_EthicsDocument] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[EthicsDocumentBusiness](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EthicsDocumentID] [int] NOT NULL REFERENCES EthicsDocument (ID),
	[BusinessID] [int] NOT NULL REFERENCES Business (ID),
 CONSTRAINT [PK_EthicsDocumentBusiness] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
