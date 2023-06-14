-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[PublishWeeklyClanderForCompany]
	-- Add the parameters for the stored procedure here
	@startDate DATETIME = NULL,
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
							WHERE Date >= @startDate AND Date < @EndDate and sr.StudioId = @StudioId) 
				BEGIN
					set @EndDate = @startDate
					set @startDate = DATEADD(DAY, -7, @startDate)
				End
			--next week 
			ELSE IF NOT EXISTS(select 1 from dbo.Class c
							join StudioRoom sr on sr.Id = c.StudioRoomId
							--join Studio s on s.Id = sr.StudioId 
							WHERE Date >= DATEADD(DAY, 7, @startDate) AND Date < DATEADD(DAY, 7, @EndDate) and sr.StudioId = @StudioId) 
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

			update trg
			set Published = 1
			from Class trg
			join StudioRoom sr on sr.Id = trg.StudioRoomId
			WHERE Date >= @startDate AND Date < @EndDate AND trg.IsDeleted = 0 and Published = 0
			and sr.StudioId = @StudioId

			SET @RESULT = 1
		END
	END

	SELECT @RESULT
END