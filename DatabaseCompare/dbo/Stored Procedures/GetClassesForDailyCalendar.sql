-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetClassesForDailyCalendar
	-- Add the parameters for the stored procedure here
	@Date DATE = NULL,
	@StudioId INT = 0,
	@ClassId INT = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select
	 c.*, ct.Name ClassTypeName, ct.Picture, ct.BGColor, ctd.Name, ctd.Description, sr.Name StudioRoomName,
	  ct.Description ClassTypeDescription, ctd.Name ClassTypeDetailsName
	 ,STUFF((SELECT '; ' + u.Id 
    FROM Class_Instructors ci
	join AspNetUsers u on u.Id = ci.InstructorId
    WHERE ci.ClassId = c.Id
    FOR XML PATH('')), 1, 1, '') InstructorIds
	from Class c
	join ClassType ct on c.ClassTypeId = ct.Id
	join StudioRoom sr on c.StudioRoomId = sr.Id
	left join ClassTypeDetails ctd on ctd.ClassTypeId = ct.Id
	--left join Class_Instructors ci on ci.ClassId = c.Id
	where c.IsDeleted = 0 and c.Published = 1
	and cast(c.Date as date) = case when @ClassId > 0 then cast(c.Date as date) else @Date end
	and  sr.StudioId = @StudioId
	and c.id = case when @ClassId > 0 then @ClassId else c.Id end 

END