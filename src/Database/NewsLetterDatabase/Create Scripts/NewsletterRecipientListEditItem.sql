 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListEditItem')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListEditItem'
		DROP  Procedure  NewsletterRecipientListEditItem
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListEditItem'
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
CREATE PROCEDURE [dbo].[NewsletterRecipientListEditItem]
	@recipientlistid int,
	@emailaddress nvarchar(150),
	@source int,
	@comment nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @exists int;
	
	SELECT @exists = Count(*) FROM NewsletterEmailAddresses 
	WHERE fkRecipientListId = @recipientlistid
		AND EmailAddress = @emailaddress
		
	IF (@exists = 0)
	BEGIN
		INSERT INTO NewsletterEmailAddresses 
				(fkRecipientListId, EmailAddress, Comment, Source)
		VALUES	(@recipientlistid, @emailaddress, @comment, @source)
	END
	ELSE
	BEGIN
		UPDATE	NewsletterEmailAddresses 
		SET		Comment = @comment, Source = @source
		WHERE	fkRecipientListId = @recipientlistid
		AND		EmailAddress = @emailaddress
		
	END
	
    SELECT @@IDENTITY
	RETURN @@IDENTITY
END
GO

GRANT EXEC ON NewsletterRecipientListEditItem TO PUBLIC

GO

/* End Of Script */
 