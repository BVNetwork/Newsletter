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
 