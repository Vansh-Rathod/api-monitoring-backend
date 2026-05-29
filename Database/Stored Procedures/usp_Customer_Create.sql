/* =========================================================
   2) Customer Create
   What it does:
   - Creates customer under tenant
   - Optionally enforces creator is member of tenant
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Customer_Create
(
    @TenantId             BIGINT,
    @ExternalCustomerRef  NVARCHAR(100) = NULL,
    @CustomerName         NVARCHAR(200),
    @Email                NVARCHAR(320) = NULL,
    @Phone                NVARCHAR(30) = NULL,
    @CreatedByUserId      BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.TenantUsers
        WHERE TenantId = @TenantId
          AND UserId = @CreatedByUserId
          AND IsActive = 1
    )
    BEGIN
        THROW 50001, 'Creator is not active in tenant.', 1;
    END

    INSERT INTO dbo.Customers
    (
        TenantId, ExternalCustomerRef, CustomerName, Email, Phone,
        Status, OnboardedAtUtc, CreatedByUserId
    )
    VALUES
    (
        @TenantId, @ExternalCustomerRef, @CustomerName, @Email, @Phone,
        1, SYSUTCDATETIME(), @CreatedByUserId
    );

    SELECT SCOPE_IDENTITY() AS CustomerId;
END
GO
