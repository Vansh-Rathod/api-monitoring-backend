/* =========================================================
   13) Monitor Health Latest
   What it does:
   - Latest check status per monitor
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_MonitorHealth_GetLatest
(
    @TenantId   BIGINT,
    @CustomerId BIGINT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH LatestRun AS
    (
        SELECT
            mcr.MonitorId,
            mcr.CheckedAtUtc,
            mcr.StatusCode,
            mcr.LatencyMs,
            mcr.Result,
            mcr.ErrorMessage,
            ROW_NUMBER() OVER (PARTITION BY mcr.MonitorId ORDER BY mcr.CheckedAtUtc DESC) AS rn
        FROM dbo.MonitorCheckRuns mcr
        WHERE mcr.TenantId = @TenantId
    )
    SELECT
        m.MonitorId,
        m.MonitorName,
        m.CustomerId,
        lr.CheckedAtUtc,
        lr.StatusCode,
        lr.LatencyMs,
        lr.Result,
        lr.ErrorMessage
    FROM dbo.Monitors m
    LEFT JOIN LatestRun lr
        ON lr.MonitorId = m.MonitorId
       AND lr.rn = 1
    WHERE m.TenantId = @TenantId
      AND (@CustomerId IS NULL OR m.CustomerId = @CustomerId)
    ORDER BY m.MonitorId DESC;
END
GO
