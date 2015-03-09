IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGetAllItems')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGetAllItems'
		DROP  Procedure  NewsletterRecipientListGetAllItems
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGetAllItems'
GO
CREATE Procedure NewsletterRecipientListGetAllItems
	@recipientlistid int
AS
BEGIN
	SELECT * 
	FROM NewsletterEmailAddresses
	WHERE fkRecipientListId = @recipientlistid
	ORDER BY Added DESC

END
GO

GRANT EXEC ON NewsletterRecipientListGetAllItems TO PUBLIC

GO

/* End Of Script */
 