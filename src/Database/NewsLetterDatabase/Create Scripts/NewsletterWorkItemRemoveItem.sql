IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemRemoveItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemRemoveItem'
		DROP  Procedure NewsletterWorkItemRemoveItem
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemRemoveItem'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemRemoveItem]
	@jobid int,
	@emailAddress varchar(150)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Delete all work items
	DELETE FROM NewsletterWorkItems
	WHERE		fkJobId = @jobid
		AND		EmailAddress = @emailAddress
	
END
GO
 
GRANT EXEC ON NewsletterWorkItemRemoveItem TO PUBLIC
GO

/* End Of Script */
 