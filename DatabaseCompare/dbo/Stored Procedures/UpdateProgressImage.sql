-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateProgressImage] 
	-- Add the parameters for the stored procedure here
	@UserId NVARCHAR(128),
	@ProgressIMG NVARCHAR(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO dbo.ProfileTracking
	        ( UserID ,
	          Date ,
	          Height ,
	          Weight ,
	          BMI ,
	          Fat ,
	          Dim_Hands ,
	          Dim_Legs ,
	          Dim_Thighs ,
	          Dim_Waist ,
	          Picture
	        )
	SELECT TOP 1 @UserId , -- UserID - nvarchar(128)
	          GETDATE() , -- Date - datetime
	          Height,
	          Weight,
			  BMI,
			  Fat,
			  Dim_Hands,
			  Dim_Legs,
			  Dim_Thighs,
			  Dim_Waist,
			  @ProgressIMG
			  FROM dbo.ProfileTracking
			  WHERE UserID = @UserId
			  ORDER BY Date DESC
			  	        

END
