CREATE TABLE [dbo].[Tips] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [StudioId]  INT             NOT NULL,
    [Short]     NVARCHAR (512)  NULL,
    [Long]      NVARCHAR (2048) NULL,
    [IsDeleted] BIT             CONSTRAINT [DF__Tips__IsDeleted__52593CB8] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Tips] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tips_Company] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studio] ([Id])
);

