 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListGetItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListGetItem'
		DROP  Procedure  NewsletterRecipientListGetItem
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListGetItem'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 
-- Description:	Get one item from list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListGetItem]
	@recipientlistid int,
	@emailaddress nvarchar(150)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT * FROM NewsletterEmailAddresses 
	WHERE	fkRecipientListId = @recipientlistid
	AND		EmailAddress = @emailaddress
		
END
GO

GRANT EXEC ON NewsletterRecipientListGetItem TO PUBLIC

GO  

/* End Of Script */
 