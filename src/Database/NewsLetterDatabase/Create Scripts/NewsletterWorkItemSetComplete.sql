 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'NewsletterWorkItemSetComplete')
	BEGIN
		PRINT 'Dropping Procedure NewsletterWorkItemSetComplete'
		DROP  Procedure  NewsletterWorkItemSetComplete
	END

GO

PRINT 'Creating Procedure NewsletterWorkItemSetComplete'
GO
CREATE Procedure NewsletterWorkItemSetComplete
	@jobid int, 
	@status int,
	@emailaddress nvarchar(4000)
AS
BEGIN
	DECLARE @spot SMALLINT, @str VARCHAR(4000), @sql VARCHAR(4000)  
     
    WHILE @emailaddress <> ''  
    BEGIN  
        SET @spot = CHARINDEX(',', @emailaddress)  
        IF @spot>0  
            BEGIN  
                SET @str = LEFT(@emailaddress, @spot-1) 
                SET @emailaddress = RIGHT(@emailaddress, LEN(@emailaddress)-@spot)  
            END  
        ELSE  
            BEGIN  
                SET @str = @emailaddress 
                SET @emailaddress = ''  
            END  
        SET @sql = 'UPDATE NewsletterWorkItems Set Status = ' + @status + ' WHERE EmailAddress = ('''+CONVERT(VARCHAR(150),@str)+''')'  
        EXEC(@sql)  
    END  

END
GO

GRANT EXEC ON NewsletterWorkItemSetComplete TO PUBLIC
GO

/* End Of Script */
 