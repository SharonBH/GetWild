-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CopyWeeklyClander]
	-- Add the parameters for the stored procedure here
	@startDate DATETIME = NULL,
	@AutoPublish bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @RESULT bit = 0

    IF @startDate IS NOT NULL
	BEGIN
		DECLARE @EndDate DATE = DATEADD(DAY, 7, @startDate)
			--current week 
			IF NOT EXISTS(select 1 from dbo.Class WHERE Date >= @startDate AND Date < @EndDate AND IsDeleted = 0) 
				BEGIN
					set @EndDate = @startDate
					set @startDate = DATEADD(DAY, -7, @startDate)
				End
			--next week 
			ELSE IF NOT EXISTS(select 1 from dbo.Class WHERE Date >= DATEADD(DAY, 7, @startDate) AND Date < DATEADD(DAY, 7, @EndDate) AND IsDeleted = 0)
				Begin
					set @EndDate = DATEADD(DAY, 7, @startDate)
					--set @startDate = DATEADD(DAY, -7, @startDate)
				End
			else
				Begin
					set @EndDate = null
					set @startDate = null
				End

    IF @startDate IS NOT NULL
		BEGIN

			INSERT INTO dbo.Class
					( Date ,
					  ClassTypeId ,
					  StudioRoomId ,
					  Name ,
					  Description ,
					  Duration ,
					  MaxParticipants ,
					  Participants ,
					  DailySlotId,
					  IsForMale ,
					  IsForFemale ,
					  IsDeleted,
					[IsMultiRoom],
					MaxExtraParticipants,
					Published
					)
			SELECT 
				DATEADD(DAY, 7, Date),
				ClassTypeId ,
				StudioRoomId ,
				Name ,
				Description ,
				Duration ,
				MaxParticipants ,
				0 ,
				DailySlotId,
				IsForMale ,
				IsForFemale ,
				IsDeleted,
				[IsMultiRoom],
				MaxExtraParticipants,
				@AutoPublish
			FROM dbo.Class
			WHERE Date >= @startDate AND Date < @EndDate AND IsDeleted = 0

			--insert instructors
			insert into [dbo].[Class_Instructors] (classid, instructorid)
			select c1.id, i.[InstructorId]
			from class c1
			inner join Class c2 on c1.DailySlotId = c2.DailySlotId and c1.Date = dateadd(day,7,c2.Date)
			inner join [dbo].[Class_Instructors] i on i.classid = c2.id
			where c1.date >= DATEADD(day,7, @startDate) AND c1.Date < DATEADD(day,7,@EndDate) and c1.StudioRoomId = c2.StudioRoomId

			--insert placements
			insert into ClassAvailablePlacement (ClassId, StudioPlacementId, StudioPlacementName, ClassPlacementNumber, IsDeleted, IsInUse)
select c1.id, i.StudioPlacementId, i.StudioPlacementName, i.ClassPlacementNumber, i.IsDeleted, 0
			from class c1
			inner join Class c2 on c1.DailySlotId = c2.DailySlotId and c1.Date = dateadd(day,7,c2.Date) and c1.StudioRoomId = c2.StudioRoomId
			inner join [dbo].ClassAvailablePlacement i on i.classid = c2.id
			where c1.date >= DATEADD(day,7, @startDate) AND c1.Date < DATEADD(day,7,@EndDate) 

			SET @RESULT = 1
		END
	END

	SELECT @RESULT
END
