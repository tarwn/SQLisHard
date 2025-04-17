/*
Instructions:
 Replace ADMINUSERNAME with new name
 Replace ADMINPASSWORD with new password
 Run
*/

CREATE LOGIN ADMINUSERNAME WITH PASSWORD=N'ADMINPASSWORD', 
								DEFAULT_DATABASE=[master], 
								DEFAULT_LANGUAGE=[us_english], 
								CHECK_EXPIRATION=ON, 
								CHECK_POLICY=ON
GO

EXEC sys.sp_addsrvrolemember @loginame = N'ADMINUSERNAME', @rolename = N'securityadmin'
EXEC sys.sp_addsrvrolemember @loginame = N'ADMINUSERNAME', @rolename = N'serveradmin'
EXEC sys.sp_addsrvrolemember @loginame = N'ADMINUSERNAME', @rolename = N'dbcreator'
EXEC sys.sp_addsrvrolemember @loginame = N'ADMINUSERNAME', @rolename = N'bulkadmin'
GO


