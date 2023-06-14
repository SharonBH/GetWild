-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetInstructorList]
	@StartDate DATE = NULL,
	@StudioId INT = 0
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @fromDate DATE = (SELECT DATEADD(month, DATEDIFF(month,0,GETUTCDATE()), 0))
	declare @EndofMonth date = (SELECT DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, @fromDate) + 1, 0)))
	DECLARE @toDate DATETIME = GETUTCDATE()

	--IF (@StartDate IS NOT NULL)
	--	BEGIN
	--		SET @fromDate = @StartDate
	--		SET @toDate = case when DATEADD(day, 6,@StartDate) > GETUTCDATE() then GETUTCDATE() else DATEADD(DAY, 6, @StartDate) end
	--	END
	--ELSE
	--	BEGIN
	--		SET @fromDate = (SELECT DATEADD(ww, DATEDIFF(ww,-1,GETUTCDATE()), -1))
	--		SET @toDate = GETUTCDATE()
	--	END
	
	DECLARE @users TABLE (FirstName NVARCHAR(128), LastName NVARCHAR(128), PhoneNumber NVARCHAR(max), Gender INT, JoinDate DATETIME, ProfileIMG NVARCHAR(250),
	UserId NVARCHAR(128), Rate float, ColorCode nchar(10) ,CurrentNumClasses int, CurrentEarnings float, LastClassDate DATETIME, LastClassType NVARCHAR(250) ,
	NextClassDate DATETIME,NextClassType NVARCHAR(250), StudioId INT, DailyRate float, CurrentDays int)
    -- Insert statements for procedure here
	INSERT INTO @users
	        ( FirstName ,
			  LastName,
	          PhoneNumber ,
	          Gender ,
	          JoinDate ,
	          ProfileIMG ,
	          UserId ,
			  Rate,
			  ColorCode,
	          CurrentNumClasses ,
	          CurrentEarnings ,
			  LastClassDate,
			  NextClassDate,
			  StudioId,
			  DailyRate, 
			  CurrentDays
	        )
	SELECT  u.FirstName,
							u.LastName,
                            u.PhoneNumber,
                            u.Gender,
                            u.JoinDate,
                            u.ProfileIMG,
                            u.Id UserId, 
							i.Rate,
							i.ColorCode,
							isNull(MonthlyClasses.NumClasses,0), 
							isNull(i.Rate * MonthlyDoneClasses.NumClasses,0) , 
							LastClass.Last LastClass, 
							NextClass.Next NextClass,
							u.StudioId,
							i.DailyRate,
							isnull(MonthlyDays.Numdays, 0)
							
FROM dbo.AspNetUsers u
INNER JOIN dbo.AspNetUserRoles r ON r.UserId = u.Id AND r.RoleId in (SELECT Id FROM [dbo].[AspNetRoles] WHERE Name LIKE '%Instructor%')
inner join [dbo].[InstructorDetails] i on i.InstructorId = u.Id
--INNER JOIN dbo.Studio s ON s.Id = u.StudioId 
--INNER JOIN dbo.AspNetRoles roles ON roles.Id = r.RoleId
LEFT JOIN (SELECT InstructorId, MAX(c.Date) Last FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETUTCDATE() GROUP BY ce.InstructorId) LastClass ON LastClass.InstructorId = u.Id
LEFT JOIN (SELECT InstructorId, MIN(c.Date) Next FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date > GETUTCDATE() GROUP BY InstructorId) NextClass ON NextClass.InstructorId = u.Id
LEFT JOIN (SELECT InstructorId, count(distinct cast(c.date as date)) Numdays FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date >= @fromDate and c.Date < GETUTCDATE() and c.isdeleted = 0 GROUP BY ce.InstructorId) MonthlyDays ON MonthlyDays.InstructorId = u.Id
LEFT JOIN (SELECT InstructorId, count(1) NumClasses FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date >= @fromDate and c.Date < @toDate GROUP BY ce.InstructorId) MonthlyClasses ON MonthlyClasses.InstructorId = u.Id
LEFT JOIN (SELECT InstructorId, count(1) NumClasses FROM dbo.Class_Instructors ce inner join Class c on ce.ClassId = c.Id WHERE c.Date >= @fromDate and c.Date < GETUTCDATE() GROUP BY ce.InstructorId) MonthlyDoneClasses ON MonthlyDoneClasses.InstructorId = u.Id
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


		SELECT * FROM @users ORDER BY CurrentEarnings DESC


END
