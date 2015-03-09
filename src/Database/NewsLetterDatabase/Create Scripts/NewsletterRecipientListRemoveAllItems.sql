IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListRemoveAllItems')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListRemoveAllItems'
		DROP  Procedure  NewsletterRecipientListRemoveAllItems
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListRemoveAll'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- Description:	Deletes email addresses from list, but not the list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListRemoveAllItems]
	@recipientlistid int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- First delete all work items
	DELETE FROM NewsletterEmailAddresses
	WHERE		fkRecipientlistId = @recipientlistid

END
GO
GRANT EXEC ON NewsletterRecipientListRemoveAllItems TO PUBLIC
GO

/* End Of Script */
 