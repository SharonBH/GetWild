﻿-- =============================================
-- Author:		Sharon Ben haim
-- Create date: 10/06/2013
-- Description:	Backup Databases
-- =============================================
create PROCEDURE [dbo].[DBBackup]
	-- Add the parameters for the stored procedure here
	@path VARCHAR(256) = 'H:\SQL-Backup\'
AS
BEGIN
	DECLARE @name VARCHAR(50) -- database name  
	--DECLARE @path VARCHAR(256) -- path for backup files  
	DECLARE @fileName VARCHAR(256) -- filename for backup  
	DECLARE @fileDate VARCHAR(20) -- used for file name

 
-- specify database backup directory
--SET @path = 'C:\Backup\'  

 
-- specify filename format
SELECT @fileDate = CONVERT(VARCHAR(20),GETDATE(),112) 

 
DECLARE db_cursor CURSOR FOR  
SELECT name 
FROM master.dbo.sysdatabases 
WHERE name NOT IN ('master','model','msdb','tempdb')  -- exclude these databases

 
OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @name   

 
WHILE @@FETCH_STATUS = 0   
BEGIN   
       SET @fileName = @path + @name + '_' + @fileDate + '.BAK'  
       BACKUP DATABASE @name TO DISK = @fileName  

 
       FETCH NEXT FROM db_cursor INTO @name   
END   

 
CLOSE db_cursor   
DEALLOCATE db_cursor
END
