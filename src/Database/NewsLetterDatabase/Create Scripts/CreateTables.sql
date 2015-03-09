
/* If you need to recreate the tables, when they already exist, uncomment the following section */
/*
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_NewsletterEmailAddresses_NewsletterRecipientLists]') AND parent_object_id = OBJECT_ID(N'[dbo].[NewsletterEmailAddresses]'))
ALTER TABLE [dbo].[NewsletterEmailAddresses] DROP CONSTRAINT [FK_NewsletterEmailAddresses_NewsletterRecipientLists]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_NewsletterWorkItems_NewsletterJobs]') AND parent_object_id = OBJECT_ID(N'[dbo].[NewsletterWorkItems]'))
ALTER TABLE [dbo].[NewsletterWorkItems] DROP CONSTRAINT [FK_NewsletterWorkItems_NewsletterJobs]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NewsletterEmailAddresses]') AND type in (N'U'))
DROP TABLE [dbo].[NewsletterEmailAddresses]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NewsletterJobs]') AND type in (N'U'))
DROP TABLE [dbo].[NewsletterJobs]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NewsletterRecipientLists]') AND type in (N'U'))
DROP TABLE [dbo].[NewsletterRecipientLists]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NewsletterWorkItems]') AND type in (N'U'))
DROP TABLE [dbo].[NewsletterWorkItems]
*/


/* Creates the tables */

/****** Object:  Table [dbo].[NewsletterEmailAddresses]    Script Date: 03/05/2007 15:25:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewsletterEmailAddresses](
	[fkRecipientListId] [int] NOT NULL,
	[EmailAddress] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Comment] [nvarchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Added] [datetime] NOT NULL CONSTRAINT [DF_NewsletterEmailAddresses_Added]  DEFAULT (getdate()),
	[Source] [int] NOT NULL CONSTRAINT [DF_NewsletterEmailAddresses_Source]  DEFAULT ((0)),
 CONSTRAINT [PK_NewsletterEmailAddresses] PRIMARY KEY CLUSTERED 
(
	[fkRecipientListId] ASC,
	[EmailAddress] ASC ) 
 ON [PRIMARY],
 CONSTRAINT [IX_UniqeKey] UNIQUE NONCLUSTERED 
(
	[fkRecipientListId] ASC,
	[EmailAddress] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[NewsletterJobs]    Script Date: 03/05/2007 15:25:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewsletterJobs](
	[pkJobId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [nvarchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PageId] [int] NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_NewsletterJobs_Status]  DEFAULT ((0)),
 CONSTRAINT [PK_NewsletterJobs] PRIMARY KEY CLUSTERED 
(
	[pkJobId] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NewsletterRecipientLists]    Script Date: 03/05/2007 15:25:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewsletterRecipientLists](
	[pkRecipientListId] [int] IDENTITY(1,1) NOT NULL,
	[ListType] [int] NOT NULL CONSTRAINT [DF_NewsletterRecipientLists_ListType]  DEFAULT ((0)),
	[Name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_NewsletterSavedRecipients_Added]  DEFAULT (getdate()),
	[Description] [nvarchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_NewsletterSavedRecipients] PRIMARY KEY CLUSTERED 
(
	[pkRecipientListId] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NewsletterWorkItems]    Script Date: 03/05/2007 15:25:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewsletterWorkItems](
	[fkJobId] [int] NOT NULL,
	[EmailAddress] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_NewsletterWorkItems_Status]  DEFAULT ((0)),
	[Info] [nvarchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_NewsletterWorkItems] PRIMARY KEY CLUSTERED 
(
	[fkJobId] ASC,
	[EmailAddress] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[NewsletterEmailAddresses]  WITH CHECK ADD  CONSTRAINT [FK_NewsletterEmailAddresses_NewsletterRecipientLists] FOREIGN KEY([fkRecipientListId])
REFERENCES [dbo].[NewsletterRecipientLists] ([pkRecipientListId])
GO
ALTER TABLE [dbo].[NewsletterEmailAddresses] CHECK CONSTRAINT [FK_NewsletterEmailAddresses_NewsletterRecipientLists]
GO
ALTER TABLE [dbo].[NewsletterWorkItems]  WITH CHECK ADD  CONSTRAINT [FK_NewsletterWorkItems_NewsletterJobs] FOREIGN KEY([fkJobId])
REFERENCES [dbo].[NewsletterJobs] ([pkJobId])
GO
ALTER TABLE [dbo].[NewsletterWorkItems] CHECK CONSTRAINT [FK_NewsletterWorkItems_NewsletterJobs]

/* End Of Script */
 