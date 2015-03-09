IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListSearch')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListSearch'
		DROP  Procedure  NewsletterRecipientListSearch
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListSearch'
GO
CREATE Procedure NewsletterRecipientListSearch
	@recipientListid int,
	@searchfor nvarchar(150)
AS
BEGIN
	
	SELECT * 
		FROM NewsletterEmailAddresses
		WHERE fkRecipientListId = @recipientListid
		AND EmailAddress LIKE @searchfor
		ORDER BY EmailAddress
END
GO

GRANT EXEC ON NewsletterRecipientListSearch TO PUBLIC
GO
 
/* End Of Script */