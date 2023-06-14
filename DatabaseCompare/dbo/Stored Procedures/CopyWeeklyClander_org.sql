-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CopyWeeklyClander_org]
	-- Add the parameters for the stored procedure here
	@startDate DATETIME = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @RESULT bit = 0
	DECLARE @EndDate DATE

    IF @startDate IS NOT NULL
	BEGIN
		SET @EndDate = DATEADD(DAY, 7, @startDate)

				IF NOT EXISTS(select 1 from dbo.Class WHERE Date >= @startDate AND Date < @EndDate) 
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
					  IsFull ,
					  IsDeleted
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
				0 ,
				IsDeleted
			FROM dbo.Class
			WHERE Date >= DATEADD(DAY, -7, @startDate) AND Date < DATEADD(DAY, -7, @EndDate) AND IsDeleted = 0

			SET @RESULT = 1
		END
		ELSE IF NOT EXISTS(select 1 from dbo.Class WHERE Date >= DATEADD(DAY, 7, @startDate) AND Date < DATEADD(DAY, 7, @EndDate)) 
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
					  IsFull ,
					  IsDeleted
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
				0 ,
				IsDeleted
			FROM dbo.Class
			WHERE Date >= @startDate AND Date < @EndDate AND IsDeleted = 0

			SET @RESULT = 1
		END
	END

	SELECT @RESULT
END
