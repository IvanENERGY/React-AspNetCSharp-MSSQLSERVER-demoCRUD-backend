USE test_db;
DROP PROCEDURE if exists [sp_DeleteEmployee]
GO  
CREATE PROCEDURE [sp_DeleteEmployee] 
@EmployeeId int
AS   
BEGIN
Declare @Err_Msg AS varchar(50)

		BEGIN TRY
			BEGIN TRAN
				DELETE FROM dbo.Employee
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
--execute [sp_DeleteEmployee] 7