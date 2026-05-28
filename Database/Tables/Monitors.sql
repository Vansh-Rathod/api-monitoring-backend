CREATE TABLE dbo.Monitors
(
    MonitorId             BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId              BIGINT         NOT NULL,
    CustomerId            BIGINT         NULL, -- NULL = tenant-wide monitor
    MonitorName           NVARCHAR(200)  NOT NULL,
    BaseUrl               NVARCHAR(500)  NOT NULL,
    [Path]                NVARCHAR(500)  NOT NULL,
    HttpMethod            NVARCHAR(10)   NOT NULL, -- GET/POST/PUT/DELETE...
    CheckIntervalSeconds  INT            NOT NULL,
    TimeoutMs             INT            NOT NULL,
    ExpectedStatusCode    INT            NOT NULL,
    IsActive              BIT            NOT NULL CONSTRAINT DF_Monitors_IsActive DEFAULT (1),
    CreatedAtUtc          DATETIME2(3)   NOT NULL CONSTRAINT DF_Monitors_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc          DATETIME2(3)   NOT NULL CONSTRAINT DF_Monitors_UpdatedAtUtc DEFAULT SYSUTCDATETIME(),
    RowVersion            ROWVERSION,

    CONSTRAINT FK_Monitors_Tenants   FOREIGN KEY (TenantId)   REFERENCES dbo.Tenants(TenantId),
    CONSTRAINT FK_Monitors_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT CK_Monitors_CheckInterval CHECK (CheckIntervalSeconds > 0),
    CONSTRAINT CK_Monitors_TimeoutMs CHECK (TimeoutMs > 0),
    CONSTRAINT CK_Monitors_ExpectedStatusCode CHECK (ExpectedStatusCode BETWEEN 100 AND 599)
);
GO

CREATE INDEX IX_Monitors_TenantId ON dbo.Monitors(TenantId);
CREATE INDEX IX_Monitors_Tenant_Customer ON dbo.Monitors(TenantId, CustomerId);
GO