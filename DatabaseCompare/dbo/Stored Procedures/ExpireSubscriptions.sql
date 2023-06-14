-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ExpireSubscriptions] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	--UPDATE dbo.UserSubscription
	--SET Active = 0
	--WHERE DateExpire < CAST(GETUTCDATE() AS date)


	-- expire subscription when date expired
	--UPDATE trg
	--SET trg.Active = 0
	--FROM dbo.UserSubscription trg
	--INNER JOIN (SELECT ISNULL(MAX(Date),DATEADD(DAY, -1,GETUTCDATE())) ClassDate ,SubscriptionId FROM dbo.ClassEnrollment
	--			INNER JOIN dbo.Class ON Class.Id = ClassEnrollment.ClassId
	--			WHERE ClassEnrollment.IsDeleted = 0
	--			GROUP BY SubscriptionId
	--) ce ON ce.SubscriptionId = trg.Id
	--WHERE ce.ClassDate < CAST(GETUTCDATE() AS date) AND trg.DateExpire < CAST(GETUTCDATE() AS date)
	
	-- expire subscription when date expired (incl. unlimited subscriptions)
	 UPDATE dbo.UserSubscription
	SET Active = 0
	WHERE DateExpire IS NOT NULL and DateExpire < CAST(GETUTCDATE() AS date) AND Frozen = 0  and Active = 1 --AND NumClasses = 0
	

	-- expire subscription when balance is over (on last class)
	UPDATE trg
	SET trg.Active = 0, DateExpire = casT(ClassDate as date)
	FROM dbo.UserSubscription trg
	INNER JOIN dbo.SubscriptionType t ON t.Id = trg.SubscriptionTypeId
	INNER JOIN (SELECT ISNULL(MAX(Date),DATEADD(DAY, -1,GETUTCDATE())) ClassDate ,SubscriptionId FROM dbo.ClassEnrollment
				INNER JOIN dbo.Class ON Class.Id = ClassEnrollment.ClassId
				WHERE ClassEnrollment.IsDeleted = 0
				GROUP BY SubscriptionId
	) ce ON ce.SubscriptionId = trg.Id
	WHERE ce.ClassDate < CAST(GETUTCDATE() AS date) AND trg.CurrentBalance <= 0 AND t.NumClasses > 0 and trg.Active = 1


	EXEC UnFreezeSubscriptions
	exec UpdateFirstSubscription

	-- update user details when subscription is expired
	--UPDATE trg1
	--SET trg1.Active = src.Active
 --   FROM dbo.AspNetUsers trg1
	--INNER JOIN dbo.UserSubscription src ON src.UserId = trg1.Id
	--WHERE src.DateExpire > GETUTCDATE() AND src.CurrentBalance > 0
	

		--select us.*, u.id, u.fullname, u.Active
	update u set u.Active = us.Active
	from AspNetUsers u
	left join (select id, active, userid from UserSubscription where id in (select max(id) from UserSubscription group by userid)) us on us.UserId = u.Id
	where u.Active <> us.Active


	--update classes done after class only (end of day or in the 2 hours before when can not cancel maybe) 


	delete from [dbo].[UserProcessing]
	where UserId in (select userid 
					 from ClassEnrollment ce inner join UserSubscription us on ce.SubscriptionId = us.Id
					 where cast(ce.DateEnrolled as date) = DATEADD(day, -1, CAST(GETUTCDATE() AS date)) and ce.IsDeleted = 0)
END
