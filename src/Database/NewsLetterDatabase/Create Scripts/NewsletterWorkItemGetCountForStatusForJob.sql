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
 