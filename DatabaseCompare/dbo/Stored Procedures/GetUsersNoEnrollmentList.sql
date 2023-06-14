-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetUsersNoEnrollmentList]
	
	-- Add the parameters for the stored procedure here
	@StudioId INT = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @fromDate DATETIME = (SELECT DATEADD(ww, DATEDIFF(ww,-1,GETUTCDATE()), -1))
	DECLARE @toDate DATETIME = DATEADD(DAY, 6, @fromDate) --GETUTCDATE()
	
	DECLARE @users TABLE (FirstName NVARCHAR(128), LastName NVARCHAR(128), PhoneNumber NVARCHAR(max), Gender INT, JoinDate DATETIME, ProfileIMG NVARCHAR(250),UserId NVARCHAR(128), SubscriptionId INT, SubscriptionStartDate DATETIME, 
						  SubscriptionExpireDate DATETIME, Active bit, NumClasses int, CurrentBalance int, LastClassDate DATETIME, LastClassType NVARCHAR(250) ,NextClassDate DATETIME,NextClassType NVARCHAR(250), Ticked BIT, WeeklyClasses INT, 
						  ClassesDone INT,ClassesMissed INT, SubscriptionType NVARCHAR(512), Frozen bit, UserType INT, StudioId INT)
    -- Insert statements for procedure here
	INSERT INTO @users
	        ( FirstName ,
			  LastName,
	          PhoneNumber ,
	          Gender ,
	          JoinDate ,
	          ProfileIMG ,
	          UserId ,
			  SubscriptionId,
	          SubscriptionStartDate ,
	          SubscriptionExpireDate ,
	          Active ,
	          NumClasses ,
	          CurrentBalance ,
	          Ticked ,
	          WeeklyClasses,
			  LastClassDate,
			  NextClassDate,
			  ClassesDone,
			  ClassesMissed,
			  SubscriptionType, 
			  Frozen,
			  UserType,
			  StudioId
	        )
	SELECT					u.FirstName,
							u.LastName,
                            u.PhoneNumber,
                            u.Gender,
                            u.JoinDate,
                            u.ProfileIMG,
                            u.Id UserId, 
							us.Id,
							us.DateSubscribed SubscriptionStartDate, 
							us.DateExpire SubscriptionExpireDate, 
							us.Active,
							us.NumClasses, 
							us.CurrentBalance , 
							CAST(CASE WHEN t.UserId IS NOT NULL THEN 1 ELSE 0 END AS BIT) Ticked,
							ISNULL(Classes.Weekly,0) WeeklyClasses,
							LastClass.Last LastClass, 
							NextClass.Next NextClass,
							ISNULL(ClassesDone.Done,0),
							ISNULL(ClassesDone.Missed,0),
							st.Name,
							us.Frozen,
							CAST(r.RoleId AS INT),
							u.StudioId
							
FROM dbo.AspNetUsers u
INNER JOIN dbo.AspNetUserRoles r ON r.UserId = u.Id AND r.RoleId in (SELECT Id FROM [dbo].[AspNetRoles] WHERE Id = '2')	--Name LIKE '%User%'
--INNER JOIN dbo.Studio s ON s.Id = u.StudioId
JOIN (SELECT * FROM dbo.UserSubscription WHERE Active = 1) us ON us.UserId = u.Id
LEFT JOIN (SELECT UserId FROM UserDailyTick WHERE CAST(Date AS date) = CAST(GETUTCDATE() AS date)) t ON u.Id = t.UserId
LEFT JOIN (SELECT SubscriptionId, MAX(c.Date) Last FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETUTCDATE() AND ce.IsDeleted = 0 GROUP BY SubscriptionId) LastClass ON LastClass.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, MIN(c.Date) Next FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date > GETUTCDATE() AND ce.IsDeleted = 0 GROUP BY SubscriptionId) NextClass ON NextClass.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, count(1) Weekly FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date >= @fromDate and c.Date <= @toDate and ce.IsDeleted = 0 GROUP BY SubscriptionId
) Classes ON Classes.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, count(1) Done, SUM(CASE WHEN ce.IsVerified = 1 THEN 0 ELSE 1 end) Missed FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETUTCDATE() and ce.IsDeleted = 0 GROUP BY SubscriptionId
) ClassesDone ON ClassesDone.SubscriptionId = us.Id
LEFT JOIN dbo.SubscriptionType st ON st.Id = us.SubscriptionTypeId
WHERE u.StudioId = @StudioId
and (NextClass.Next IS NULL OR NextClass.Next >= DATEADD(DAY,1,@toDate)) AND (LastClass.Last IS NULL OR LastClass.Last < @fromDate)
order by us.Active desc,  SubscriptionExpireDate

UPDATE trg
SET trg.LastClassType = ct.Name
FROM @users trg
INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = trg.SubscriptionId
INNER JOIN dbo.Class c ON c.Id = ce.ClassId
INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId
WHERE ce.SubscriptionId = trg.SubscriptionId AND c.Date = trg.LastClassDate

UPDATE trg
SET trg.NextClassType = ct.Name
FROM @users trg
INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = trg.SubscriptionId
INNER JOIN dbo.Class c ON c.Id = ce.ClassId
INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId
WHERE ce.SubscriptionId = trg.SubscriptionId AND c.Date = trg.NextClassDate


		SELECT * FROM @users 
		--WHERE (NextClassDate IS NULL OR NextClassDate >= DATEADD(DAY,1,@toDate)) AND (LastClassDate IS NULL OR LastClassDate < @fromDate)
		--AND Active = 1
		ORDER BY  SubscriptionExpireDate

END
