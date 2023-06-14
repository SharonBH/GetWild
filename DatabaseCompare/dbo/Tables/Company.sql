CREATE TABLE [dbo].[Company] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [Name]                      NVARCHAR (256) NOT NULL,
    [Address]                   NVARCHAR (512) NULL,
    [ManagerName]               NVARCHAR (256) NULL,
    [PhoneNumber]               NVARCHAR (64)  NULL,
    [Email]                     NVARCHAR (256) NOT NULL,
    [StartDate]                 DATETIME       CONSTRAINT [DF_Company_StartDate] DEFAULT (getdate()) NOT NULL,
    [UseSMS]                    BIT            CONSTRAINT [DF_Company_UseSMS] DEFAULT ((0)) NOT NULL,
    [UseHosting]                BIT            CONSTRAINT [DF_Company_UseHosting] DEFAULT ((1)) NOT NULL,
    [Active]                    BIT            CONSTRAINT [DF_Company_Active] DEFAULT ((1)) NOT NULL,
    [IsDeleted]                 BIT            CONSTRAINT [DF_Company_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CompanyCode]               NVARCHAR (512) NOT NULL,
    [WebSiteURL]                NVARCHAR (256) NOT NULL,
    [SiteName]                  NVARCHAR (256) NULL,
    [LogoURL]                   NVARCHAR (128) NULL,
    [CSSFileName]               NVARCHAR (128) NULL,
    [CancellationThresholdMins] INT            CONSTRAINT [DF_Company_CancellationThresholdMins] DEFAULT ((120)) NOT NULL,
    [SpacesLeftToShow]          INT            NULL,
    [HealthTandCEnabled]        BIT            CONSTRAINT [DF_Company_HealthTandCEnabled] DEFAULT ((0)) NOT NULL,
    [ClassRatingEnabled]        BIT            CONSTRAINT [DF_Company_ClassRatingEnabled] DEFAULT ((0)) NOT NULL,
    [GoogleAPIKey]              NVARCHAR (128) NULL,
    [HostingRenewDate]          DATETIME       NULL,
    [StudioRoomName]            NVARCHAR (256) CONSTRAINT [DF_Company_StudioRoomName] DEFAULT (N'סטודיו') NOT NULL,
    [UseClassNamefromType]      BIT            CONSTRAINT [DF_Company_UseClassNamefromType] DEFAULT ((1)) NOT NULL,
    [WaitingListEnabled]        BIT            CONSTRAINT [DF_Company_WaitingListEnabled] DEFAULT ((0)) NOT NULL,
    [WaitingListSpaces]         INT            CONSTRAINT [DF_Company_WaitingListSpaces] DEFAULT ((0)) NOT NULL,
    [TipsName]                  NVARCHAR (256) CONSTRAINT [DF_Company_TipsName] DEFAULT (N'טיפים') NOT NULL,
    [CalanderMode]              NVARCHAR (100) CONSTRAINT [DF_Company_CalanderMode] DEFAULT (N'daily') NOT NULL,
    [UseInstructors]            BIT            CONSTRAINT [DF_Company_UseInstructors] DEFAULT ((0)) NOT NULL,
    [UseExpenses]               BIT            CONSTRAINT [DF_Company_UseExpenses] DEFAULT ((0)) NOT NULL,
    [UseClassTypeDetails]       BIT            CONSTRAINT [DF_Company_UseExpenses1] DEFAULT ((0)) NOT NULL,
    [NumAdvncedEnrollments]     INT            CONSTRAINT [DF_Company_NumAdvncedEnrollments] DEFAULT ((2)) NOT NULL,
    [ManageAfterRegister]       BIT            CONSTRAINT [DF_Company_ManageAfterRegister] DEFAULT ((0)) NOT NULL,
    [AutoPublish]               BIT            CONSTRAINT [DF_Company_AutoPublish] DEFAULT ((0)) NOT NULL,
    [LateRegistration]          INT            CONSTRAINT [DF_Company_LateRegistration] DEFAULT ((0)) NOT NULL,
    [UsePlacements]             BIT            CONSTRAINT [DF_Company_UsePlacements] DEFAULT ((0)) NOT NULL,
    [UseAgeforGender]           BIT            CONSTRAINT [DF__Company__UseAgef__1D66518C] DEFAULT ((0)) NOT NULL,
    [PriorityWaitListDays]      INT            CONSTRAINT [DF_Company_PriorityWaitListDays] DEFAULT ((0)) NOT NULL,
    [AdultAge]                  INT            CONSTRAINT [DF_Company_AdultAge] DEFAULT ((0)) NOT NULL,
    [TeenAge]                   INT            CONSTRAINT [DF_Company_TeenAge] DEFAULT ((0)) NOT NULL,
    [LateCancelation]           INT            CONSTRAINT [DF__Company__LateCan__605D434C] DEFAULT ((-1)) NOT NULL,
    [LateCancelPenalty]         NVARCHAR (50)  CONSTRAINT [DF__Company__LateCan__1E5A75C5] DEFAULT ((0)) NOT NULL,
    [RemoveMarked]              BIT            CONSTRAINT [DF_Company_RemoveMarked] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([Id] ASC)
);















