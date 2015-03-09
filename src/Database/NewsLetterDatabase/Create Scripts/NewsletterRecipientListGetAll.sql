IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGetAll')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGetAll'
		DROP  Procedure  NewsletterRecipientListGetAll
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGetAll'
GO
CREATE Procedure NewsletterRecipientListGetAll
AS
BEGIN

	SELECT *, 
		(SELECT Count(*) FROM NewsletterEmailAddresses 
		WHERE NewsletterRecipientLists.pkRecipientListId = NewsletterEmailAddresses.fkRecipientListId)
		AS [Count] 
	FROM NewsletterRecipientLists
	ORDER BY [Name] ASC

END
GO

GRANT EXEC ON NewsletterRecipientListGetAll TO PUBLIC

GO

/* End Of Script */
 