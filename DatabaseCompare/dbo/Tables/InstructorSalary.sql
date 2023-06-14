CREATE TABLE [dbo].[InstructorSalary] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [InstructorId]  NVARCHAR (128) NOT NULL,
    [RateSalary]    FLOAT (53)     NOT NULL,
	[RateDaily]		FLOAT (53)     DEFAULT ((0)) NOT NULL,
    [Classes]       INT            NOT NULL,
    [AmountSalary]  FLOAT (53)     NOT NULL,
    [AmountDaily]   FLOAT (53)     DEFAULT ((0)) NOT NULL,
    [Adjustment]    FLOAT (53)     DEFAULT ((0)) NOT NULL,
    [Total]         AS             (([AmountSalary]+[AmountDaily])+[Adjustment]),
    [DateSalary]    DATETIME       NOT NULL,
    [Confirmed]     BIT            NOT NULL,
    [DateConfirmed] DATETIME       NOT NULL,
    [UserConfirmed] NVARCHAR (128) NOT NULL,
    [Note]          NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_InstructorSalary] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InstructorSalary_AspNetUsers] FOREIGN KEY ([InstructorId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_InstructorSalary_AspNetUsers1] FOREIGN KEY ([UserConfirmed]) REFERENCES [dbo].[AspNetUsers] ([Id])
);



