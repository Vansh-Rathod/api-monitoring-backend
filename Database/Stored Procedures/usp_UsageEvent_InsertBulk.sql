/************************************************************
   8) UsageEvent Bulk Insert (TVP)
   What it does:
   - Inserts many usage events in one call (best for Dapper bulk ingest)
 ************************************************************/
CREATE OR ALTER PROCEDURE dbo.usp_UsageEvent_InsertBulk
(
    @TenantId   BIGINT,
    @Events     dbo.UsageEventTvp READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.UsageEvents
    (
        TenantId, CustomerId, MonitorId, RequestAtUtc,
        StatusCode, LatencyMs, IsSuccess, RequestUnits, TraceId
    )
    SELECT
        @TenantId, e.CustomerId, e.MonitorId, e.RequestAtUtc,
        e.StatusCode, e.LatencyMs, e.IsSuccess, e.RequestUnits, e.TraceId
    FROM @Events e
    WHERE EXISTS
    (
        SELECT 1 FROM dbo.Customers c
        WHERE c.TenantId = @TenantId
          AND c.CustomerId = e.CustomerId
    );

    SELECT @@ROWCOUNT AS InsertedRows;
END
GO
