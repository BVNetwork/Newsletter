
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
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobCreate')
	BEGIN
		PRINT 'Dropping Procedure NewsletterJobCreate'
		DROP  Procedure  NewsletterJobCreate
	END

GO

PRINT 'Creating Procedure NewsletterJobCreate'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Creates a new newsletter job, and returs the id of the job
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterJobCreate]
	-- Add the parameters for the stored procedure here 
	@name nvarchar(255), 
	@pageId int = 0,
	@status int = 0,
	@description nvarchar(2000)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO NewsletterJobs(Name, Description, PageId, Status)
	VALUES		(@name,@description,@pageId,@status)
	SELECT @@IDENTITY
	RETURN @@IDENTITY
END
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobDelete')
	BEGIN
		PRINT 'Dropping Procedure NewsletterJobDelete'
		DROP  Procedure  NewsletterJobDelete
	END

GO

PRINT 'Creating Procedure NewsletterJobDelete'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Deletes a job and all the corresponding work items
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterJobDelete]
	@jobid int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- First delete all work items
	DELETE FROM NewsletterWorkItems
	WHERE		fkJobId = @jobid
	
	-- Delete job
	DELETE FROM NewsletterJobs
	WHERE		pkJobId = @jobid

END
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobEdit')
	BEGIN
		PRINT 'Dropping Procedure NewsletterJobEdit'
		DROP  Procedure  NewsletterJobEdit
	END

GO

PRINT 'Creating Procedure NewsletterJobEdit'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Edits the properties of a job
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterJobEdit]
	@jobid int,
	@name nvarchar(255), 
	@pageid int = 0,
	@status int = 0,
	@description nvarchar(2000)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE    NewsletterJobs
	SET       [Name] = @name, Description = @description, PageId = @pageid, Status = @status
	WHERE     pkJobId = @jobid
END
GO

/* End Of Script */
 
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobGet')
	BEGIN
		PRINT 'Dropping Stored Procedure NewsletterJobGet'
		DROP  Procedure  NewsletterJobGet
	END

GO

PRINT 'Creating Stored Procedure NewsletterJobGet'
GO

CREATE Procedure [dbo].[NewsletterJobGet]
	@jobid int
AS
BEGIN
	SELECT * 
	FROM NewsletterJobs
	WHERE pkJobId = @jobid
	
	SELECT NewsletterWorkItems.Status, Count(*) AS NumOfRows
	FROM NewsletterJobs 
		Left join NewsletterWorkItems on (NewsletterJobs.pkJobId = NewsletterWorkItems.fkJobId)
	WHERE fkJobID = @jobid
	GROUP BY [Name], NewsletterWorkItems.Status
	ORDER BY [Name]	

END
GO
GRANT EXEC ON NewsletterJobGet TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobGetAll')
	BEGIN
		PRINT 'Dropping Procedure NewsletterJobGetAll'
		DROP  Procedure  NewsletterJobGetAll
	END

GO

PRINT 'Creating Procedure NewsletterJobGetAll'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Creates a new newsletter job, and returs the id of the job
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterJobGetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT * 
	FROM NewsletterJobs
	ORDER BY [Name]
	
	SELECT pkJobiD, NewsletterWorkItems.Status, Count(*) AS NumOfRows
	FROM NewsletterJobs
		Left join NewsletterWorkItems on (NewsletterJobs.pkJobId = NewsletterWorkItems.fkJobId)
		Group by pkJobiD, [Name], NewsletterWorkItems.Status
		--Having NewsletterWorkItems.Status is not null
	Order by [Name]	
	
END
GO
 
/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobGetAllByStatus')
	BEGIN
		PRINT 'Dropping Procedure NewsletterJobGetAllByStatus'
		DROP  Procedure  NewsletterJobGetAllByStatus
	END

GO

PRINT 'Creating Procedure NewsletterJobGetAllByStatus'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Creates a new newsletter job, and returs the id of the job
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterJobGetAllByStatus]
	@status int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT * 
	FROM NewsletterJobs
	WHERE Status = @status
	ORDER BY [Name]
	
	SELECT pkJobiD, NewsletterWorkItems.Status, Count(*) AS NumOfRows
	FROM NewsletterJobs
		Left join NewsletterWorkItems on (NewsletterJobs.pkJobId = NewsletterWorkItems.fkJobId)
		Group by pkJobiD, [Name], NewsletterWorkItems.Status
		--Having NewsletterWorkItems.Status is not null
	Order by [Name]	
	
END
GO
  
GRANT EXEC ON NewsletterJobGetAllByStatus TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobGetByPage')
	BEGIN
		PRINT 'Dropping Stored Procedure NewsletterJobGetByPage'
		DROP  Procedure  NewsletterJobGetByPage
	END

GO

PRINT 'Creating Stored Procedure NewsletterJobGetByPage'
GO

CREATE Procedure [dbo].[NewsletterJobGetByPage]
	@pageid int
AS
BEGIN
	SELECT * 
	FROM NewsletterJobs
	WHERE PageId = @pageid
	ORDER BY [NAME]

END
GO

GRANT EXEC ON NewsletterJobGetByPage TO PUBLIC

GO

/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListAddItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListAddItem'
		DROP  Procedure  NewsletterRecipientListAddItem
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListAddItem'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Adds an email address to a recipient list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListAddItem]
	@recipientlistid int,
	@emailaddress nvarchar(150),
	@source int,
	@comment nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO NewsletterEmailAddresses 
				(fkRecipientListId, EmailAddress, Comment, Source)
	VALUES		(@recipientlistid, @emailaddress, @comment, @source)

END
GO

GRANT EXEC ON NewsletterRecipientListAddItem TO PUBLIC

GO

/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListAddItemBatch')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListAddItemBatch'
		DROP  Procedure  NewsletterRecipientListAddItemBatch
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListAddItemBatch'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Adds an email address to a recipient list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListAddItemBatch]
	@recipientlistid int,
	@emailaddress nvarchar(150),
	@source int,
	@comment nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO NewsletterEmailAddresses 
				(fkRecipientListId, EmailAddress, Comment, Source)
	VALUES		(@recipientlistid, @emailaddress, @comment, @source)

END
GO

GRANT EXEC ON NewsletterRecipientListAddItemBatch TO PUBLIC

GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListCreate')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListCreate'
		DROP  Procedure  NewsletterRecipientListCreate
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListCreate'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Creates a new recipient list, and returs the id
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListCreate]
	-- Add the parameters for the stored procedure here 
	@name nvarchar(100), 
	@listtype int = 0,
	@description nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO NewsletterRecipientLists(Name, ListType, Description)
	VALUES		(@name, @listtype, @description)
	
	SELECT @@IDENTITY
	RETURN @@IDENTITY
END
GO

GRANT EXEC ON NewsletterRecipientListCreate TO PUBLIC

GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListDelete')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListDelete'
		DROP  Procedure  NewsletterRecipientListDelete
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListDelete'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Deletes a recipient list and all the 
--				corresponding email addresses
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListDelete]
	@recipientlistid int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- First delete all work items
	DELETE FROM NewsletterEmailAddresses
	WHERE		fkRecipientlistId = @recipientlistid
	
	-- Delete job
	DELETE FROM NewsletterRecipientLists
	WHERE		pkRecipientlistId = @recipientlistid

END
GO
GRANT EXEC ON NewsletterRecipientListDelete TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListEdit')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListEdit'
		DROP  Procedure  NewsletterRecipientListEdit
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListEdit'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Edits a recipient list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListEdit]
	@recipientlistid int,
	@name nvarchar(100), 
	@listtype int = 0,
	@description nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE NewsletterRecipientLists
	SET [Name] = @name, 
		ListType = @listtype, 
		Description = @description
	WHERE pkRecipientListId = @recipientlistid

END
GO
GRANT EXEC ON NewsletterRecipientListEdit TO PUBLIC
GO

/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListEditItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListEditItem'
		DROP  Procedure  NewsletterRecipientListEditItem
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListEditItem'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Adds an email address to a recipient list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListEditItem]
	@recipientlistid int,
	@emailaddress nvarchar(150),
	@source int,
	@comment nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @exists int;
	
	SELECT @exists = Count(*) FROM NewsletterEmailAddresses 
	WHERE fkRecipientListId = @recipientlistid
		AND EmailAddress = @emailaddress
		
	IF (@exists = 0)
	BEGIN
		INSERT INTO NewsletterEmailAddresses 
				(fkRecipientListId, EmailAddress, Comment, Source)
		VALUES	(@recipientlistid, @emailaddress, @comment, @source)
	END
	ELSE
	BEGIN
		UPDATE	NewsletterEmailAddresses 
		SET		Comment = @comment, Source = @source
		WHERE	fkRecipientListId = @recipientlistid
		AND		EmailAddress = @emailaddress
		
	END
	
    SELECT @@IDENTITY
	RETURN @@IDENTITY
END
GO

GRANT EXEC ON NewsletterRecipientListEditItem TO PUBLIC

GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGet')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGet'
		DROP  Procedure  NewsletterRecipientListGet
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGet'
GO
CREATE Procedure NewsletterRecipientListGet
	@recipientlistid int
AS
BEGIN

	SELECT *, 
		(SELECT Count(*) FROM NewsletterEmailAddresses 
		WHERE NewsletterRecipientLists.pkRecipientListId = NewsletterEmailAddresses.fkRecipientListId)
		AS [Count] 
	FROM NewsletterRecipientLists
	WHERE pkRecipientListId = @recipientlistid
	ORDER BY [Name] ASC
	
END
GO

GRANT EXEC ON NewsletterRecipientListGet TO PUBLIC

GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGetAll')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGetAll'
		DROP  Procedure  NewsletterRecipientListGetAll
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGetAll'
GO
CREATE Procedure NewsletterRecipientListGetAll
AS
BEGIN

	SELECT *, 
		(SELECT Count(*) FROM NewsletterEmailAddresses 
		WHERE NewsletterRecipientLists.pkRecipientListId = NewsletterEmailAddresses.fkRecipientListId)
		AS [Count] 
	FROM NewsletterRecipientLists
	ORDER BY [Name] ASC

END
GO

GRANT EXEC ON NewsletterRecipientListGetAll TO PUBLIC

GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGetAllByEmail')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGetAllByEmail'
		DROP  Procedure  NewsletterRecipientListGetAllByEmail
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGetAllByEmail'
GO
CREATE Procedure NewsletterRecipientListGetAllByEmail
	@email nvarchar(150)
AS
BEGIN
	SELECT * FROM NewsletterRecipientLists	
	WHERE pkRecipientListId IN (
	
	SELECT fkRecipientListId 
		FROM NewsletterEmailAddresses
		WHERE EmailAddress = @email
)

END
GO

GRANT EXEC ON NewsletterRecipientListGetAllByEmail TO PUBLIC

GO 

/* End Of Script */
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGetAllItems')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGetAllItems'
		DROP  Procedure  NewsletterRecipientListGetAllItems
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGetAllItems'
GO
CREATE Procedure NewsletterRecipientListGetAllItems
	@recipientlistid int
AS
BEGIN
	SELECT * 
	FROM NewsletterEmailAddresses
	WHERE fkRecipientListId = @recipientlistid
	ORDER BY Added DESC

END
GO

GRANT EXEC ON NewsletterRecipientListGetAllItems TO PUBLIC

GO

/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGetItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGetItem'
		DROP  Procedure  NewsletterRecipientListGetItem
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGetItem'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 
-- Description:	Get one item from list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListGetItem]
	@recipientlistid int,
	@emailaddress nvarchar(150)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT * FROM NewsletterEmailAddresses 
	WHERE	fkRecipientListId = @recipientlistid
	AND		EmailAddress = @emailaddress
		
END
GO

GRANT EXEC ON NewsletterRecipientListGetItem TO PUBLIC

GO  

/* End Of Script */
    IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListInsertFromRecipientList')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListInsertFromRecipientList'
		DROP  Procedure NewsletterRecipientListInsertFromRecipientList
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListInsertFromRecipientList'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 06.03.2007
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListInsertFromRecipientList]
	@recipientListFrom int,
	@recipientListIdTo int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	/*IF NOT EXISTS (SELECT n.fkRecipientListId, n.EMailAddress FROM NewsletterEmailAddresses n WHERE n.fkRecipientListId = @recipientListIdTo and EXISTS (SELECT n2.fkRecipientListId, n2.EmailAddress FROM NewsletterEmailAddresses n2 WHERE n2.fkRecipientListId = n.fkRecipientListId and n2.EmailAddress = n.EmailAddress))*/
	INSERT INTO NewsletterEmailAddresses (fkRecipientListId, EmailAddress)
		SELECT @recipientListIdTo, n.EmailAddress FROM NewsletterEmailAddresses n 
		WHERE n.fkRecipientListId = @recipientListFrom
		and NOT EXISTS
	    (SELECT n2.EmailAddress FROM NewsletterEmailAddresses n2 
		WHERE n2.fkRecipientListId = @recipientListIdTo and n2.EmailAddress = n.EmailAddress)
			

		SELECT @@ROWCOUNT
		RETURN @@ROWCOUNT

END
GO
 
GRANT EXEC ON NewsletterRecipientListInsertFromRecipientList TO PUBLIC
GO

/* End Of Script */
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListRemoveAllItems')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListRemoveAllItems'
		DROP  Procedure  NewsletterRecipientListRemoveAllItems
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListRemoveAll'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- Description:	Deletes email addresses from list, but not the list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListRemoveAllItems]
	@recipientlistid int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- First delete all work items
	DELETE FROM NewsletterEmailAddresses
	WHERE		fkRecipientlistId = @recipientlistid

END
GO
GRANT EXEC ON NewsletterRecipientListRemoveAllItems TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListRemoveItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListRemoveItem'
		DROP  Procedure  NewsletterRecipientListRemoveItem
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListRemoveItem'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Adds an email address to a recipient list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListRemoveItem]
	@recipientlistid int,
	@emailaddress nvarchar(150)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM NewsletterEmailAddresses
	WHERE		fkRecipientListId = @recipientlistid
		AND		EmailAddress = @emailaddress

END
GO

GRANT EXEC ON NewsletterRecipientListRemoveItem TO PUBLIC
GO

/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListSearch')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListSearch'
		DROP  Procedure  NewsletterRecipientListSearch
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListSearch'
GO
CREATE Procedure NewsletterRecipientListSearch
	@recipientListid int,
	@searchfor nvarchar(150)
AS
BEGIN
	
	SELECT * 
		FROM NewsletterEmailAddresses
		WHERE fkRecipientListId = @recipientListid
		AND EmailAddress LIKE @searchfor
		ORDER BY EmailAddress
END
GO

GRANT EXEC ON NewsletterRecipientListSearch TO PUBLIC
GO
 
/* End Of Script */  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListWash')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListWash'
		DROP  Procedure NewsletterRecipientListWash
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListWash'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 01.02.2007
-- Description:	Deletes all items in NewsletterRecipientList that exists in NewsletterRecipientList with id = @recipientListWashId
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListWash]
	@recipientListId int,
	@recipientListWashId int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
	DELETE NewsletterRecipientLists
	FROM	NewsletterRecipientLists 
	INNER JOIN NewsletterRecipientLists washList
    ON NewsletterRecipientLists.pkRecipientListId = washList.pkRecipientListId 
    AND NewsletterRecipientLists.pkRecipientListId = @recipientListId
    AND washList.pkRecipientListId = @recipientListWashId; 


	SELECT @@ROWCOUNT
	RETURN @@ROWCOUNT    
    
END
GO
GRANT EXEC ON NewsletterRecipientListWash TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemAdd')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemAdd'
		DROP  Procedure  NewsletterWorkItemAdd
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemAdd'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Adds an email address as a worker item for a given job
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemAdd]
	@jobid int,
	@emailaddress nvarchar(150)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO NewsletterWorkItems 
				(fkJobId, EmailAddress)
	VALUES		(@jobid, @emailaddress)

END
GO
GRANT EXEC ON NewsletterWorkItemAdd TO PUBLIC
GO
 
/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemChangeStatus')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemChangeStatus'
		DROP  Procedure  NewsletterWorkItemChangeStatus
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemChangeStatus'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Changes the status of a work item
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemChangeStatus]
	@jobid int,
	@emailaddress nvarchar(150),
	@status int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE 		NewsletterWorkItems
	SET 		Status = @status
	WHERE 		fkJobId = @jobid
		AND		EmailAddress = @emailaddress
	
END
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemChangeStatusAndInfo')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemChangeStatusAndInfo'
		DROP  Procedure NewsletterWorkItemChangeStatusAndInfo
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemChangeStatusAndInfo'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Changes the status and info of a work item
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemChangeStatusAndInfo]
	@jobid int,
	@emailaddress nvarchar(150),
	@status int,
	@info nvarchar(2000)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE 		NewsletterWorkItems
	SET 		Status = @status, Info = @info
	WHERE 		fkJobId = @jobid
		AND		EmailAddress = @emailaddress
	
END
GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemChangeStatusForAll')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemChangeStatusForAll'
		DROP  Procedure  NewsletterWorkItemChangeStatusForAll
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemChangeStatusForAll'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Changes the status of a work item
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemChangeStatusForAll]
	@jobid int,
	@status int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE 		NewsletterWorkItems
	SET 		Status = @status, Info = null
	WHERE 		fkJobId = @jobid

END
GO
 
/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemDeleteAllForJob')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemDeleteAllForJob'
		DROP  Procedure NewsletterWorkItemDeleteAllForJob
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemDeleteAllForJob'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Deletes all corresponding work items for a job
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemDeleteAllForJob]
	@jobid int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Delete all work items
	DELETE FROM NewsletterWorkItems
	WHERE		fkJobId = @jobid
	
END
GO
GRANT EXEC ON NewsletterWorkItemDeleteAllForJob TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemEdit')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemEdit'
		DROP  Procedure  NewsletterWorkItemEdit
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemEdit'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Changes the status of a work item
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemEdit]
	@jobid int,
	@emailaddress nvarchar(150),
	@status int,
	@info nvarchar(2000) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @exists int;
	
	SELECT		@exists	= Count(*) FROM NewsletterWorkItems
	WHERE 		fkJobId = @jobid
		AND		EmailAddress = @emailaddress;

	IF	(@exists = 0)
	BEGIN
		INSERT INTO NewsletterWorkItems
		VALUES (@jobid, @emailaddress, @status, @info)
	END
	ELSE
	BEGIN
			-- Insert statements for procedure here
		UPDATE 		NewsletterWorkItems
		SET 		Status = @status,
					Info = @info
		WHERE 		fkJobId = @jobid
		AND			EmailAddress = @emailaddress
	END
END
GO

GRANT EXEC ON NewsletterWorkItemEdit TO PUBLIC
GO

 
/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemFilterList')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemFilterList'
		DROP  Procedure NewsletterWorkItemFilterList
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemFilterList'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- Description:	Deletes all items in NewsletterWorkItems if they exists in NewsletterEmailAddresses
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemFilterList]
	@jobid int,
	@recipientListId int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Delete all work items
	
	DELETE NewsletterWorkItems
	FROM	NewsletterWorkItems
	INNER JOIN NewsletterEmailAddresses 
    ON NewsletterWorkItems.EmailAddress = NewsletterEmailAddresses.EmailAddress
    AND NewsletterEmailAddresses.fkRecipientListId = @recipientListId
    AND NewsletterWorkItems.fkJobId = @jobid; 

	SELECT @@ROWCOUNT
	RETURN @@ROWCOUNT    
    /*
    
	DELETE FROM NewsletterWorkItems
	WHERE EXISTS
		(SELECT 1 FROM NewsletterEmailAddresses
			WHERE EmailAddress = NewsletterWorkItems.EmailAddress
				AND fkReciptientListId = @jobid)
	*/
END
GO
GRANT EXEC ON NewsletterWorkItemFilterList TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemGet')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemGet'
		DROP  Procedure  NewsletterWorkItemGet
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemGet'
GO
CREATE Procedure NewsletterWorkItemGet
	@jobid int, 
	@emailaddress nvarchar(150)
AS
BEGIN
	SELECT	* 
	FROM	NewsletterWorkItems
	WHERE	fkJobId = @jobid
	AND		EmailAddress = @emailaddress
END
GO

GRANT EXEC ON NewsletterWorkItemGet TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemGetAllForJob')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemGetAllForJob'
		DROP  Procedure  NewsletterWorkItemGetAllForJob
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemGetAllForJob'
GO
CREATE Procedure NewsletterWorkItemGetAllForJob
	@jobid int
AS
BEGIN
	SELECT * 
	FROM NewsletterWorkItems
	WHERE fkJobId = @jobid
	ORDER BY EmailAddress
END
GO

GRANT EXEC ON NewsletterWorkItemGetAllForJob TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemGetAllWithStatusForJob')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemGetAllWithStatusForJob'
		DROP  Procedure  NewsletterWorkItemGetAllWithStatusForJob
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemGetAllWithStatusForJob'
GO
CREATE Procedure NewsletterWorkItemGetAllWithStatusForJob
	@jobid int, 
	@status int
AS
BEGIN
	SELECT * 
	FROM NewsletterWorkItems
	WHERE fkJobId = @jobid
		AND Status = @status
	ORDER BY  EmailAddress
END
GO

GRANT EXEC ON NewsletterWorkItemGetAllWithStatusForJob TO PUBLIC

GO

/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemGetBatchForProcessing')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemGetBatchForProcessing'
		DROP  Procedure NewsletterWorkItemGetBatchForProcessing
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemGetBatchForProcessing'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemGetBatchForProcessing]
	@jobid int,
	@selectStatus int,
	@updatedStatus int,
	@count int = 10

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	-- SET TRANSACTION ISOLATION LEVEL REPEATABLE READ

	BEGIN TRANSACTION 
	
	SET ROWCOUNT @count 

	SELECT * INTO #WorkItems 
	FROM NewsletterWorkItems WITH (TABLOCKX)
	WHERE fkJobId = @jobid
		AND Status = @selectStatus
		
	UPDATE NewsletterWorkItems Set Status = @updatedStatus
	WHERE fkJobId = @jobid
		AND Status = @selectStatus
	
	SET ROWCOUNT 0
	COMMIT TRANSACTION
	
	-- Update memory rows with new status too
	UPDATE #WorkItems Set Status = @updatedStatus

	SELECT * FROM #WorkItems
	
END
GO
 
GRANT EXEC ON NewsletterWorkItemGetBatchForProcessing TO PUBLIC
GO

/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemGetCountForStatusForJob')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemGetCountForStatusForJob'
		DROP  Procedure  NewsletterWorkItemGetCountForStatusForJob
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemGetCountForStatusForJob'
GO
CREATE Procedure NewsletterWorkItemGetCountForStatusForJob
	@jobid int, 
	@status int
AS
BEGIN
	SELECT Count(*)
	FROM NewsletterWorkItems
	WHERE fkJobId = @jobid
		AND Status = @status
END
GO

GRANT EXEC ON NewsletterWorkItemGetCountForStatusForJob TO PUBLIC

GO

/* End Of Script */
   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemInsertFromRecipient')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemInsertFromRecipient'
		DROP  Procedure NewsletterWorkItemInsertFromRecipient
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemInsertFromRecipient'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemInsertFromRecipient]
	@jobid int,
	@recipientlistid int,
	@status int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @rowcntUpdate int;	
	DECLARE @rowcntInsert int;	
	DECLARE @cursoraddress varchar(150);
	
	Set @rowcntInsert = 0;
	Set @rowcntUpdate = 0;

	DECLARE CopyItems_Cursor CURSOR FOR
	SELECT EmailAddress FROM NewsletterEmailAddresses
	WHERE fkRecipientListId = @recipientlistid ;
	
	OPEN CopyItems_Cursor;
	
	FETCH NEXT FROM CopyItems_Cursor INTO @cursoraddress;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE NewsletterWorkItems Set Status = @status 
		WHERE EmailAddress = @cursoraddress
			AND  fkJobId = @jobid
			
		IF @@ROWCOUNT = 0
		BEGIN
			INSERT INTO NewsletterWorkItems (fkJobId, EmailAddress, Status)
			VALUES (@jobid, @cursoraddress, @status);
			
			Set @rowcntInsert = @rowcntInsert + 1
		END 
		ELSE
			Set @rowcntUpdate = @rowcntUpdate + 1
			
      FETCH NEXT FROM CopyItems_Cursor INTO @cursoraddress;
	END;
	CLOSE CopyItems_Cursor;
	DEALLOCATE CopyItems_Cursor;

	/* RETURN @rowcntUpdate + @rowcntInsert */
	SELECT @rowcntInsert;
	RETURN @rowcntInsert;

/*	Need to check if the item already exist
	INSERT INTO NewsletterWorkItems (fkJobId, EmailAddress, Status)
	SELECT @jobid, EmailAddress, @status  FROM NewsletterEmailAddresses
	WHERE fkRecipientListId = @recipientlistid  
		AND NOT EXISTS (SELECT 1 FROM NewsletterWorkItems 
						WHERE NewsletterWorkItems.EmailAddress = NewsletterEmailAddresses.EmailAddress
							AND NewsletterWorkItems.fkJobId = @jobid)
	
	SELECT @@ROWCOUNT
	RETURN @@ROWCOUNT
*/
END
GO
 
GRANT EXEC ON NewsletterWorkItemInsertFromRecipient TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemRemoveItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemRemoveItem'
		DROP  Procedure NewsletterWorkItemRemoveItem
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemRemoveItem'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemRemoveItem]
	@jobid int,
	@emailAddress varchar(150)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Delete all work items
	DELETE FROM NewsletterWorkItems
	WHERE		fkJobId = @jobid
		AND		EmailAddress = @emailAddress
	
END
GO
 
GRANT EXEC ON NewsletterWorkItemRemoveItem TO PUBLIC
GO

/* End Of Script */
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemSearch')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemSearch'
		DROP  Procedure  NewsletterWorkItemSearch
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemSearch'
GO
CREATE Procedure NewsletterWorkItemSearch
	@jobid int,
	@searchfor nvarchar(150)
AS
BEGIN
	SELECT * 
	FROM NewsletterWorkItems
	WHERE fkJobId = @jobid
	AND EmailAddress LIKE @searchfor
	ORDER BY EmailAddress
END
GO

GRANT EXEC ON NewsletterWorkItemSearch TO PUBLIC
GO
 
/* End Of Script */
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemSetComplete')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemSetComplete'
		DROP  Procedure  NewsletterWorkItemSetComplete
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemSetComplete'
GO
CREATE Procedure NewsletterWorkItemSetComplete
	@jobid int, 
	@status int,
	@emailaddress nvarchar(4000)
AS
BEGIN
	DECLARE @spot SMALLINT, @str VARCHAR(4000), @sql VARCHAR(4000)  
     
    WHILE @emailaddress <> ''  
    BEGIN  
        SET @spot = CHARINDEX(',', @emailaddress)  
        IF @spot>0  
            BEGIN  
                SET @str = LEFT(@emailaddress, @spot-1) 
                SET @emailaddress = RIGHT(@emailaddress, LEN(@emailaddress)-@spot)  
            END  
        ELSE  
            BEGIN  
                SET @str = @emailaddress 
                SET @emailaddress = ''  
            END  
        SET @sql = 'UPDATE NewsletterWorkItems Set Status = ' + @status + ' WHERE EmailAddress = ('''+CONVERT(VARCHAR(150),@str)+''')'  
        EXEC(@sql)  
    END  

END
GO

GRANT EXEC ON NewsletterWorkItemSetComplete TO PUBLIC
GO

/* End Of Script */
 