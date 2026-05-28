CREATE TABLE dbo.TenantUsers
(
    TenantUserId        BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId            BIGINT        NOT NULL,
    UserId              BIGINT        NOT NULL,
    Role                NVARCHAR(30)  NOT NULL,   -- Owner/Admin/Viewer
    IsPrimaryOwner      BIT           NOT NULL CONSTRAINT DF_TenantUsers_IsPrimaryOwner DEFAULT (0),
    IsActive            BIT           NOT NULL CONSTRAINT DF_TenantUsers_IsActive DEFAULT (1),
    CreatedAtUtc        DATETIME2(3)  NOT NULL CONSTRAINT DF_TenantUsers_CreatedAtUtc DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_TenantUsers_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(TenantId),
    CONSTRAINT FK_TenantUsers_Users   FOREIGN KEY (UserId)   REFERENCES dbo.Users(UserId),
    CONSTRAINT UQ_TenantUsers_Tenant_User UNIQUE (TenantId, UserId),
    CONSTRAINT CK_TenantUsers_Role CHECK (Role IN ('Owner','Admin','Viewer'))
);
GO

CREATE INDEX IX_TenantUsers_UserId ON dbo.TenantUsers(UserId);
GO