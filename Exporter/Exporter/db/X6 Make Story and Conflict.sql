USE Trump
GO

EXEC DropTable 'Story'
GO
EXEC DropTable 'Conflict'
GO

CREATE TABLE [dbo].[Conflict](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ConflictingEntityID] [int] NOT NULL REFERENCES ConflictingEntity (ID),
	[FamilyMemberID] [int] NOT NULL REFERENCES FamilyMember (ID),
	[CategoryID] [int] NOT NULL REFERENCES Category (ID),
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
	[DateChanged] [date] NULL,
	[EditTime] [datetime2](6) NOT NULL CONSTRAINT [DF_Story_EditTime]  DEFAULT (getdate()),
	[EditorID] [int] NOT NULL  REFERENCES Person (ID) DEFAULT ((1)),
 CONSTRAINT [PK_Story] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO





