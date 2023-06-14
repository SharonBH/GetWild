-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetLastClass]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--truncate table LastClass
	delete from LastClass
	where RowDate < CAST(DATEADD(dd, -7, GETUTCDATE() )AS date)

	IF NOT EXISTS
        (
        SELECT 1
        FROM LastClass
        WHERE RowDate = cast(GETUTCDATE() as date)
        )
		begin
	insert into LastClass (rowDate,userid, date)
	select cast(GETUTCDATE() as date) --cast(DATEADD(dd, -1, GETUTCDATE()) as date)
	,trg.Id, Last
	FROM dbo.AspNetUsers trg
	INNER JOIN dbo.UserSubscription u ON u.UserId = trg.Id and u.Active = 1
	--INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = u.Id
	INNER JOIN (SELECT SubscriptionId, MAX(c.Date) Last FROM dbo.ClassEnrollment ce 
	inner join Class c on ce.ClassId = c.Id WHERE c.Date < cast(GETUTCDATE() as date) AND ce.IsDeleted = 0 and ce.IsVerified = 1 GROUP BY SubscriptionId) LastClass ON LastClass.SubscriptionId = u.Id
	--INNER JOIN (select id, max(date) from dbo.Class 
	--WHERE --Last >= CAST(DATEADD(dd, -3, GETUTCDATE() )AS date) AND 
	--Last < GETUTCDATE()

    -- Insert statements for procedure here
	UPDATE trg
	SET trg.LastClass = Last
	FROM dbo.AspNetUsers trg
	INNER JOIN dbo.UserSubscription u ON u.UserId = trg.Id --and u.Active = 1
	--INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = u.Id
	INNER JOIN (SELECT SubscriptionId, MAX(c.Date) Last FROM dbo.ClassEnrollment ce 
	inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETUTCDATE() AND ce.IsDeleted = 0 and ce.IsVerified = 1 GROUP BY SubscriptionId) LastClass ON LastClass.SubscriptionId = u.Id
	--INNER JOIN (select id, max(date) from dbo.Class 
	WHERE Last >= CAST(DATEADD(dd, -3, GETUTCDATE() )AS date) AND Last < GETUTCDATE()

 --   FROM dbo.AspNetUsers trg
	--INNER JOIN dbo.UserSubscription u ON u.UserId = trg.Id
	--INNER JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = u.Id
	--INNER JOIN dbo.Class c ON c.Id = ce.ClassId
	--WHERE c.Date >= CAST(DATEADD(dd, -1, GETUTCDATE() )AS date) AND c.Date < GETUTCDATE()
	--AND ce.IsDeleted = 0
	end


END
