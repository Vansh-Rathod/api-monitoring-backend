CREATE TABLE dbo.Users
(
    UserId              BIGINT IDENTITY(1,1) PRIMARY KEY,
    Email               NVARCHAR(320) NOT NULL,
    PasswordHash        NVARCHAR(500) NULL, -- NULL if external auth
    FirstName           NVARCHAR(100) NULL,
    LastName            NVARCHAR(100) NULL,
    IsActive            BIT           NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAtUtc        DATETIME2(3)  NOT NULL CONSTRAINT DF_Users_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc        DATETIME2(3)  NOT NULL CONSTRAINT DF_Users_UpdatedAtUtc DEFAULT SYSUTCDATETIME(),
    RowVersion          ROWVERSION,
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO