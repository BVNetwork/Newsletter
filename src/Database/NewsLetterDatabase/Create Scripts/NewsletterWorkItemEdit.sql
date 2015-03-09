IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemEdit')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemEdit'
		DROP  Procedure  NewsletterWorkItemEdit
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemEdit'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Changes the status of a work item
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemEdit]
	@jobid int,
	@emailaddress nvarchar(150),
	@status int,
	@info nvarchar(2000) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @exists int;
	
	SELECT		@exists	= Count(*) FROM NewsletterWorkItems
	WHERE 		fkJobId = @jobid
		AND		EmailAddress = @emailaddress;

	IF	(@exists = 0)
	BEGIN
		INSERT INTO NewsletterWorkItems
		VALUES (@jobid, @emailaddress, @status, @info)
	END
	ELSE
	BEGIN
			-- Insert statements for procedure here
		UPDATE 		NewsletterWorkItems
		SET 		Status = @status,
					Info = @info
		WHERE 		fkJobId = @jobid
		AND			EmailAddress = @emailaddress
	END
END
GO

GRANT EXEC ON NewsletterWorkItemEdit TO PUBLIC
GO

 
/* End Of Script */
 