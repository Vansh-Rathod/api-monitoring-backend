/* =========================================================
   12) Customer Usage Daily Trend
   What it does:
   - Gives day-wise trend for one customer
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_CustomerUsage_GetDailyTrend
(
    @TenantId      BIGINT,
    @CustomerId    BIGINT,
    @FromDate      DATE,
    @ToDate        DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UsageDate,
        TotalRequests,
        SuccessRequests,
        ErrorRequests,
        AvgLatencyMs
    FROM dbo.UsageDailyAgg
    WHERE TenantId = @TenantId
      AND CustomerId = @CustomerId
      AND UsageDate >= @FromDate
      AND UsageDate <= @ToDate
    ORDER BY UsageDate ASC;
END
GO
