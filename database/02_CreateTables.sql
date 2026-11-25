USE InvenTrackDB;
GO

CREATE TABLE Users (
    UserId          INT IDENTITY(1,1) PRIMARY KEY,
    Username        NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(200) NOT NULL,
    FullName        NVARCHAR(100) NOT NULL,
    Email           NVARCHAR(100) NULL,
    Role            NVARCHAR(20)  NOT NULL DEFAULT 'Operario',
    IsActive        BIT           NOT NULL DEFAULT 1,
    CreatedAt       DATETIME      NOT NULL DEFAULT GETDATE(),
    LastLogin       DATETIME      NULL,
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Administrador', 'Operario'))
);
GO

CREATE TABLE Categories (
    CategoryId      INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100) NOT NULL UNIQUE,
    Description     NVARCHAR(300) NULL,
    IsActive        BIT           NOT NULL DEFAULT 1
);
GO

CREATE TABLE Products (
    ProductId       INT IDENTITY(1,1) PRIMARY KEY,
    Code            NVARCHAR(30)  NOT NULL UNIQUE,
    Name            NVARCHAR(200) NOT NULL,
    Description     NVARCHAR(500) NULL,
    CategoryId      INT           NOT NULL,
    UnitPrice       DECIMAL(12,2) NOT NULL DEFAULT 0,
    CurrentStock    INT           NOT NULL DEFAULT 0,
    MinimumStock    INT           NOT NULL DEFAULT 5,
    Barcode         NVARCHAR(50)  NULL,
    QRCode          NVARCHAR(100) NULL,
    RFIDTag         NVARCHAR(100) NULL,
    Unit            NVARCHAR(10)  NOT NULL DEFAULT 'UND',
    IsActive        BIT           NOT NULL DEFAULT 1,
    CreatedAt       DATETIME      NOT NULL DEFAULT GETDATE(),
    UpdatedAt       DATETIME      NULL,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT CK_Products_Stock CHECK (CurrentStock >= 0),
    CONSTRAINT CK_Products_Price CHECK (UnitPrice >= 0)
);
GO

CREATE TABLE Warehouses (
    WarehouseId     INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100) NOT NULL UNIQUE,
    Location        NVARCHAR(200) NULL,
    IsActive        BIT           NOT NULL DEFAULT 1
);
GO

CREATE TABLE StockMovements (
    MovementId          INT IDENTITY(1,1) PRIMARY KEY,
    ProductId           INT           NOT NULL,
    WarehouseId         INT           NOT NULL,
    Type                INT           NOT NULL,
    Quantity            INT           NOT NULL,
    PreviousStock       INT           NOT NULL,
    NewStock            INT           NOT NULL,
    ReferenceDocument   NVARCHAR(50)  NULL,
    Notes               NVARCHAR(500) NULL,
    CaptureMethod       NVARCHAR(20)  NOT NULL DEFAULT 'Manual',
    UserId              INT           NOT NULL,
    CreatedAt           DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Movements_Products   FOREIGN KEY (ProductId)   REFERENCES Products(ProductId),
    CONSTRAINT FK_Movements_Warehouses FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId),
    CONSTRAINT FK_Movements_Users      FOREIGN KEY (UserId)      REFERENCES Users(UserId),
    CONSTRAINT CK_Movements_Type CHECK (Type IN (1, 2, 3, 4)),
    CONSTRAINT CK_Movements_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_Movements_CaptureMethod CHECK (CaptureMethod IN ('Manual', 'Barcode', 'QRCode', 'RFID'))
);
GO

CREATE TABLE ScanEvents (
    ScanEventId     INT IDENTITY(1,1) PRIMARY KEY,
    ScanValue       NVARCHAR(200) NOT NULL,
    Type            INT           NOT NULL,
    ProductId       INT           NULL,
    WasResolved     BIT           NOT NULL DEFAULT 0,
    UserId          INT           NOT NULL,
    ScannedAt       DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ScanEvents_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_ScanEvents_Users    FOREIGN KEY (UserId)    REFERENCES Users(UserId),
    CONSTRAINT CK_ScanEvents_Type CHECK (Type IN (1, 2, 3))
);
GO

CREATE NONCLUSTERED INDEX IX_Products_Code        ON Products(Code);
CREATE NONCLUSTERED INDEX IX_Products_Barcode     ON Products(Barcode) WHERE Barcode IS NOT NULL;
CREATE NONCLUSTERED INDEX IX_Products_QRCode      ON Products(QRCode)  WHERE QRCode  IS NOT NULL;
CREATE NONCLUSTERED INDEX IX_Products_RFIDTag     ON Products(RFIDTag) WHERE RFIDTag IS NOT NULL;
CREATE NONCLUSTERED INDEX IX_Products_CategoryId  ON Products(CategoryId);
CREATE NONCLUSTERED INDEX IX_Movements_ProductId  ON StockMovements(ProductId);
CREATE NONCLUSTERED INDEX IX_Movements_CreatedAt  ON StockMovements(CreatedAt DESC);
CREATE NONCLUSTERED INDEX IX_ScanEvents_ScannedAt ON ScanEvents(ScannedAt DESC);
GO
