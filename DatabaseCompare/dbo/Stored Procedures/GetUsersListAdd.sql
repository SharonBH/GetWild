-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[GetUsersListAdd]
	@DayNo INT = -1, 
	@StartDate DATE = NULL,
	@CompanyId INT = 0,
	@PageNo int = 1,
	@UserType int = 0,
	@Frozen bit = 0
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @fromDate DATE = (SELECT DATEADD(ww, DATEDIFF(ww,-1,GETUTCDATE()), -1))
	DECLARE @toDate DATETIME = GETUTCDATE()
	declare @pageSize int = 100

	IF (@StartDate IS NOT NULL)
		BEGIN
			SET @fromDate = @StartDate
			SET @toDate = case when DATEADD(day, 6,@StartDate) > GETUTCDATE() then GETUTCDATE() else DATEADD(DAY, 6, @StartDate) end
		END
	--ELSE
	--	BEGIN
	--		SET @fromDate = (SELECT DATEADD(ww, DATEDIFF(ww,-1,GETUTCDATE()), -1))
	--		SET @toDate = GETUTCDATE()
	--	END
	
	--DECLARE @users TABLE (FirstName NVARCHAR(128), LastName NVARCHAR(128), PhoneNumber NVARCHAR(max), Gender INT, JoinDate DATETIME, ProfileIMG NVARCHAR(250),UserId NVARCHAR(128), SubscriptionId INT, SubscriptionStartDate DATETIME, 
	--					  SubscriptionExpireDate DATETIME, Active bit, NumClasses int, CurrentBalance int, LastClassDate DATETIME, LastClassType NVARCHAR(250) ,NextClassDate DATETIME,NextClassType NVARCHAR(250), Ticked BIT, WeeklyClasses INT, 
	--					  ClassesDone INT,ClassesMissed INT, SubscriptionType NVARCHAR(512), Frozen BIT DEFAULT(0), UserType INT, StudioId INT, OrderCol int)
 --   -- Insert statements for procedure here
	--INSERT INTO @users
	--        ( FirstName ,
	--		  LastName,
	--          PhoneNumber ,
	--          Gender ,
	--          JoinDate ,
	--          ProfileIMG ,
	--          UserId ,
	--		  SubscriptionId,
	--          SubscriptionStartDate ,
	--          SubscriptionExpireDate ,
	--          Active ,
	--          NumClasses ,
	--          CurrentBalance ,
	--          Ticked ,
	--          WeeklyClasses,
	--		  LastClassDate,
	--		  NextClassDate,
	--		  ClassesDone,
	--		  ClassesMissed,
	--		  SubscriptionType, 
	--		  Frozen,
	--		  UserType,
	--		  StudioId,
	--		  OrderCol
	--        )
	SELECT  u.FirstName,
							u.LastName,
                            u.PhoneNumber,
                            u.Gender,
                            u.JoinDate,
                            u.ProfileIMG,
							u.DOB,
							u.address,
                            u.Id UserId, 
							us.Id,
							us.DateSubscribed SubscriptionStartDate, 
							us.DateExpire SubscriptionExpireDate,
							us.DateExpireOriginal PayEndDate,  
							isnull(us.Active, 0) Active,
							us.NumClasses, 
							us.CurrentBalance , 
							CAST(CASE WHEN t.UserId IS NOT NULL THEN 1 ELSE 0 END AS BIT) Ticked,
							ISNULL(Classes.Weekly,0) WeeklyClasses,
							LastClass.Last LastClassDate, 
							LastClass.LastType LastClassType,
							NextClass.Next NextClassDate,
							NextClass.NextType NextClassType,
							ISNULL(ClassesDone.Done,0) ClassesDone,
							ISNULL(ClassesDone.Missed,0) ClassesMissed,
							st.Name SubscriptionType,
							ISNULL(us.Frozen,0) frozen,
							CAST(r.RoleId AS INT) UserType,
							u.StudioId,
							isnull(case when us.DateExpire > GETUTCDATE() or us.Active = 1 then 0 else DATEDIFF(DAY,us.DateExpire, GETUTCDATE()) end, 9999) OrderCol
							
FROM dbo.AspNetUsers u
INNER JOIN dbo.AspNetUserRoles r ON r.UserId = u.Id AND r.RoleId in (SELECT Id FROM [dbo].[AspNetRoles] WHERE Name LIKE '%User%')
INNER JOIN dbo.Studio s ON s.Id = u.StudioId 
--INNER JOIN dbo.AspNetRoles roles ON roles.Id = r.RoleId
LEFT JOIN (SELECT * FROM dbo.UserSubscription t WHERE t.id = (select max(Id) from UserSubscription where UserId = t.UserId)) us ON us.UserId = u.Id
LEFT JOIN (SELECT UserId FROM UserDailyTick WHERE CAST(Date AS date) >= DATEADD(ww, DATEDIFF(ww,-1,GETUTCDATE()), -1)) t ON u.Id = t.UserId
LEFT JOIN (SELECT SubscriptionId, MAX(c.Date) Last, max(ct.Name) LastType FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId WHERE c.Date < GETUTCDATE() AND ce.IsDeleted = 0 GROUP BY SubscriptionId) LastClass ON LastClass.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, MIN(c.Date) Next, max(ct.Name) NextType FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId WHERE c.Date > GETUTCDATE() AND ce.IsDeleted = 0 GROUP BY SubscriptionId) NextClass ON NextClass.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, count(1) Weekly FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE cast(c.Date as date) >= @fromDate and cast(c.Date as date) <= @toDate and ce.IsDeleted = 0 GROUP BY SubscriptionId
) Classes ON Classes.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, count(1) Done, SUM(CASE WHEN ce.IsVerified = 1 THEN 0 ELSE 1 end) Missed FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETUTCDATE() and ce.IsDeleted = 0 GROUP BY SubscriptionId
) ClassesDone ON ClassesDone.SubscriptionId = us.Id
LEFT JOIN dbo.SubscriptionType st ON st.Id = us.SubscriptionTypeId
WHERE s.CompanyId = @CompanyId and --isnull(us.Frozen,0) = case when @Frozen = 1 then 1 else isnull(us.Frozen,0) end
us.Frozen = case when @Frozen = 1 then 1 else us.Frozen end
and r.RoleId = case when @UserType = 0 then r.RoleId else @UserType end
and ISNULL(Classes.Weekly,0) = case when @DayNo >= 0 AND @DayNo < 7 then @DayNo else ISNULL(Classes.Weekly,0) end 
order by Frozen, ordercol, us.Active desc,  SubscriptionExpireDate

--OFFSET ((@PageNo-1) * @pageSize) ROWS FETCH NEXT (@pageSize) ROWS ONLY


--UPDATE trg
--SET trg.LastClassType = ct.Name
--FROM @users trg
--INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = trg.SubscriptionId
--INNER JOIN dbo.Class c ON c.Id = ce.ClassId
--INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId
--WHERE ce.SubscriptionId = trg.SubscriptionId AND c.Date = trg.LastClassDate

--UPDATE trg
--SET trg.NextClassType = ct.Name
--FROM @users trg
--INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = trg.SubscriptionId
--INNER JOIN dbo.Class c ON c.Id = ce.ClassId
--INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId
--WHERE ce.SubscriptionId = trg.SubscriptionId AND c.Date = trg.NextClassDate

--IF (@DayNo >= 0 AND @DayNo < 7)
--	BEGIN
--		SELECT * FROM @users WHERE WeeklyClasses = @DayNo --ORDER BY Active DESC,  SubscriptionExpireDate
--		order by Frozen, ordercol, Active desc,  SubscriptionExpireDate
--	END
--ELSE IF (@DayNo = 7)
--    BEGIN
--    	SELECT * FROM @users WHERE WeeklyClasses >= @DayNo --ORDER BY Active DESC,  SubscriptionExpireDate
--		order by Frozen, ordercol, Active desc,  SubscriptionExpireDate
--    END
--ELSE
--	BEGIN
--		SELECT * FROM @users --ORDER BY Active DESC,  SubscriptionExpireDate
--		order by Frozen, ordercol, Active desc,  SubscriptionExpireDate
--	END


END