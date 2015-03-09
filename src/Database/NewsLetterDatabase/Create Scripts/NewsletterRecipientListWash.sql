  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListWash')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListWash'
		DROP  Procedure NewsletterRecipientListWash
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListWash'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 01.02.2007
-- Description:	Deletes all items in NewsletterRecipientList that exists in NewsletterRecipientList with id = @recipientListWashId
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListWash]
	@recipientListId int,
	@recipientListWashId int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
	DELETE NewsletterRecipientLists
	FROM	NewsletterRecipientLists 
	INNER JOIN NewsletterRecipientLists washList
    ON NewsletterRecipientLists.pkRecipientListId = washList.pkRecipientListId 
    AND NewsletterRecipientLists.pkRecipientListId = @recipientListId
    AND washList.pkRecipientListId = @recipientListWashId; 


	SELECT @@ROWCOUNT
	RETURN @@ROWCOUNT    
    
END
GO
GRANT EXEC ON NewsletterRecipientListWash TO PUBLIC
GO

/* End Of Script */
 