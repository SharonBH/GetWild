-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DailySystemAlerts] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @alerts TABLE (userid NVARCHAR(128), MsgType INT)
    -- Insert statements for procedure here

	--Get almost expitred subscriptions
	INSERT INTO @alerts
	      SELECT us.UserId, 1
		  FROM dbo.UserSubscription us
		  WHERE us.DateExpire = CAST(DATEADD(DAY, 3, GETDATE()) AS DATE) AND us.DateExpire >= GETDATE() and us.Active = 1
		  --WHERE us.DateExpire <= CAST(DATEADD(DAY, 7, GETDATE()) AS DATE) AND us.DateExpire >= GETDATE()

--Get almost finshed subscriptions
	INSERT INTO @alerts
	      SELECT us.UserId, 5
		  FROM dbo.UserSubscription us
		  inner join subscriptionType st on us.[SubscriptionTypeId] = st.Id
		  WHERE us.CurrentBalance <= 3 AND us.Active = 1 and st.[NumClasses]  > 1 --exclude trails



	--Get Inactive subscriptions
	INSERT INTO @alerts
	      SELECT us.Id, 4
		  FROM dbo.AspNetUsers us
		  WHERE us.Active = 1 AND us.LastClass = CAST(DATEADD(DAY, -7, GETDATE()) AS DATE)
		  --WHERE us.Active = 1 AND us.LastClass <= CAST(DATEADD(DAY, -7, GETDATE()) AS DATE)

		  --remove (read) old msmgs
		  update src
		  set IsRead = 1
		  from SysAlerts src join @alerts a on src.UserId = a.userid and src.TypeId = a.MsgType

		  INSERT INTO SysAlerts
		  (userId, Message, Date, TypeId) 
		  SELECT userid, mt.Message , GETDATE(), a.MsgType
		  FROM @alerts a
		  INNER JOIN [dbo].[MSGTypes] mt ON mt.Id = a.MsgType

END
