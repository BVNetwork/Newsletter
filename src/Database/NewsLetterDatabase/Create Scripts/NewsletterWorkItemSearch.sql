IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemSearch')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemSearch'
		DROP  Procedure  NewsletterWorkItemSearch
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemSearch'
GO
CREATE Procedure NewsletterWorkItemSearch
	@jobid int,
	@searchfor nvarchar(150)
AS
BEGIN
	SELECT * 
	FROM NewsletterWorkItems
	WHERE fkJobId = @jobid
	AND EmailAddress LIKE @searchfor
	ORDER BY EmailAddress
END
GO

GRANT EXEC ON NewsletterWorkItemSearch TO PUBLIC
GO
 
/* End Of Script */
 