-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ConfirmSalary]
	-- Add the parameters for the stored procedure here
	@UserId NVARCHAR(128),
	@Adjustment float = 0,
	@Date datetime,
	@Note nvarchar(max),
	@UserConfirmed NVARCHAR(128)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @result bit = 0
		DECLARE @fromDate DATE = (SELECT DATEADD(month, DATEDIFF(month,0,@Date), 0))
	declare @EndofMonth date = (SELECT DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, @fromDate) + 1, 0)))
    -- Insert statements for procedure here
	IF NOT EXISTS(select 1 from [dbo].[InstructorSalary] WHERE InstructorId = @userId and [DateSalary] = @Date) 
		BEGIN
			declare @rate float = 0
			declare @dailyrate float = 0
			declare @classes int = 0
			declare @Salary float = 0

			select @classes = count(distinct ci.ClassId),@rate = i.Rate, @dailyrate = i.DailyRate
			from [dbo].[InstructorDetails] i
			inner join Class_Instructors ci on ci.InstructorId = i.InstructorId
			inner join class c on ci.ClassId = c.Id
			where i.InstructorId = @UserId
			and c.isdeleted = 0 
			and date between @fromDate and @EndofMonth
			group by i.Rate, i.DailyRate

			declare @daily float = 0
			select @daily = count(distinct cast(c.Date as date))
			from Class_Instructors ci 
			inner join class c on ci.ClassId = c.Id
			where ci.InstructorId = @UserId
			and c.isdeleted = 0 
			and c.date between @fromDate and @EndofMonth
			--group by cast(c.Date as date)

			set @Salary = (@rate * @classes) --+ (@daily * @dailyrate) + @Adjustment

			insert into  [dbo].[InstructorSalary] (InstructorId, RateSalary, RateDaily, Classes, AmountSalary, AmountDaily, Adjustment , DateSalary, DateConfirmed, UserConfirmed, Confirmed, Note)
			values (@UserId, @rate, @dailyrate, @classes, @Salary, @daily * @dailyrate, @Adjustment, @fromDate, GETUTCDATE(), @UserConfirmed, 1, @Note)

			set @result = 1
		END
	select @result
END
