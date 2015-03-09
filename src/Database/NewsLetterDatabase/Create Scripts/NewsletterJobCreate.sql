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
 