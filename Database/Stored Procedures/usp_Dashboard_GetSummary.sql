/* =========================================================
   10) Dashboard Summary
   What it does:
   - Returns tenant-level request stats for date range
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Dashboard_GetSummary
(
    @TenantId      BIGINT,
    @FromUtc       DATETIME2(3),
    @ToUtc         DATETIME2(3)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(1) AS TotalRequests,
        SUM(CASE WHEN IsSuccess = 1 THEN 1 ELSE 0 END) AS SuccessRequests,
        SUM(CASE WHEN IsSuccess = 0 THEN 1 ELSE 0 END) AS ErrorRequests,
        CAST(AVG(CAST(LatencyMs AS FLOAT)) AS INT) AS AvgLatencyMs
    FROM dbo.UsageEvents
    WHERE TenantId = @TenantId
      AND RequestAtUtc >= @FromUtc
      AND RequestAtUtc <  @ToUtc;
END
GO
