USE test_db;
DROP PROCEDURE if exists [sp_CreateEmployee]
GO  
CREATE PROCEDURE [sp_CreateEmployee] 
	@EmployeeName nvarchar(1000),
	@Department nvarchar(1000),
	@DateOfJoining datetime,
	@PhotoFileName nvarchar(1000)
AS   
BEGIN
Declare @Err_Msg AS varchar(50)

		BEGIN TRY
			BEGIN TRAN
				Insert into Employee (EmployeeName,Department,DateOfJoining,PhotoFileName)
				values(@EmployeeName,@Department,@DateOfJoining,@PhotoFileName )
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
--execute [sp_CreateEmployee] "Ivan","ACCOUNT", "2023-06-12" ,"somepic.png"