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
 