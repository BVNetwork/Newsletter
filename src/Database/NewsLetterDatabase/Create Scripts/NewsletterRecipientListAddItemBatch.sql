 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListAddItemBatch')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListAddItemBatch'
		DROP  Procedure  NewsletterRecipientListAddItemBatch
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListAddItemBatch'
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
CREATE PROCEDURE [dbo].[NewsletterRecipientListAddItemBatch]
	@recipientlistid int,
	@emailaddress nvarchar(150),
	@source int,
	@comment nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO NewsletterEmailAddresses 
				(fkRecipientListId, EmailAddress, Comment, Source)
	VALUES		(@recipientlistid, @emailaddress, @comment, @source)

END
GO

GRANT EXEC ON NewsletterRecipientListAddItemBatch TO PUBLIC

GO

/* End Of Script */
 