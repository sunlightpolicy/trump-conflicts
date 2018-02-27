USE Trump
GO

EXEC DropTable 'ConflictingEntity'
GO
CREATE TABLE [dbo].[ConflictingEntity](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](5000) NOT NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_ConflictingEntity_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_ConflictingEntity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



EXEC DropTable 'Category'
GO
CREATE TABLE [dbo].[Category](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](20) UNIQUE NOT NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Category_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO Category VALUES ('Submitted', GetDate(), 1)
INSERT INTO Category VALUES ('Potential', GetDate(), 1)
INSERT INTO Category VALUES ('Active', GetDate(), 1)
INSERT INTO Category VALUES ('Resolved', GetDate(), 1)



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

INSERT INTO FamilyMember VALUES ('Donald Trump', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Donald Trump Jr.', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Ivanka Trump', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Eric Trump', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Melania Trump', GetDate(), 1)
INSERT INTO FamilyMember VALUES ('Jared Kushner', GetDate(), 1)




