SET QUOTED_IDENTIFIER ON;
GO

-- ============================================
-- INQUIRIES TABLE
-- Submissions from the public "Inquiry" page (About/Contact-adjacent lead
-- form) - kept as its own lightweight table rather than reusing ChatHistory
-- or Reviews, since it has no relationship to a product or an authenticated
-- user (anonymous visitors submit these).
-- ============================================
CREATE TABLE [dbo].[Inquiries]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Inquiries PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Phone] NVARCHAR(20) NULL,
    [Subject] NVARCHAR(255) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [IsResolved] BIT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT DEFAULT 0
);

CREATE NONCLUSTERED INDEX IX_Inquiries_CreatedAt ON [dbo].[Inquiries] ([CreatedAt] DESC) WHERE [IsDeleted] = 0;
GO
