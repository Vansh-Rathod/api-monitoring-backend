/* =========================================================
   3) Customer Update
   What it does:
   - Updates editable customer profile fields
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Customer_Update
(
    @TenantId             BIGINT,
    @CustomerId           BIGINT,
    @ExternalCustomerRef  NVARCHAR(100) = NULL,
    @CustomerName         NVARCHAR(200),
    @Email                NVARCHAR(320) = NULL,
    @Phone                NVARCHAR(30) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customers
    SET ExternalCustomerRef = @ExternalCustomerRef,
        CustomerName = @CustomerName,
        Email = @Email,
        Phone = @Phone,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE TenantId = @TenantId
      AND CustomerId = @CustomerId;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO
