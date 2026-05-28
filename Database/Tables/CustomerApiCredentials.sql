CREATE TABLE dbo.CustomerApiCredentials
(
    CredentialId         BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId             BIGINT         NOT NULL,
    CustomerId           BIGINT         NOT NULL,
    CredentialType       NVARCHAR(20)   NOT NULL, -- ApiKey/Token
    CredentialHash       VARBINARY(64)  NOT NULL, -- store hash only
    Last4                NVARCHAR(4)    NULL,
    IsActive             BIT            NOT NULL CONSTRAINT DF_CustomerApiCredentials_IsActive DEFAULT (1),
    CreatedAtUtc         DATETIME2(3)   NOT NULL CONSTRAINT DF_CustomerApiCredentials_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    RotatedAtUtc         DATETIME2(3)   NULL,

    CONSTRAINT FK_CustomerApiCredentials_Tenants   FOREIGN KEY (TenantId)   REFERENCES dbo.Tenants(TenantId),
    CONSTRAINT FK_CustomerApiCredentials_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT CK_CustomerApiCredentials_Type CHECK (CredentialType IN ('ApiKey','Token'))
);
GO

CREATE INDEX IX_CustomerApiCredentials_Tenant_Customer
ON dbo.CustomerApiCredentials(TenantId, CustomerId);
GO