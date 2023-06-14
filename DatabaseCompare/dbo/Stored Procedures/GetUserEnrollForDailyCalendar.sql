
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetUserEnrollForDailyCalendar]
	-- Add the parameters for the stored procedure here
	@Date DATE = NULL,
	--@StudioId INT = 0,
	@UserId nvarchar(128) = 0,
	@Next bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @Next = 0
	begin 
		if  @Date is NULL
			begin
				select ce.*, ap.StudioPlacementName, ap.ClassPlacementNumber, ap.StudioPlacementId
				, c.Name ClassName, c.Date ClassDate, c.Participants ClassParticipants, us.Active SubscriptionActive
				from ClassEnrollment ce
				left join ClassAvailablePlacement ap on ap.Id = ce.ClassAvailablePlacementId
				join UserSubscription us on us.Id = ce.SubscriptionId
				join AspNetUsers u on u.Id = us.UserId
				join Class c on c.id = ce.ClassId 
				where ce.IsDeleted = 0
				and u.id = @UserId
				order by c.Date desc
			end
			else
			begin
				select ce.*, ap.StudioPlacementName, ap.ClassPlacementNumber, ap.StudioPlacementId
				from ClassEnrollment ce
				left join ClassAvailablePlacement ap on ap.Id = ce.ClassAvailablePlacementId
				join UserSubscription us on us.Id = ce.SubscriptionId
				join AspNetUsers u on u.Id = us.UserId
				join Class c on c.id = ce.ClassId and cast(c.date as date) = @Date 
				where ce.IsDeleted = 0
				and u.id = @UserId
				order by c.Date desc
				
			end
	--and cast(c.Date as date) = case when @ClassId > 0 then cast(c.Date as date) else @Date end
	--and  sr.StudioId = @StudioId
	--and c.id = case when @ClassId > 0 then @ClassId else c.Id end 
	end
	else
	begin
	select ce.*, ap.StudioPlacementName, ap.ClassPlacementNumber, ap.StudioPlacementId, c.Date as ClassDate, c.Name as ClassName
	from ClassEnrollment ce
	left join ClassAvailablePlacement ap on ap.Id = ce.ClassAvailablePlacementId
	join UserSubscription us on us.Id = ce.SubscriptionId
	join AspNetUsers u on u.Id = us.UserId
	join Class c on c.id = ce.ClassId --and cast(c.date as date) = @Date 
	where ce.IsDeleted = 0 and c.Date > @Date
	and u.id = @UserId
	order by c.Date
	end
END