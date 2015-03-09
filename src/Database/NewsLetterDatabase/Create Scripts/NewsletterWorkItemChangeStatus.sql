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
 