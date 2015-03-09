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
 