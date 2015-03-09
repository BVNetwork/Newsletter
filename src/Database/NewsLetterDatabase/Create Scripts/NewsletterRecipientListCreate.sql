IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterRecipientListCreate')
	BEGIN
		PRINT 'Dropping Procedure NewsletterRecipientListCreate'
		DROP  Procedure  NewsletterRecipientListCreate
	END

GO

PRINT 'Creating Procedure NewsletterRecipientListCreate'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 
-- Description:	Creates a new recipient list, and returs the id
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterRecipientListCreate]
	-- Add the parameters for the stored procedure here 
	@name nvarchar(100), 
	@listtype int = 0,
	@description nvarchar(2000) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO NewsletterRecipientLists(Name, ListType, Description)
	VALUES		(@name, @listtype, @description)
	
	SELECT @@IDENTITY
	RETURN @@IDENTITY
END
GO

GRANT EXEC ON NewsletterRecipientListCreate TO PUBLIC

GO

/* End Of Script */
 