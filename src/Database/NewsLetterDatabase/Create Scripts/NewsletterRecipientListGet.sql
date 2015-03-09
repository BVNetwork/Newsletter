IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGet')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGet'
		DROP  Procedure  NewsletterRecipientListGet
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGet'
GO
CREATE Procedure NewsletterRecipientListGet
	@recipientlistid int
AS
BEGIN

	SELECT *, 
		(SELECT Count(*) FROM NewsletterEmailAddresses 
		WHERE NewsletterRecipientLists.pkRecipientListId = NewsletterEmailAddresses.fkRecipientListId)
		AS [Count] 
	FROM NewsletterRecipientLists
	WHERE pkRecipientListId = @recipientlistid
	ORDER BY [Name] ASC
	
END
GO

GRANT EXEC ON NewsletterRecipientListGet TO PUBLIC

GO

/* End Of Script */
 