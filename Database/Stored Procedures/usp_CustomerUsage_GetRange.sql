/* =========================================================
   11) Customer Usage Range
   What it does:
   - Returns customer-wise totals for date range
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_CustomerUsage_GetRange
(
    @TenantId      BIGINT,
    @FromUtc       DATETIME2(3),
    @ToUtc         DATETIME2(3),
    @CustomerId    BIGINT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ue.CustomerId,
        c.CustomerName,
        COUNT(1) AS TotalRequests,
        SUM(CASE WHEN ue.IsSuccess = 1 THEN 1 ELSE 0 END) AS SuccessRequests,
        SUM(CASE WHEN ue.IsSuccess = 0 THEN 1 ELSE 0 END) AS ErrorRequests,
        CAST(AVG(CAST(ue.LatencyMs AS FLOAT)) AS INT) AS AvgLatencyMs
    FROM dbo.UsageEvents ue
    INNER JOIN dbo.Customers c
        ON c.CustomerId = ue.CustomerId
       AND c.TenantId = ue.TenantId
    WHERE ue.TenantId = @TenantId
      AND ue.RequestAtUtc >= @FromUtc
      AND ue.RequestAtUtc <  @ToUtc
      AND (@CustomerId IS NULL OR ue.CustomerId = @CustomerId)
    GROUP BY ue.CustomerId, c.CustomerName
    ORDER BY TotalRequests DESC;
END
GO
