/* =========================================================
   1) Onboarding: Create Tenant + Owner mapping
   What it does:
   - Creates tenant
   - Creates user if not exists by email
   - Maps user as tenant owner in TenantUsers
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Tenant_CreateWithOwner
(
    @TenantCode      NVARCHAR(50),
    @TenantName      NVARCHAR(200),
    @PlanType        NVARCHAR(30),
    @OwnerEmail      NVARCHAR(320),
    @OwnerFirstName  NVARCHAR(100) = NULL,
    @OwnerLastName   NVARCHAR(100) = NULL,
    @PasswordHash    NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @TenantId BIGINT, @UserId BIGINT;

    BEGIN TRAN;

    INSERT INTO dbo.Tenants (TenantCode, TenantName, PlanType, Status)
    VALUES (@TenantCode, @TenantName, @PlanType, 1); -- 1=Trial

    SET @TenantId = SCOPE_IDENTITY();

    SELECT @UserId = UserId
    FROM dbo.Users
    WHERE Email = @OwnerEmail;

    IF @UserId IS NULL
    BEGIN
        INSERT INTO dbo.Users (Email, PasswordHash, FirstName, LastName, IsActive)
        VALUES (@OwnerEmail, @PasswordHash, @OwnerFirstName, @OwnerLastName, 1);

        SET @UserId = SCOPE_IDENTITY();
    END

    INSERT INTO dbo.TenantUsers (TenantId, UserId, Role, IsPrimaryOwner, IsActive)
    VALUES (@TenantId, @UserId, 'Owner', 1, 1);

    COMMIT;

    SELECT @TenantId AS TenantId, @UserId AS OwnerUserId;
END
GO
