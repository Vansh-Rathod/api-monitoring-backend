CREATE OR ALTER PROCEDURE dbo.usp_Monitor_ListActive
(
    @TenantId BIGINT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MonitorId,
        TenantId,
        CustomerId,
        MonitorName,
        BaseUrl,
        [Path],
        HttpMethod,
        CheckIntervalSeconds,
        TimeoutMs,
        ExpectedStatusCode
    FROM dbo.Monitors
    WHERE IsActive = 1
      AND (@TenantId IS NULL OR TenantId = @TenantId);
END
GO
