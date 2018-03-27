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


EXEC DropTable 'Person'
GO
EXEC DropTable 'UserGroup'
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
	
	--[Pass] [varbinary](128) NULL,
	--[UserExpirationDate] [date] NULL,
	--[FailedLoginAttempts] [int] NOT NULL DEFAULT ((0)),
	--[LastIP] [varchar](18) NOT NULL DEFAULT (''),
	--[LastLogin] [date] NOT NULL DEFAULT ('1900-01-01'),
	--[TempPassword] [int] NOT NULL DEFAULT ((0)),
	--[PasswordResetDate] [date] NOT NULL DEFAULT ('1900-01-01'),
	--[LastPassword] [varbinary](128) NULL,
	--[UserAgreementAcceptedDate] [date] NULL,
	--[LastSessionID] [varchar](33) NULL DEFAULT (''),
	--[LastBrowser] [varchar](255) NULL DEFAULT (''),
	--[Deleted] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO Person VALUES (1, 'scottmckissock@gmail.com', 'McKissock', 'Scott', GetDate(), 1)

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


