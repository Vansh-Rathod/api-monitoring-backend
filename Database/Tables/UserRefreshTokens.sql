CREATE TABLE dbo.UserRefreshTokens
(
    RefreshTokenId       BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId               BIGINT         NOT NULL,
    TenantId             BIGINT         NOT NULL,
    TokenHash            NVARCHAR(128)  NOT NULL,
    JwtId                NVARCHAR(100)  NULL,
    ExpiresAtUtc         DATETIME2(3)   NOT NULL,
    CreatedAtUtc         DATETIME2(3)   NOT NULL CONSTRAINT DF_UserRefreshTokens_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    CreatedByIp          NVARCHAR(64)   NULL,
    RevokedAtUtc         DATETIME2(3)   NULL,
    RevokedByIp          NVARCHAR(64)   NULL,
    ReplacedByTokenHash  NVARCHAR(128)  NULL,
    RowVersion           ROWVERSION,

    CONSTRAINT FK_UserRefreshTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_UserRefreshTokens_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(TenantId),
    CONSTRAINT UQ_UserRefreshTokens_TokenHash UNIQUE (TokenHash)
);
GO

CREATE INDEX IX_UserRefreshTokens_User_Tenant
ON dbo.UserRefreshTokens(UserId, TenantId, ExpiresAtUtc DESC);
GO
