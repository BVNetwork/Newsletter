IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterJobGetAll')
	BEGIN
		PRINT 'Dropping Procedure NewsletterJobGetAll'
		DROP  Procedure  NewsletterJobGetAll
	END

GO

PRINT 'Creating Procedure NewsletterJobGetAll'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Creates a new newsletter job, and returs the id of the job
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterJobGetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT * 
	FROM NewsletterJobs
	ORDER BY [Name]
	
	SELECT pkJobiD, NewsletterWorkItems.Status, Count(*) AS NumOfRows
	FROM NewsletterJobs
		Left join NewsletterWorkItems on (NewsletterJobs.pkJobId = NewsletterWorkItems.fkJobId)
		Group by pkJobiD, [Name], NewsletterWorkItems.Status
		--Having NewsletterWorkItems.Status is not null
	Order by [Name]	
	
END
GO
 
/* End Of Script */
 