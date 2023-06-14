-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateFirstSubscription]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	update UserSubscription
	set IsFirst = 1
	where id in (select min(id) from UserSubscription where SubscriptionTypeId in (select id from SubscriptionType where NumClasses != 1 and IsDeleted = 0) group by UserId )
END