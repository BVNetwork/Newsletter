IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemChangeStatusAndInfo')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemChangeStatusAndInfo'
		DROP  Procedure NewsletterWorkItemChangeStatusAndInfo
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemChangeStatusAndInfo'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Steve Celius
-- Create date: 10.11.2006
-- Description:	Changes the status and info of a work item
-- =============================================
CREATE PROCEDURE [dbo].[NewsletterWorkItemChangeStatusAndInfo]
	@jobid int,
	@emailaddress nvarchar(150),
	@status int,
	@info nvarchar(2000)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE 		NewsletterWorkItems
	SET 		Status = @status, Info = @info
	WHERE 		fkJobId = @jobid
		AND		EmailAddress = @emailaddress
	
END
GO

