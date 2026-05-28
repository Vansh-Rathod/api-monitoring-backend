/* =========================================================
   0) TVP for bulk usage ingestion
   ========================================================= */
CREATE TYPE dbo.UsageEventTvp AS TABLE
(
    CustomerId      BIGINT         NOT NULL,
    MonitorId       BIGINT         NULL,
    RequestAtUtc    DATETIME2(3)   NOT NULL,
    StatusCode      INT            NOT NULL,
    LatencyMs       INT            NOT NULL,
    IsSuccess       BIT            NOT NULL,
    RequestUnits    INT            NOT NULL,
    TraceId         NVARCHAR(100)  NULL
);
GO