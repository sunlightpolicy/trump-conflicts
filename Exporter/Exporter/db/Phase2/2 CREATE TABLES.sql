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


EXEC DropTable 'Story'
GO
EXEC DropTable 'StoryStatus'
GO
EXEC DropTable 'BusinessUnitPotentialConflict'
GO
EXEC DropTable 'BusinessUnitOwnership'
GO
EXEC DropTable 'BusinessUnit'
GO
EXEC DropTable 'FamilyMemberPotentialConflict'
GO
EXEC DropTable 'PotentialConflict'
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


CREATE TABLE [dbo].[PotentialConflict](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) UNIQUE NOT NULL,
	[Description] [varchar](MAX) NOT NULL DEFAULT '',
	[DateChanged] [date] NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_PotentialConflict_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES SystemUser (ID) DEFAULT ((1)),
 CONSTRAINT [PK_PotentialConflict] PRIMARY KEY CLUSTERED 
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
	[PotentialConflictID] [int] NOT NULL REFERENCES PotentialConflict (ID),
	[StoryStatusID] [int] NOT NULL REFERENCES StoryStatus (ID),
	[Headline] [varchar](1000) NULL,
	[Date] [date] NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Story_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES SystemUser (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Story] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[FamilyMemberPotentialConflict](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FamilyMemberID] [int] NOT NULL REFERENCES FamilyMember (ID),
	[PotentialConflictID] [int] NOT NULL REFERENCES PotentialConflict (ID),
	[FamilyMemberConflictStatusID] [int] NOT NULL REFERENCES FamilyMemberConflictStatus (ID),
	[Description] [varchar](MAX) NOT NULL DEFAULT '',
 CONSTRAINT [PK_FamilyMemberPotentialConflict] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[BusinessUnit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	Name Varchar(500) NOT NULL DEFAULT '',
	--[ParentID] [int] NOT NULL REFERENCES BusinessUnit (ID) DEFAULT 1,
	--[PotentialConflictID] [int] NOT NULL REFERENCES PotentialConflict (ID),
	--[Description] [varchar](MAX) NOT NULL DEFAULT '',
 CONSTRAINT [PK_BusinessUnit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO PotentialConflict VALUES ('Unknown', '', GetDate(), GetDate(), 1)
GO 
INSERT INTO BusinessUnit VALUES ('') 
GO


CREATE TABLE [dbo].[BusinessUnitPotentialConflict](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	--[ParentID] [int] NOT NULL REFERENCES BusinessUnit (ID) DEFAULT 1,
	[PotentialConflictID] [int] NOT NULL REFERENCES PotentialConflict (ID),
	[BusinessUnitID] [int] NOT NULL REFERENCES BusinessUnit (ID),	
 CONSTRAINT [PK_BusinessUnitPotentialConflict] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[BusinessUnitOwnership](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OwnerID] [int] NOT NULL REFERENCES BusinessUnit (ID),
	[OwneeID] [int] NOT NULL REFERENCES BusinessUnit (ID),
	OwnershipPercentage varchar(10) NOT NULL DEFAULT '',	
 CONSTRAINT [PK_BusinessUnitOwnership] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
