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
 