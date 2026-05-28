CREATE TABLE dbo.UsageEvents
(
    UsageEventId          BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId              BIGINT         NOT NULL,
    CustomerId            BIGINT         NOT NULL,
    MonitorId             BIGINT         NULL,
    RequestAtUtc          DATETIME2(3)   NOT NULL,
    StatusCode            INT            NOT NULL,
    LatencyMs             INT            NOT NULL,
    IsSuccess             BIT            NOT NULL,
    RequestUnits          INT            NOT NULL CONSTRAINT DF_UsageEvents_RequestUnits DEFAULT (1),
    TraceId               NVARCHAR(100)  NULL,

    CONSTRAINT FK_UsageEvents_Tenants   FOREIGN KEY (TenantId)   REFERENCES dbo.Tenants(TenantId),
    CONSTRAINT FK_UsageEvents_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT FK_UsageEvents_Monitors  FOREIGN KEY (MonitorId)  REFERENCES dbo.Monitors(MonitorId),
    CONSTRAINT CK_UsageEvents_StatusCode CHECK (StatusCode BETWEEN 100 AND 599),
    CONSTRAINT CK_UsageEvents_Latency CHECK (LatencyMs >= 0),
    CONSTRAINT CK_UsageEvents_RequestUnits CHECK (RequestUnits > 0)
);
GO

CREATE INDEX IX_UsageEvents_Tenant_Customer_RequestAtUtc
ON dbo.UsageEvents(TenantId, CustomerId, RequestAtUtc DESC);

CREATE INDEX IX_UsageEvents_Tenant_Monitor_RequestAtUtc
ON dbo.UsageEvents(TenantId, MonitorId, RequestAtUtc DESC);
GO