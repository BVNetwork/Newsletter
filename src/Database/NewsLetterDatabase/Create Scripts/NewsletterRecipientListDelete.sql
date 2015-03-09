IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListDelete')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListDelete'
		DROP  Procedure  NewsletterRecipientListDelete
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListDelete'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Deletes a recipient list and all the 
--				corresponding email addresses
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListDelete]
	@recipientlistid int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- First delete all work items
	DELETE FROM NewsletterEmailAddresses
	WHERE		fkRecipientlistId = @recipientlistid
	
	-- Delete job
	DELETE FROM NewsletterRecipientLists
	WHERE		pkRecipientlistId = @recipientlistid

END
GO
GRANT EXEC ON NewsletterRecipientListDelete TO PUBLIC
GO

/* End Of Script */
 