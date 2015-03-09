 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListAddItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListAddItem'
		DROP  Procedure  NewsletterRecipientListAddItem
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListAddItem'
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
CREATE PROCEDURE [dbo].[NewsletterRecipientListAddItem]
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

GRANT EXEC ON NewsletterRecipientListAddItem TO PUBLIC

GO

/* End Of Script */
 