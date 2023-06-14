-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetInstructorReport]
	@FromDate DATE = NULL,
	@StudioId INT = 0
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--DECLARE @fromDate DATE = (SELECT DATEADD(month, DATEDIFF(month,0,GETUTCDATE()), 0))
	
	DECLARE @toDate DATETIME = GETUTCDATE()

	IF (@FromDate IS NOT NULL)
		BEGIN
			--SET @fromDate = @StartDate
			SET @toDate = dateadd(mm,1,@FromDate) --(SELECT DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, @StartDate) + 1, 0)))
		END
		else
			begin
				set @fromDate = (SELECT DATEADD(month, DATEDIFF(month,0,GETUTCDATE()), 0))
			end
	--ELSE
	--	BEGIN
	--		SET @fromDate = (SELECT DATEADD(ww, DATEDIFF(ww,-1,GETUTCDATE()), -1))
	--		SET @toDate = GETUTCDATE()
	--	END
	
	declare @EndofMonth date = (select DATEADD(mm, 1, @FromDate))
	 --(SELECT DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, @fromDate) + 1, 0)))
	DECLARE @users TABLE (FirstName NVARCHAR(128), LastName NVARCHAR(128), JoinDate DATETIME, ProfileIMG NVARCHAR(250),UserId NVARCHAR(128), Rate float, ColorCode nchar(10), CurrentNumClasses int, --CurrentEarnings float  ,
							Classes int, AmountSalary float, LastClassDate DATETIME, LastClassType NVARCHAR(250) ,NextClassDate DATETIME,NextClassType NVARCHAR(250), StudioId INT, DateSalary datetime, Confirmed bit, DateConfirmed datetime, 
							Note nvarchar(max), UserConfirmed nvarchar(128), DailyRate float, CurrentDays int)
    -- Insert statements for procedure here
	INSERT INTO @users
	        ( FirstName ,
			  LastName,
	          JoinDate ,
	          ProfileIMG ,
	          UserId ,
			  Rate,
			  ColorCode,
			  CurrentNumClasses,
			  --CurrentEarnings,
	          Classes ,
	          AmountSalary ,
			  LastClassDate,
			  NextClassDate,
			  StudioId,
			  DateSalary,
			  Confirmed,
			  DateConfirmed,
			  Note,
			  UserConfirmed, 
			  DailyRate, 
			  CurrentDays
	        )
	SELECT  u.FirstName,
							u.LastName,
                            u.JoinDate,
                            u.ProfileIMG,
                            u.Id, 
							i.Rate,
							i.ColorCode,
							isNull(MonthlyDoneClasses.NumClasses,0), 
							--isNull(i.Rate * MonthlyDoneClasses.NumClasses,0) , 
							isNull(ins.[Classes],0),
							isNull(ins.Total,0),
							LastClass.Last LastClass, 
							NextClass.Next NextClass,
							u.StudioId,
							ins.[DateSalary],
							isNull(ins.[Confirmed],0),
							ins.DateConfirmed,
							ins.Note,
							(select [FirstName]+ ' ' + [LastName] from AspNetUsers where id = ins.[UserConfirmed]),
							i.DailyRate,
							isnull(MonthlyDays.Numdays, 0)

							
FROM dbo.AspNetUsers u
INNER JOIN dbo.AspNetUserRoles r ON r.UserId = u.Id AND r.RoleId in (SELECT Id FROM [dbo].[AspNetRoles] WHERE Name LIKE '%Instructor%')
inner join [dbo].[InstructorDetails] i on i.InstructorId = u.Id
--INNER JOIN dbo.Studio s ON s.Id = u.StudioId 
left join [dbo].[InstructorSalary] ins on ins.[InstructorId] = u.id and year(ins.[DateSalary]) = year(@fromDate) and month(ins.[DateSalary]) = month(@fromDate)
--INNER JOIN dbo.AspNetRoles roles ON roles.Id = r.RoleId
LEFT JOIN (SELECT InstructorId, MAX(c.Date) Last FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < @toDate GROUP BY ce.InstructorId) LastClass ON LastClass.InstructorId = u.Id
LEFT JOIN (SELECT InstructorId, MIN(c.Date) Next FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date > GETUTCDATE() GROUP BY InstructorId) NextClass ON NextClass.InstructorId = u.Id
--LEFT JOIN (SELECT InstructorId, count(1) NumClasses FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date >= @fromDate and c.Date < @toDate and c.isdeleted = 0 GROUP BY ce.InstructorId) MonthlyClasses ON MonthlyClasses.InstructorId = u.Id
LEFT JOIN (SELECT InstructorId, count(distinct cast(c.date as date)) Numdays FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date >= @fromDate and c.Date < @EndofMonth and c.isdeleted = 0 GROUP BY ce.InstructorId) MonthlyDays ON MonthlyDays.InstructorId = u.Id
LEFT JOIN (SELECT InstructorId, count(distinct c.Id) NumClasses FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date >= @fromDate and c.Date < @EndofMonth and c.isdeleted = 0 GROUP BY ce.InstructorId) MonthlyDoneClasses ON MonthlyDoneClasses.InstructorId = u.Id
WHERE u.StudioId = @StudioId AND I.IsDeleted = 0
order by MonthlyDoneClasses.NumClasses desc

UPDATE trg
SET trg.LastClassType = ct.Name
FROM @users trg
INNER JOIN dbo.Class_Instructors ce ON ce.InstructorId = trg.UserId
INNER JOIN dbo.Class c ON c.Id = ce.ClassId
INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId
WHERE ce.InstructorId = trg.UserId AND c.Date = trg.LastClassDate

UPDATE trg
SET trg.NextClassType = ct.Name
FROM @users trg
INNER JOIN dbo.Class_Instructors ce ON ce.InstructorId = trg.UserId
INNER JOIN dbo.Class c ON c.Id = ce.ClassId
INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId
WHERE ce.InstructorId = trg.UserId AND c.Date = trg.NextClassDate


		SELECT * FROM @users ORDER BY CurrentNumClasses DESC
		--select convert(nvarchar(20), @fromdate) + convert(nvarchar(20), @todate)

END
