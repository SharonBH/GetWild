CREATE TABLE [dbo].[Class] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [Date]                 DATETIME        NOT NULL,
    [ClassTypeId]          INT             NOT NULL,
    [StudioRoomId]         INT             NOT NULL,
    [Name]                 NVARCHAR (250)  NOT NULL,
    [Description]          NVARCHAR (1024) NULL,
    [Duration]             INT             NOT NULL,
    [MaxParticipants]      INT             CONSTRAINT [DF__tmp_rg_xx__MaxPa__0D7A0286] DEFAULT ((0)) NOT NULL,
    [Participants]         INT             CONSTRAINT [DF_Class_Participants] DEFAULT ((0)) NOT NULL,
    [IsForMale]            BIT             CONSTRAINT [DF__tmp_rg_xx__IsFor__0E6E26BF] DEFAULT ((0)) NOT NULL,
    [IsForFemale]          BIT             CONSTRAINT [DF__tmp_rg_xx__IsFor__0F624AF8] DEFAULT ((0)) NOT NULL,
    [IsFull]               AS              (CONVERT([bit],case when (([MaxParticipants]-[Participants])-[WaitingList])>(0) then (0) else (1) end,(0))),
    [IsDeleted]            BIT             CONSTRAINT [DF__tmp_rg_xx__IsDel__114A936A] DEFAULT ((0)) NOT NULL,
    [DailySlotId]          INT             CONSTRAINT [DF_Class_DailySlotId] DEFAULT ((-1)) NOT NULL,
    [WaitingList]          INT             CONSTRAINT [DF_Class_WaitingList] DEFAULT ((0)) NOT NULL,
    [ShortURL]             NVARCHAR (128)  NULL,
    [Rating]               FLOAT (53)      CONSTRAINT [DF_Class_Rating] DEFAULT ((0)) NOT NULL,
    [IsMultiRoom]          BIT             CONSTRAINT [DF_Class_IsMultiRoom] DEFAULT ((0)) NOT NULL,
    [ClassTypeDetailsId]   INT             NULL,
    [Published]            BIT             CONSTRAINT [DF_Class_Published] DEFAULT ((1)) NOT NULL,
    [ExtraParticipants]    INT             CONSTRAINT [DF_Class_ExtraParticipants] DEFAULT ((0)) NOT NULL,
    [MaxExtraParticipants] INT             CONSTRAINT [DF_Class_MaxParticipants1] DEFAULT ((0)) NOT NULL,
    [MinAge]               INT             CONSTRAINT [DF_Class_MinAge] DEFAULT ((18)) NOT NULL,
    [AgeGroup]             INT             CONSTRAINT [DF_Class_AgeGroup] DEFAULT ((0)) NOT NULL,
    [UsePlacements]        BIT             CONSTRAINT [DF_Class_UsePlacements] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Class_ClassDailySlot] FOREIGN KEY ([DailySlotId]) REFERENCES [dbo].[ClassDailySlot] ([Id]),
    CONSTRAINT [FK_Class_ClassType] FOREIGN KEY ([ClassTypeId]) REFERENCES [dbo].[ClassType] ([Id]),
    CONSTRAINT [FK_Class_ClassTypeDetails] FOREIGN KEY ([ClassTypeDetailsId]) REFERENCES [dbo].[ClassTypeDetails] ([Id]),
    CONSTRAINT [FK_Class_StudioRoom] FOREIGN KEY ([StudioRoomId]) REFERENCES [dbo].[StudioRoom] ([Id])
);










GO
CREATE NONCLUSTERED INDEX [IX_UserClass]
    ON [dbo].[Class]([Date] ASC)
    INCLUDE([Id], [ClassTypeId]);

