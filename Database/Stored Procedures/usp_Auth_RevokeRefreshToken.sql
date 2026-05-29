CREATE OR ALTER PROCEDURE dbo.usp_Auth_RevokeRefreshToken
(
    @TokenHash       NVARCHAR(128),
    @RevokedByIp     NVARCHAR(64) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.UserRefreshTokens
    SET RevokedAtUtc = SYSUTCDATETIME(),
        RevokedByIp = @RevokedByIp
    WHERE TokenHash = @TokenHash
      AND RevokedAtUtc IS NULL;

    SELECT @@ROWCOUNT AS AffectedRows;
END
GO
