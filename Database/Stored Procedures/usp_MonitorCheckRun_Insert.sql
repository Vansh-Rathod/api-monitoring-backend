/* =========================================================
   9) Monitor Check Run Insert
   What it does:
   - Stores synthetic monitor check result (up/down/timeout/error)
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_MonitorCheckRun_Insert
(
    @TenantId      BIGINT,
    @MonitorId     BIGINT,
    @CheckedAtUtc  DATETIME2(3),
    @StatusCode    INT = NULL,
    @LatencyMs     INT = NULL,
    @Result        TINYINT,          -- 1=Up,2=Down,3=Timeout,4=Error
    @ErrorMessage  NVARCHAR(1000) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.MonitorCheckRuns
    (
        TenantId, MonitorId, CheckedAtUtc, StatusCode, LatencyMs, Result, ErrorMessage
    )
    VALUES
    (
        @TenantId, @MonitorId, @CheckedAtUtc, @StatusCode, @LatencyMs, @Result, @ErrorMessage
    );

    SELECT SCOPE_IDENTITY() AS CheckRunId;
END
GO
