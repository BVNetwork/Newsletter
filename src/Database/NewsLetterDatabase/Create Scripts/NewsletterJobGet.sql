
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobGet')
	BEGIN
		PRINT 'Dropping Stored Procedure NewsletterJobGet'
		DROP  Procedure  NewsletterJobGet
	END

GO

PRINT 'Creating Stored Procedure NewsletterJobGet'
GO

CREATE Procedure [dbo].[NewsletterJobGet]
	@jobid int
AS
BEGIN
	SELECT * 
	FROM NewsletterJobs
	WHERE pkJobId = @jobid
	
	SELECT NewsletterWorkItems.Status, Count(*) AS NumOfRows
	FROM NewsletterJobs 
		Left join NewsletterWorkItems on (NewsletterJobs.pkJobId = NewsletterWorkItems.fkJobId)
	WHERE fkJobID = @jobid
	GROUP BY [Name], NewsletterWorkItems.Status
	ORDER BY [Name]	

END
GO
GRANT EXEC ON NewsletterJobGet TO PUBLIC
GO

/* End Of Script */
 