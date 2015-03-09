IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListRemoveItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListRemoveItem'
		DROP  Procedure  NewsletterRecipientListRemoveItem
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListRemoveItem'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Adds an email address to a recipient list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListRemoveItem]
	@recipientlistid int,
	@emailaddress nvarchar(150)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM NewsletterEmailAddresses
	WHERE		fkRecipientListId = @recipientlistid
		AND		EmailAddress = @emailaddress

END
GO

GRANT EXEC ON NewsletterRecipientListRemoveItem TO PUBLIC
GO

/* End Of Script */
  