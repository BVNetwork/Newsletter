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
 