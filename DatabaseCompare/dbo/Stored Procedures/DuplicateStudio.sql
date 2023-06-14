-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE DuplicateStudio
	-- Add the parameters for the stored procedure here
	@ORGstudioId int = 0,
	@NEWstudioId int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @ORGCompanyId int, @NEWCompanyId int

	set @ORGCompanyId = (select CompanyId from Studio where id = @ORGstudioId)
	set @NEWCompanyId = (select CompanyId from Studio where id = @NEWstudioId)

INSERT INTO [dbo].[ClassDailySlot]
           ([StartTime]
           ,[EndTime]
           ,[Duration]
           ,[Description]
           ,[IsDeleted]
           ,[StudioId])
SELECT [StartTime]
      ,[EndTime]
      ,[Duration]
      ,[Description]
      ,[IsDeleted]
      ,@NEWstudioId
  FROM [dbo].[ClassDailySlot]
  where StudioId = @ORGstudioId


INSERT INTO [dbo].[ClassType]
           ([Name]
           ,[Description]
           ,[Picture]
           ,[BGColor]
           ,[IsDeleted]
           ,[StudioId])
SELECT [Name]
      ,[Description]
      ,[Picture]
      ,[BGColor]
      ,[IsDeleted]
      ,@NEWstudioId
  FROM [dbo].[ClassType]
  where StudioId = @ORGstudioId



INSERT INTO [dbo].[MSGTypes]
           ([Id]
           ,[Name]
           ,[Message]
           ,[Active]
           ,[AutoSend]
           ,[TimeBefore]
           ,CompanyId)
SELECT [Id]+9
      ,[Name]
      ,[Message]
      ,[Active]
      ,[AutoSend]
      ,[TimeBefore]
      ,@NEWCompanyId
  FROM [dbo].[MSGTypes]
  where CompanyId = @ORGCompanyId



INSERT INTO [dbo].[SMSSettings]
           ([CompanyId]
           ,[UserName]
           ,[Password]
           ,[AccountId]
           ,[AccountName]
           ,[DisplyName]
           ,[Balance]
           ,[Active]
           ,[URL])
SELECT @NEWCompanyId
      ,[UserName]
      ,[Password]
      ,[AccountId]
      ,[AccountName]
      ,[DisplyName]
      ,[Balance]
      ,[Active]
      ,[URL]
  FROM [dbo].[SMSSettings]
  where CompanyId = @ORGCompanyId



INSERT INTO [dbo].[StudioPlacements]
           ([Name]
           ,[StudioId]
           ,[IsDeleted])
SELECT [Name]
      ,@NEWstudioId
      ,[IsDeleted]
  FROM [dbo].[StudioPlacements]
  where StudioId = @ORGstudioId


INSERT INTO [dbo].[StudioRoom]
           ([Name]
           ,[MaxParticipants]
           ,[Picture]
           ,[StudioId]
           ,[IsDeleted])
SELECT [Name]
      ,[MaxParticipants]
      ,[Picture]
      ,@NEWstudioId
      ,[IsDeleted]
  FROM [dbo].[StudioRoom]
  where StudioId = @ORGstudioId


INSERT INTO [dbo].[SubscriptionType]
           ([Name]
           ,[Description]
           ,[NumClasses]
           ,[PeriodMonths]
           ,[Price]
           ,[IsDeleted]
           ,[StudioId])
SELECT [Name]
      ,[Description]
      ,[NumClasses]
      ,[PeriodMonths]
      ,[Price]
      ,[IsDeleted]
      ,@NEWstudioId
  FROM [dbo].[SubscriptionType]
  where StudioId = @ORGstudioId


INSERT INTO [dbo].[Tips]
           ([StudioId]
           ,[Short]
           ,[Long]
           ,[IsDeleted])
SELECT @NEWstudioId
      ,[Short]
      ,[Long]
      ,[IsDeleted]
  FROM [dbo].[Tips]
  where StudioId = @ORGstudioId



END