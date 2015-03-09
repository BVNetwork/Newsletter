IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemGet')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemGet'
		DROP  Procedure  NewsletterWorkItemGet
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemGet'
GO
CREATE Procedure NewsletterWorkItemGet
	@jobid int, 
	@emailaddress nvarchar(150)
AS
BEGIN
	SELECT	* 
	FROM	NewsletterWorkItems
	WHERE	fkJobId = @jobid
	AND		EmailAddress = @emailaddress
END
GO

GRANT EXEC ON NewsletterWorkItemGet TO PUBLIC
GO

/* End Of Script */
 