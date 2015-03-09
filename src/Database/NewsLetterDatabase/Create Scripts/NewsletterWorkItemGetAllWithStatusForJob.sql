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
 