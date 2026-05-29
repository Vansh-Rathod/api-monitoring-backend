/*
Run this script after tables/types/procedures are created.
Demo login credentials:
  Email    : owner@demo.com
  Password : Pass@123
*/

SET NOCOUNT ON;

DECLARE @TenantCode NVARCHAR(50) = 'demo001';
DECLARE @OwnerEmail NVARCHAR(320) = 'owner@demo.com';
DECLARE @OwnerPasswordHash NVARCHAR(500) = '$2a$11$h.gOxrg1z6t.Nu5pwEX3xO9bwgik6.S0XJnUthMSnDFjEZgyOZRJe';
DECLARE @TenantId BIGINT;
DECLARE @UserId BIGINT;
DECLARE @CustomerId BIGINT;

SELECT @TenantId = TenantId
FROM dbo.Tenants
WHERE TenantCode = @TenantCode;

IF @TenantId IS NULL
BEGIN
    INSERT INTO dbo.Tenants (TenantCode, TenantName, PlanType, Status)
    VALUES (@TenantCode, 'Demo Tenant', 'Trial', 1);
    SET @TenantId = SCOPE_IDENTITY();
END;

SELECT @UserId = UserId
FROM dbo.Users
WHERE Email = @OwnerEmail;

IF @UserId IS NULL
BEGIN
    INSERT INTO dbo.Users (Email, PasswordHash, FirstName, LastName, IsActive)
    VALUES (@OwnerEmail, @OwnerPasswordHash, 'Demo', 'Owner', 1);
    SET @UserId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE dbo.Users
    SET PasswordHash = @OwnerPasswordHash,
        IsActive = 1,
        UpdatedAtUtc = SYSUTCDATETIME()
    WHERE UserId = @UserId;
END;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.TenantUsers
    WHERE TenantId = @TenantId
      AND UserId = @UserId
)
BEGIN
    INSERT INTO dbo.TenantUsers (TenantId, UserId, Role, IsPrimaryOwner, IsActive)
    VALUES (@TenantId, @UserId, 'Owner', 1, 1);
END;

SELECT @CustomerId = CustomerId
FROM dbo.Customers
WHERE TenantId = @TenantId
  AND ExternalCustomerRef = 'CUST-001';

IF @CustomerId IS NULL
BEGIN
    INSERT INTO dbo.Customers
    (
        TenantId, ExternalCustomerRef, CustomerName, Email, Phone,
        Status, OnboardedAtUtc, CreatedByUserId
    )
    VALUES
    (
        @TenantId, 'CUST-001', 'Acme Customer', 'customer@acme.com', '+91-9999999999',
        1, SYSUTCDATETIME(), @UserId
    );
    SET @CustomerId = SCOPE_IDENTITY();
END;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.Monitors
    WHERE TenantId = @TenantId
      AND MonitorName = 'Demo API Monitor'
)
BEGIN
    INSERT INTO dbo.Monitors
    (
        TenantId, CustomerId, MonitorName, BaseUrl, [Path], HttpMethod,
        CheckIntervalSeconds, TimeoutMs, ExpectedStatusCode, IsActive
    )
    VALUES
    (
        @TenantId, @CustomerId, 'Demo API Monitor', 'https://postman-echo.com', '/get', 'GET',
        60, 5000, 200, 1
    );
END;

SELECT @TenantId AS TenantId, @UserId AS UserId, @CustomerId AS CustomerId;
