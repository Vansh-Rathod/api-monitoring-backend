/* =========================================================
   7) Monitor Set Active
   What it does:
   - Enables/disables monitor
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Monitor_SetActive
(
    @TenantId    BIGINT,
    @MonitorId   BIGINT,
    @IsActive    BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Monitors
    SET IsActive = @IsActive,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE TenantId = @TenantId
      AND MonitorId = @MonitorId;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO
