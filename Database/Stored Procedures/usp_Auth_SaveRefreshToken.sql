CREATE OR ALTER PROCEDURE dbo.usp_Auth_SaveRefreshToken
(
    @UserId          BIGINT,
    @TenantId        BIGINT,
    @TokenHash       NVARCHAR(128),
    @JwtId           NVARCHAR(100) = NULL,
    @ExpiresAtUtc    DATETIME2(3),
    @CreatedByIp     NVARCHAR(64) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.UserRefreshTokens
    (
        UserId, TenantId, TokenHash, JwtId, ExpiresAtUtc, CreatedByIp
    )
    VALUES
    (
        @UserId, @TenantId, @TokenHash, @JwtId, @ExpiresAtUtc, @CreatedByIp
    );

    SELECT SCOPE_IDENTITY() AS RefreshTokenId;
END
GO
