CREATE OR ALTER PROCEDURE dbo.usp_Customer_List
(
    @TenantId     BIGINT,
    @PageNumber   INT = 1,
    @PageSize     INT = 50
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT CustomerId, ExternalCustomerRef, CustomerName, Email, Phone, Status, OnboardedAtUtc
    FROM dbo.Customers
    WHERE TenantId = @TenantId
    ORDER BY CustomerId DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Customers
    WHERE TenantId = @TenantId;
END
GO
