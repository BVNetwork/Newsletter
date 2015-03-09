   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListInsertFromRecipientList')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListInsertFromRecipientList'
		DROP  Procedure NewsletterRecipientListInsertFromRecipientList
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListInsertFromRecipientList'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 06.03.2007
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListInsertFromRecipientList]
	@recipientListFrom int,
	@recipientListIdTo int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	/*IF NOT EXISTS (SELECT n.fkRecipientListId, n.EMailAddress FROM NewsletterEmailAddresses n WHERE n.fkRecipientListId = @recipientListIdTo and EXISTS (SELECT n2.fkRecipientListId, n2.EmailAddress FROM NewsletterEmailAddresses n2 WHERE n2.fkRecipientListId = n.fkRecipientListId and n2.EmailAddress = n.EmailAddress))*/
	INSERT INTO NewsletterEmailAddresses (fkRecipientListId, EmailAddress)
		SELECT @recipientListIdTo, n.EmailAddress FROM NewsletterEmailAddresses n 
		WHERE n.fkRecipientListId = @recipientListFrom
		and NOT EXISTS
	    (SELECT n2.EmailAddress FROM NewsletterEmailAddresses n2 
		WHERE n2.fkRecipientListId = @recipientListIdTo and n2.EmailAddress = n.EmailAddress)
			

		SELECT @@ROWCOUNT
		RETURN @@ROWCOUNT

END
GO
 
GRANT EXEC ON NewsletterRecipientListInsertFromRecipientList TO PUBLIC
GO

/* End Of Script */
