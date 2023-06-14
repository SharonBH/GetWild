
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetNewUsersList]
	@Days INT = -1, 
	@StudioId INT = 0
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @fromDate DATE = dateadd(day,@Days, GETUTCDATE())

	
	DECLARE @newusers TABLE (FirstName NVARCHAR(128), LastName NVARCHAR(128), PhoneNumber NVARCHAR(max), Gender INT, JoinDate DATETIME, ProfileIMG NVARCHAR(250),UserId NVARCHAR(128), SubscriptionId INT, SubscriptionStartDate DATETIME, 
	SubscriptionExpireDate DATETIME, Active bit, NumClasses int, CurrentBalance int, NextClassDate DATETIME, ClassesDone INT,SubscriptionType NVARCHAR(512), Frozen BIT DEFAULT(0), UserType INT, StudioId INT, NextClassType NVARCHAR(250))
    -- Insert statements for procedure here
	INSERT INTO @newusers
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
			  NextClassDate,
			  ClassesDone,
			  SubscriptionType, 
			  Frozen,
			  UserType,
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
							NextClass.Next NextClass,
							ISNULL(ClassesDone.Done,0),
							st.Name,
							ISNULL(us.Frozen,0),
							CAST(r.RoleId AS INT),
							u.StudioId
							
FROM dbo.AspNetUsers u
INNER JOIN dbo.AspNetUserRoles r ON r.UserId = u.Id AND r.RoleId in (SELECT Id FROM [dbo].[AspNetRoles] WHERE NAME LIKE '%User%') --Id = '2')
--INNER JOIN dbo.Studio s ON s.Id = u.StudioId 
--INNER JOIN dbo.AspNetRoles roles ON roles.Id = r.RoleId
LEFT JOIN 
(SELECT * FROM 
dbo.UserSubscription 
WHERE Active = 1) us ON us.UserId = u.Id
--LEFT JOIN (SELECT UserId FROM UserDailyTick WHERE CAST(Date AS date) >= DATEADD(ww, DATEDIFF(ww,-1,GETUTCDATE()), -1)) t ON u.Id = t.UserId
--LEFT JOIN (SELECT SubscriptionId, MAX(c.Date) Last FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETUTCDATE() AND ce.IsDeleted = 0 GROUP BY SubscriptionId) LastClass ON LastClass.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, MIN(c.Date) Next FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date > GETUTCDATE() AND ce.IsDeleted = 0 GROUP BY SubscriptionId) NextClass ON NextClass.SubscriptionId = us.Id
--LEFT JOIN (SELECT SubscriptionId, count(1) Weekly FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE cast(c.Date as date) >= @fromDate and cast(c.Date as date) <= @toDate and ce.IsDeleted = 0 GROUP BY SubscriptionId
--) Classes ON Classes.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, count(1) Done, SUM(CASE WHEN ce.IsVerified = 1 THEN 0 ELSE 1 end) Missed FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETUTCDATE() and ce.IsDeleted = 0 GROUP BY SubscriptionId
) ClassesDone ON ClassesDone.SubscriptionId = us.Id
LEFT JOIN dbo.SubscriptionType st ON st.Id = us.SubscriptionTypeId
WHERE u.StudioId = @StudioId
and us.DateSubscribed >= @fromDate
order by us.Active desc,  SubscriptionExpireDate

UPDATE trg
SET trg.NextClassType = ct.Name
FROM @newusers trg
INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = trg.SubscriptionId
INNER JOIN dbo.Class c ON c.Id = ce.ClassId
INNER JOIN dbo.ClassType ct ON ct.Id = c.ClassTypeId
WHERE ce.SubscriptionId = trg.SubscriptionId AND c.Date = trg.NextClassDate


SELECT * FROM @newusers ORDER BY Active DESC,  JoinDate



END
