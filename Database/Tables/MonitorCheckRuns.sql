CREATE TABLE dbo.MonitorCheckRuns
(
    CheckRunId            BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId              BIGINT         NOT NULL,
    MonitorId             BIGINT         NOT NULL,
    CheckedAtUtc          DATETIME2(3)   NOT NULL,
    StatusCode            INT            NULL,
    LatencyMs             INT            NULL,
    Result                TINYINT        NOT NULL, -- 1=Up, 2=Down, 3=Timeout, 4=Error
    ErrorMessage          NVARCHAR(1000) NULL,

    CONSTRAINT FK_MonitorCheckRuns_Tenants  FOREIGN KEY (TenantId)  REFERENCES dbo.Tenants(TenantId),
    CONSTRAINT FK_MonitorCheckRuns_Monitors FOREIGN KEY (MonitorId) REFERENCES dbo.Monitors(MonitorId),
    CONSTRAINT CK_MonitorCheckRuns_Result CHECK (Result IN (1,2,3,4))
);
GO

CREATE INDEX IX_MonitorCheckRuns_Tenant_Monitor_CheckedAtUtc
ON dbo.MonitorCheckRuns(TenantId, MonitorId, CheckedAtUtc DESC);
GO