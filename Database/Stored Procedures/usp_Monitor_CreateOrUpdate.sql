/* =========================================================
   6) Monitor Create/Update (Upsert style)
   What it does:
   - Creates monitor if MonitorId is null
   - Updates monitor if MonitorId provided
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Monitor_CreateOrUpdate
(
    @TenantId               BIGINT,
    @MonitorId              BIGINT = NULL,
    @CustomerId             BIGINT = NULL,
    @MonitorName            NVARCHAR(200),
    @BaseUrl                NVARCHAR(500),
    @Path                   NVARCHAR(500),
    @HttpMethod             NVARCHAR(10),
    @CheckIntervalSeconds   INT,
    @TimeoutMs              INT,
    @ExpectedStatusCode     INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @MonitorId IS NULL
    BEGIN
        INSERT INTO dbo.Monitors
        (
            TenantId, CustomerId, MonitorName, BaseUrl, [Path], HttpMethod,
            CheckIntervalSeconds, TimeoutMs, ExpectedStatusCode, IsActive
        )
        VALUES
        (
            @TenantId, @CustomerId, @MonitorName, @BaseUrl, @Path, @HttpMethod,
            @CheckIntervalSeconds, @TimeoutMs, @ExpectedStatusCode, 1
        );

        SELECT SCOPE_IDENTITY() AS MonitorId;
    END
    ELSE
    BEGIN
        UPDATE dbo.Monitors
        SET CustomerId = @CustomerId,
            MonitorName = @MonitorName,
            BaseUrl = @BaseUrl,
            [Path] = @Path,
            HttpMethod = @HttpMethod,
            CheckIntervalSeconds = @CheckIntervalSeconds,
            TimeoutMs = @TimeoutMs,
            ExpectedStatusCode = @ExpectedStatusCode,
            UpdatedAtUtc = SYSUTCDATETIME()
        WHERE TenantId = @TenantId
          AND MonitorId = @MonitorId;

        SELECT @MonitorId AS MonitorId, @@ROWCOUNT AS AffectedRows;
    END
END
GO
