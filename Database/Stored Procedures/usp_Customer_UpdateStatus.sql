/* =========================================================
   4) Customer Update Status
   What it does:
   - Activates/Deactivates/Blocks customer
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Customer_UpdateStatus
(
    @TenantId      BIGINT,
    @CustomerId    BIGINT,
    @Status        TINYINT  -- 1=Active,2=Inactive,3=Blocked
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customers
    SET Status = @Status,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE TenantId = @TenantId
      AND CustomerId = @CustomerId;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO
