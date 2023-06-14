CREATE TABLE [dbo].[ProfileTracking] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [UserID]       NVARCHAR (128) NOT NULL,
    [Date]         DATETIME       NOT NULL,
    [Picture]      NVARCHAR (256) NULL,
    [Height]       INT            NULL,
    [Weight]       DECIMAL (5, 2) NULL,
    [BMI]          DECIMAL (5, 2) NULL,
    [Fat]          DECIMAL (5, 2) NULL,
    [Dim_Hands]    DECIMAL (5, 2) NULL,
    [Dim_Legs]     DECIMAL (5, 2) NULL,
    [Dim_Thighs]   DECIMAL (5, 2) NULL,
    [Dim_Waist]    DECIMAL (5, 2) NULL,
    [WeightChange] DECIMAL (5, 2) CONSTRAINT [DF__tmp_rg_xx__Weigh__5AEE82B9] DEFAULT ((0)) NOT NULL,
    [Fat_HandL]    DECIMAL (5, 2) NULL,
    [Fat_HandR]    DECIMAL (5, 2) NULL,
    [Fat_LegR]     DECIMAL (5, 2) NULL,
    [Fat_LegL]     DECIMAL (5, 2) NULL,
    [Fat_Belly]    DECIMAL (5, 2) NULL,
    [Mucsle]       DECIMAL (5, 2) NULL,
    CONSTRAINT [PK_ProfileTracking] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProfileTracking_AspNetUsers] FOREIGN KEY ([UserID]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE TRIGGER [dbo].[trig_Update_WeightChange]
ON [dbo].[ProfileTracking]
FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @lastchange INT 
	DECLARE @weight INT
	DECLARE @oldweight INT
	DECLARE @change INT
	DECLARE @date DATETIME
    DECLARE @id INT
	--DECLARE @Pic NVARCHAR(256)

	SELECT @date = Inserted.Date, @weight = Inserted.Weight , @id = Inserted.Id
	FROM Inserted

	SELECT TOP 1 @lastchange = WeightChange, @oldweight = [Weight]
	FROM dbo.ProfileTracking
	WHERE [Date] < @date
	ORDER BY Date DESC

	IF (@lastchange IS NULL) SET @change = 0
	ELSE SET @change = ((@weight - @oldweight) * -1) + @lastchange

	UPDATE dbo.ProfileTracking
	SET WeightChange = ISNULL(@change,0)
	WHERE Id = @Id
END

GO
DISABLE TRIGGER [dbo].[trig_Update_WeightChange]
    ON [dbo].[ProfileTracking];

