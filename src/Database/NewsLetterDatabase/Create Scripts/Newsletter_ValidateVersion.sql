--beginvalidatingquery
	if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_NewsletterDatabaseVersion]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
    begin
            declare @ver int
            exec @ver=sp_NewsletterDatabaseVersion
            if (@ver >= 301)
				select 0, 'Already correct database version'
            else if (@ver = 300)
                 select 1, 'Upgrading database'
            else
                 select -1, 'Invalid database version detected'
    end
    else if exists (select * from sys.objects where object_id = object_id(N'[dbo].[NewsletterJobs]') and type in (N'U'))
	begin	
		-- installed without version proc
		CREATE PROCEDURE [dbo].[sp_NewsletterDatabaseVersion]
		AS
			RETURN 301
		GO
		select 0, 'Already correct database version'
	end
	else
--            select -1, 'Not a Newsletter database'
-- First time - install it
			select 1, 'Newsletter never installed, adding tables and procedures.';			          
--endvalidatingquery


PRINT N'Altering [dbo].[sp_NewsletterDatabaseVersion]...';
GO
ALTER PROCEDURE [dbo].[sp_NewsletterDatabaseVersion]
AS
	RETURN 301
GO
