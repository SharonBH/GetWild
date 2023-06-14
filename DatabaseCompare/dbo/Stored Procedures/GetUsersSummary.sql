-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetUsersSummary]
	-- Add the parameters for the stored procedure here
	@StudioId int = 0,
	@RemoveMarked bit = 0

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

select cast (r.Id as int) UserType, count(u.id) Total, sum(case when u.Active = 1 then 1 else 0 end) Active, sum(case when us.Frozen = 1 then 1 else 0 end) Frozen,
sum(case when u.Active = 1 and cast(DateExpireOriginal as date) >= cast(getutcdate() as date) then 1 else 0 end) Paying
from AspNetUsers u
left join UserSubscription us on us.UserId = u.Id and us.Active = 1
join AspNetUserRoles ur on u.Id = ur.UserId
join AspNetRoles r on r.Id = ur.RoleId
--join Studio s on s.Id = u.StudioId
where r.Id between 2 and 10
and u.StudioId = @StudioId
--and u.Marked = case when @RemoveMarked = 1 then 0 else u.Marked end
group by r.Id
order by UserType, Total desc

END