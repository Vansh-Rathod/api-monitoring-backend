CREATE OR ALTER PROCEDURE dbo.usp_Auth_GetRefreshTokenByHash
(
    @TokenHash NVARCHAR(128)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        RefreshTokenId,
        UserId,
        TenantId,
        TokenHash,
        JwtId,
        ExpiresAtUtc,
        RevokedAtUtc,
        CAST(CASE WHEN RevokedAtUtc IS NOT NULL THEN 1 ELSE 0 END AS bit) AS IsRevoked
    FROM dbo.UserRefreshTokens
    WHERE TokenHash = @TokenHash;
END
GO
