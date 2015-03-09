IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGetAllByEmail')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGetAllByEmail'
		DROP  Procedure  NewsletterRecipientListGetAllByEmail
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGetAllByEmail'
GO
CREATE Procedure NewsletterRecipientListGetAllByEmail
	@email nvarchar(150)
AS
BEGIN
	SELECT * FROM NewsletterRecipientLists	
	WHERE pkRecipientListId IN (
	
	SELECT fkRecipientListId 
		FROM NewsletterEmailAddresses
		WHERE EmailAddress = @email
)

END
GO

GRANT EXEC ON NewsletterRecipientListGetAllByEmail TO PUBLIC

GO 

/* End Of Script */
