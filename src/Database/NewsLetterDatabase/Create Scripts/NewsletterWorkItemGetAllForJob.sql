IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemGetAllForJob')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemGetAllForJob'
		DROP  Procedure  NewsletterWorkItemGetAllForJob
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemGetAllForJob'
GO
CREATE Procedure NewsletterWorkItemGetAllForJob
	@jobid int
AS
BEGIN
	SELECT * 
	FROM NewsletterWorkItems
	WHERE fkJobId = @jobid
	ORDER BY EmailAddress
END
GO

GRANT EXEC ON NewsletterWorkItemGetAllForJob TO PUBLIC
GO

/* End Of Script */
 