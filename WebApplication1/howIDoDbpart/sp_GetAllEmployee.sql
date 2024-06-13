USE test_db;
DROP PROCEDURE if exists [sp_GetAllEmployee]
GO  
CREATE PROCEDURE [sp_GetAllEmployee]

AS   
BEGIN

SELECT EmployeeId
,EmployeeName
,Department
,convert(varchar(10),DateOfJoining,120) as DateOfJoining
,PhotoFileName 
FROM dbo.Employee

END
GO  
--execute [sp_GetAllEmployee]