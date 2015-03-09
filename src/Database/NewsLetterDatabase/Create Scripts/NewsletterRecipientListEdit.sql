IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListEdit')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListEdit'
		DROP  Procedure  NewsletterRecipientListEdit
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListEdit'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Edits a recipient list
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListEdit]
	@recipientlistid int,
	@name nvarchar(100), 
	@listtype int = 0,
	@description nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE NewsletterRecipientLists
	SET [Name] = @name, 
		ListType = @listtype, 
		Description = @description
	WHERE pkRecipientListId = @recipientlistid

END
GO
GRANT EXEC ON NewsletterRecipientListEdit TO PUBLIC
GO

/* End Of Script */
 