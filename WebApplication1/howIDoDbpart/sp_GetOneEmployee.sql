USE test_db;
DROP PROCEDURE if exists [sp_GetOneEmployee]
GO  
CREATE PROCEDURE [sp_GetOneEmployee]
@id int
AS   
BEGIN

SELECT EmployeeId
,EmployeeName
,Department
,convert(varchar(10),DateOfJoining,120) as DateOfJoining
,PhotoFileName 
FROM dbo.Employee
WHERE EmployeeId=@id

END
GO  
--execute [sp_GetOneEmployee] 1