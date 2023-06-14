-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CopyWeeklyClanderForCompany]
	-- Add the parameters for the stored procedure here
	@startDate DATETIME = NULL,
	@AutoPublish bit = 1,
	@StudioId int = 0
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
			IF NOT EXISTS(select 1 from dbo.Class c 
							join StudioRoom sr on sr.Id = c.StudioRoomId
							--join Studio s on s.Id = sr.StudioId
							WHERE Date >= @startDate AND Date < @EndDate AND c.IsDeleted = 0 and sr.StudioId = @StudioId) 
				BEGIN
					set @EndDate = @startDate
					set @startDate = DATEADD(DAY, -7, @startDate)
				End
			--next week 
			ELSE IF NOT EXISTS(select 1 from dbo.Class c 
							join StudioRoom sr on sr.Id = c.StudioRoomId
							--join Studio s on s.Id = sr.StudioId 
							WHERE Date >= DATEADD(DAY, 7, @startDate) AND Date < DATEADD(DAY, 7, @EndDate) AND c.IsDeleted = 0 and sr.StudioId = @StudioId) 
				Begin
					set @EndDate = DATEADD(DAY, 7, @startDate)
					--set @startDate = DATEADD(DAY, -7, @startDate)
				End
			else
				Begin
					set @EndDate = null
					set @startDate = null
				End

				--select @startDate, @EndDate, @StudioId

    IF @startDate IS NOT NULL and @StudioId > 0
		BEGIN
		IF OBJECT_ID('tempdb..#Wcal') IS NOT NULL DROP Table #Wcal

		create table #Wcal (Id int,
							Date datetime,
							ClassTypeId int,
							StudioRoomId int,
							Name nvarchar(250),
							Description nvarchar(1024),
							Duration int,
							MaxParticipants int,
							DailySlotId int,
							IsForMale bit,
							IsForFemale bit,
							IsMultiRoom bit,
							MaxExtraParticipants int,
							AgeGroup int,
							UsePlacements bit,
							ClassTypeDetailsId int,
							MinAge int		)

		INSERT INTO #Wcal
				( Id,
					Date ,
					ClassTypeId ,
					StudioRoomId ,
					Name ,
					Description ,
					Duration ,
					MaxParticipants ,
					DailySlotId,
					IsForMale ,
					IsForFemale ,
					IsMultiRoom,
					MaxExtraParticipants,
					AgeGroup,
					UsePlacements,
					ClassTypeDetailsId,
					MinAge		)
		SELECT 
			c.Id,
			DATEADD(DAY, 7, Date),
			ClassTypeId ,
			StudioRoomId ,
			c.Name ,
			Description ,
			Duration ,
			c.MaxParticipants ,
			DailySlotId,
			IsForMale ,
			IsForFemale ,
			IsMultiRoom,
			MaxExtraParticipants,
			AgeGroup,
			UsePlacements,
			ClassTypeDetailsId,
			MinAge
		FROM dbo.Class c
		join StudioRoom sr on sr.Id = c.StudioRoomId
		--join Studio s on s.Id = sr.StudioId
		WHERE Date >= @startDate AND Date < @EndDate AND c.IsDeleted = 0 and sr.StudioId = @StudioId

			--insert classes
		INSERT INTO [dbo].[Class]
           ([Date]
           ,[ClassTypeId]
           ,[StudioRoomId]
           ,[Name]
           ,[Description]
           ,[Duration]
           ,[MaxParticipants]
           ,[Participants]
           ,[IsForMale]
           ,[IsForFemale]
           ,[IsDeleted]
           ,[DailySlotId]
           ,[WaitingList]
           ,[Rating]
           ,[IsMultiRoom]
           ,[ClassTypeDetailsId]
           ,[Published]
           ,[ExtraParticipants]
           ,[MaxExtraParticipants]
           ,[MinAge]
           ,[AgeGroup]
           ,[UsePlacements])
	select
			Date
           ,[ClassTypeId]
           ,[StudioRoomId]
           ,[Name]
           ,[Description]
           ,[Duration]
           ,[MaxParticipants]
           ,0
           ,[IsForMale]
           ,[IsForFemale]
           ,0
           ,[DailySlotId]
           ,0
           ,0
           ,[IsMultiRoom]
           ,[ClassTypeDetailsId]
           ,@AutoPublish
           ,0
           ,[MaxExtraParticipants]
           ,[MinAge]
           ,[AgeGroup]
           ,[UsePlacements]
		   from #Wcal

			--select * from #Wcal
	
			--insert instructors
			insert into [dbo].[Class_Instructors] (classid, instructorid)
			select c1.id, i.[InstructorId]
			from class c1
			inner join #Wcal c2 on c1.DailySlotId = c2.DailySlotId and c1.Date = c2.Date and c1.StudioRoomId = c2.StudioRoomId and c1.IsDeleted = 0
			inner join [dbo].[Class_Instructors] i on i.classid = c2.id

			--insert placements
			insert into ClassAvailablePlacement (ClassId, StudioPlacementId, StudioPlacementName, ClassPlacementNumber, IsDeleted, IsInUse)
			select c1.id, i.StudioPlacementId, i.StudioPlacementName, i.ClassPlacementNumber, i.IsDeleted, 0
						from class c1
			inner join #Wcal c2 on c1.DailySlotId = c2.DailySlotId and c1.Date = c2.Date and c1.StudioRoomId = c2.StudioRoomId and c1.IsDeleted = 0
			inner join [dbo].ClassAvailablePlacement i on i.classid = c2.id

			SET @RESULT = 1
		END
	END

	SELECT @RESULT
END