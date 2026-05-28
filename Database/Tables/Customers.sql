CREATE TABLE dbo.Customers
(
    CustomerId           BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId             BIGINT         NOT NULL,
    ExternalCustomerRef  NVARCHAR(100)  NULL,
    CustomerName         NVARCHAR(200)  NOT NULL,
    Email                NVARCHAR(320)  NULL,
    Phone                NVARCHAR(30)   NULL,
    Status               TINYINT        NOT NULL, -- 1=Active, 2=Inactive, 3=Blocked
    OnboardedAtUtc       DATETIME2(3)   NOT NULL CONSTRAINT DF_Customers_OnboardedAtUtc DEFAULT SYSUTCDATETIME(),
    CreatedByUserId      BIGINT         NOT NULL,
    CreatedAtUtc         DATETIME2(3)   NOT NULL CONSTRAINT DF_Customers_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc         DATETIME2(3)   NOT NULL CONSTRAINT DF_Customers_UpdatedAtUtc DEFAULT SYSUTCDATETIME(),
    RowVersion           ROWVERSION,

    CONSTRAINT FK_Customers_Tenants       FOREIGN KEY (TenantId)        REFERENCES dbo.Tenants(TenantId),
    CONSTRAINT FK_Customers_CreatedByUser FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_Customers_Status CHECK (Status IN (1,2,3))
);
GO

CREATE INDEX IX_Customers_TenantId ON dbo.Customers(TenantId);
GO

-- Unique only when ExternalCustomerRef is provided
CREATE UNIQUE INDEX UX_Customers_Tenant_ExternalRef
ON dbo.Customers(TenantId, ExternalCustomerRef)
WHERE ExternalCustomerRef IS NOT NULL;
GO