  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemInsertFromRecipient')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemInsertFromRecipient'
		DROP  Procedure NewsletterWorkItemInsertFromRecipient
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemInsertFromRecipient'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		ØWH
-- Create date: 17.01.2007
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemInsertFromRecipient]
	@jobid int,
	@recipientlistid int,
	@status int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @rowcntUpdate int;	
	DECLARE @rowcntInsert int;	
	DECLARE @cursoraddress varchar(150);
	
	Set @rowcntInsert = 0;
	Set @rowcntUpdate = 0;

	DECLARE CopyItems_Cursor CURSOR FOR
	SELECT EmailAddress FROM NewsletterEmailAddresses
	WHERE fkRecipientListId = @recipientlistid ;
	
	OPEN CopyItems_Cursor;
	
	FETCH NEXT FROM CopyItems_Cursor INTO @cursoraddress;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE NewsletterWorkItems Set Status = @status 
		WHERE EmailAddress = @cursoraddress
			AND  fkJobId = @jobid
			
		IF @@ROWCOUNT = 0
		BEGIN
			INSERT INTO NewsletterWorkItems (fkJobId, EmailAddress, Status)
			VALUES (@jobid, @cursoraddress, @status);
			
			Set @rowcntInsert = @rowcntInsert + 1
		END 
		ELSE
			Set @rowcntUpdate = @rowcntUpdate + 1
			
      FETCH NEXT FROM CopyItems_Cursor INTO @cursoraddress;
	END;
	CLOSE CopyItems_Cursor;
	DEALLOCATE CopyItems_Cursor;

	/* RETURN @rowcntUpdate + @rowcntInsert */
	SELECT @rowcntInsert;
	RETURN @rowcntInsert;

/*	Need to check if the item already exist
	INSERT INTO NewsletterWorkItems (fkJobId, EmailAddress, Status)
	SELECT @jobid, EmailAddress, @status  FROM NewsletterEmailAddresses
	WHERE fkRecipientListId = @recipientlistid  
		AND NOT EXISTS (SELECT 1 FROM NewsletterWorkItems 
						WHERE NewsletterWorkItems.EmailAddress = NewsletterEmailAddresses.EmailAddress
							AND NewsletterWorkItems.fkJobId = @jobid)
	
	SELECT @@ROWCOUNT
	RETURN @@ROWCOUNT
*/
END
GO
 
GRANT EXEC ON NewsletterWorkItemInsertFromRecipient TO PUBLIC
GO

/* End Of Script */
 