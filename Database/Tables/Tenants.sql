CREATE TABLE dbo.Tenants
(
    TenantId            BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantCode          NVARCHAR(50)  NOT NULL,
    TenantName          NVARCHAR(200) NOT NULL,
    PlanType            NVARCHAR(30)  NOT NULL,   -- e.g. Free/Pro/Enterprise
    Status              TINYINT       NOT NULL,   -- 1=Trial, 2=Active, 3=Suspended
    CreatedAtUtc        DATETIME2(3)  NOT NULL CONSTRAINT DF_Tenants_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc        DATETIME2(3)  NOT NULL CONSTRAINT DF_Tenants_UpdatedAtUtc DEFAULT SYSUTCDATETIME(),
    RowVersion          ROWVERSION,
    CONSTRAINT UQ_Tenants_TenantCode UNIQUE (TenantCode),
    CONSTRAINT CK_Tenants_Status CHECK (Status IN (1,2,3))
);
GO