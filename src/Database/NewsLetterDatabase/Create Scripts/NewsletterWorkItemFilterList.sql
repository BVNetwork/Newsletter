 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemFilterList')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemFilterList'
		DROP  Procedure NewsletterWorkItemFilterList
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemFilterList'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- Description:	Deletes all items in NewsletterWorkItems if they exists in NewsletterEmailAddresses
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemFilterList]
	@jobid int,
	@recipientListId int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Delete all work items
	
	DELETE NewsletterWorkItems
	FROM	NewsletterWorkItems
	INNER JOIN NewsletterEmailAddresses 
    ON NewsletterWorkItems.EmailAddress = NewsletterEmailAddresses.EmailAddress
    AND NewsletterEmailAddresses.fkRecipientListId = @recipientListId
    AND NewsletterWorkItems.fkJobId = @jobid; 

	SELECT @@ROWCOUNT
	RETURN @@ROWCOUNT    
    /*
    
	DELETE FROM NewsletterWorkItems
	WHERE EXISTS
		(SELECT 1 FROM NewsletterEmailAddresses
			WHERE EmailAddress = NewsletterWorkItems.EmailAddress
				AND fkReciptientListId = @jobid)
	*/
END
GO
GRANT EXEC ON NewsletterWorkItemFilterList TO PUBLIC
GO

/* End Of Script */
 