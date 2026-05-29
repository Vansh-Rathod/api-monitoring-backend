/* =========================================================
   5) Customer Get/List
   What it does:
   - Read one customer
   - Read paginated customer list
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Customer_GetById
(
    @TenantId    BIGINT,
    @CustomerId  BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, TenantId, ExternalCustomerRef, CustomerName, Email, Phone,
           Status, OnboardedAtUtc, CreatedByUserId, CreatedAtUtc, UpdatedAtUtc
    FROM dbo.Customers
    WHERE TenantId = @TenantId
      AND CustomerId = @CustomerId;
END
GO
