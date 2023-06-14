-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetUsersListFor]
	@RefId INT = NULL,
    @RefDate DATETIME = NULL,
	@StudioId int
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @fromDate DATE = (SELECT DATEADD(ww, DATEDIFF(ww,-1,GETDATE()), -1))
	DECLARE @toDate DATETIME = GETDATE()
	
	DECLARE @users TABLE (FirstName NVARCHAR(128), LastName NVARCHAR(128), PhoneNumber NVARCHAR(max), Gender INT, JoinDate DATETIME, ProfileIMG NVARCHAR(250),UserId NVARCHAR(128), SubscriptionId INT, SubscriptionStartDate DATETIME, 
	SubscriptionExpireDate DATETIME, Active bit, NumClasses int, CurrentBalance int, LastClassDate DATETIME, LastClassType NVARCHAR(250) ,NextClassDate DATETIME,NextClassType NVARCHAR(250), Ticked BIT, WeeklyClasses INT, UserType INT, CompanyId int, StudioId int)
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
			  UserType,
			  CompanyId,
			  StudioId
	        )
	SELECT  u.FirstName,
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
							CAST(r.RoleId AS INT),
							s.CompanyId,
							s.Id
							
FROM dbo.AspNetUsers u
INNER JOIN dbo.AspNetUserRoles r ON r.UserId = u.Id AND r.RoleId in (SELECT Id FROM [dbo].[AspNetRoles] WHERE Name LIKE '%User%') --AND r.RoleId = '2'
INNER JOIN dbo.UserSubscription us1 ON us1.UserId = u.Id
INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = us1.Id
INNER JOIN dbo.Class c ON c.Id = ce.ClassId
inner join [dbo].[Studio]s on s.Id = u.StudioId and s.Id = @StudioId
LEFT JOIN 
(SELECT * FROM 
dbo.UserSubscription 
WHERE Active = 1) us ON us.UserId = u.Id
LEFT JOIN (SELECT UserId FROM UserDailyTick WHERE CAST(Date AS date) = CAST(GETDATE() AS date)) t ON u.Id = t.UserId
LEFT JOIN (SELECT SubscriptionId, MAX(c.Date) Last FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETDATE() AND ce.IsDeleted = 0 GROUP BY SubscriptionId) LastClass ON LastClass.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, MIN(c.Date) Next FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date > GETDATE() AND ce.IsDeleted = 0 GROUP BY SubscriptionId) NextClass ON NextClass.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, count(1) Weekly FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date >= @fromDate and c.Date <= @toDate and ce.IsDeleted = 0 GROUP BY SubscriptionId
) Classes ON Classes.SubscriptionId = us.Id
WHERE CAST(c.Date AS DATE) = CASE WHEN @RefDate IS NULL THEN CAST(c.Date AS date) ELSE @RefDate END
AND c.Id = CASE WHEN @RefId IS NULL THEN c.Id ELSE @RefId END
AND ce.IsDeleted = 0
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


		SELECT * FROM @users ORDER BY Active DESC,  SubscriptionExpireDate

END
