-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[PublishWeeklyClander]
	-- Add the parameters for the stored procedure here
	@startDate DATETIME = NULL
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
			IF NOT EXISTS(select 1 from dbo.Class WHERE Date >= @startDate AND Date < @EndDate) 
				BEGIN
					set @EndDate = @startDate
					set @startDate = DATEADD(DAY, -7, @startDate)
				End
			--next week 
			ELSE IF NOT EXISTS(select 1 from dbo.Class WHERE Date >= DATEADD(DAY, 7, @startDate) AND Date < DATEADD(DAY, 7, @EndDate))
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

			update dbo.Class
			set Published = 1
			WHERE Date >= @startDate AND Date < @EndDate AND IsDeleted = 0 and Published = 0

			SET @RESULT = 1
		END
	END

	SELECT @RESULT
END
