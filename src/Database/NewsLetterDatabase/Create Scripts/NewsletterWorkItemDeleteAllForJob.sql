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
 