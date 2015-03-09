IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobGetByPage')
	BEGIN
		PRINT 'Dropping Stored Procedure NewsletterJobGetByPage'
		DROP  Procedure  NewsletterJobGetByPage
	END

GO

PRINT 'Creating Stored Procedure NewsletterJobGetByPage'
GO

CREATE Procedure [dbo].[NewsletterJobGetByPage]
	@pageid int
AS
BEGIN
	SELECT * 
	FROM NewsletterJobs
	WHERE PageId = @pageid
	ORDER BY [NAME]

END
GO

GRANT EXEC ON NewsletterJobGetByPage TO PUBLIC

GO

/* End Of Script */
 