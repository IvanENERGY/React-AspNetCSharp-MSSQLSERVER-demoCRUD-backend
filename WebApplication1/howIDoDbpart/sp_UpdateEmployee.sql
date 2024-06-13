USE test_db;
DROP PROCEDURE if exists [sp_UpdateEmployee]
GO  
CREATE PROCEDURE [sp_UpdateEmployee] 
@EmployeeName nvarchar(1000),
@Department nvarchar(1000),
@DateOfJoining datetime,
@PhotoFileName nvarchar(1000),
@EmployeeId int
AS   
BEGIN
Declare @Err_Msg AS varchar(50)

		BEGIN TRY
			BEGIN TRAN
				UPDATE dbo.Employee
				SET EmployeeName= @EmployeeName,
					Department=@Department,
					DateOfJoining=@DateOfJoining,
					PhotoFileName=@PhotoFileName
				WHERE EmployeeId=@EmployeeId
			COMMIT TRAN
		END TRY

		BEGIN CATCH
			SET @Err_Msg=ERROR_MESSAGE()
			ROLLBACK TRAN
		END CATCH	

	IF (@Err_Msg is null)	
		SELECT 'SUCCESS' AS tranState
	ELSE
		SELECT @Err_Msg AS tranState

END
GO  
--execute [sp_UpdateEmployee] "IvanC","ACCOUNT", "2023-06-12" ,"somepic.png",12