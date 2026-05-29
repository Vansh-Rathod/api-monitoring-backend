CREATE OR ALTER PROCEDURE dbo.usp_Auth_GetUserForLogin
(
    @Email      NVARCHAR(320),
    @TenantId   BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        u.UserId,
        tu.TenantId,
        u.Email,
        u.PasswordHash,
        LTRIM(RTRIM(CONCAT(ISNULL(u.FirstName, ''), ' ', ISNULL(u.LastName, '')))) AS FullName,
        tu.Role,
        CAST(CASE WHEN u.IsActive = 1 AND tu.IsActive = 1 THEN 1 ELSE 0 END AS bit) AS IsActive
    FROM dbo.Users u
    INNER JOIN dbo.TenantUsers tu
        ON tu.UserId = u.UserId
       AND tu.TenantId = @TenantId
    WHERE u.Email = @Email;
END
GO
