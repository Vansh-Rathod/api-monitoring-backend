CREATE TABLE dbo.UsageDailyAgg
(
    UsageDailyAggId       BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId              BIGINT        NOT NULL,
    CustomerId            BIGINT        NOT NULL,
    MonitorId             BIGINT        NULL,
    UsageDate             DATE          NOT NULL,
    TotalRequests         BIGINT        NOT NULL,
    SuccessRequests       BIGINT        NOT NULL,
    ErrorRequests         BIGINT        NOT NULL,
    AvgLatencyMs          INT           NOT NULL,

    CONSTRAINT FK_UsageDailyAgg_Tenants   FOREIGN KEY (TenantId)   REFERENCES dbo.Tenants(TenantId),
    CONSTRAINT FK_UsageDailyAgg_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT FK_UsageDailyAgg_Monitors  FOREIGN KEY (MonitorId)  REFERENCES dbo.Monitors(MonitorId),
    CONSTRAINT CK_UsageDailyAgg_Counts CHECK (
        TotalRequests >= 0 AND SuccessRequests >= 0 AND ErrorRequests >= 0
        AND SuccessRequests + ErrorRequests <= TotalRequests
    )
);
GO

CREATE UNIQUE INDEX UX_UsageDailyAgg_Tenant_Customer_Monitor_Date
ON dbo.UsageDailyAgg(TenantId, CustomerId, MonitorId, UsageDate);
GO