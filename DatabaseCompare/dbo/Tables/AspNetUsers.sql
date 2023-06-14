CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [PhoneNumber]          NVARCHAR (256) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [UserName]             NVARCHAR (256) NOT NULL,
    [FirstName]            NVARCHAR (128) NULL,
    [LastName]             NVARCHAR (128) NULL,
    [DOB]                  DATETIME       NULL,
    [Address]              NVARCHAR (512) NULL,
    [ProfileUpdateDate]    DATETIME       NULL,
    [JoinDate]             DATETIME       NULL,
    [ProfileIMG]           NVARCHAR (250) NULL,
    [Gender]               INT            CONSTRAINT [DF__tmp_ms_xx__Gende__1940BAED] DEFAULT ((1)) NOT NULL,
    [Active]               BIT            CONSTRAINT [DF__tmp_ms_xx__Activ__1A34DF26] DEFAULT ((0)) NOT NULL,
    [ReceiveSMS]           BIT            CONSTRAINT [DF__tmp_ms_xx__Recei__1B29035F] DEFAULT ((1)) NOT NULL,
    [LastClass]            DATETIME       NULL,
    [FullName]             AS             ((ltrim(rtrim([FirstName]))+' ')+ltrim(rtrim([LastName]))),
    [AcceptedTandC]        BIT            CONSTRAINT [DF_AspNetUsers_AcceptedTandC] DEFAULT ((0)) NOT NULL,
    [CitizenId]            BIGINT         NULL,
    [SignedHealthTandC]    BIT            CONSTRAINT [DF_AspNetUsers_SignedHealthTandC] DEFAULT ((0)) NOT NULL,
    [SignedDate]           DATETIME       NULL,
    [Occupation]           NVARCHAR (256) NULL,
    [StudioId]             INT            NOT NULL,
    [AgeGroup]             INT            CONSTRAINT [DF_AspNetUsers_Gender1] DEFAULT ((30)) NOT NULL,
    [Marked]               BIT            CONSTRAINT [DF_AspNetUsers_ReceiveSMS1] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUsers_Studio] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studio] ([Id])
);










GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[AspNetUsers]([UserName] ASC);

