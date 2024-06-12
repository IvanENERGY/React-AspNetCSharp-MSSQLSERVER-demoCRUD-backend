Create table dbo.Department(
DepartmentId int identity(1,1),
DepartmentName nvarchar(500)
)

insert into dbo.Department values('IT')
insert into dbo.Department values('Support')
select * from dbo.Department