USE InvenTrackDB;
GO

-- =============================================
-- USUARIOS
-- =============================================

CREATE OR ALTER PROCEDURE sp_GetUserByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Username, PasswordHash, FullName, Email, Role, IsActive, CreatedAt, LastLogin
    FROM Users
    WHERE Username = @Username;
END
GO

CREATE OR ALTER PROCEDURE sp_GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Username, PasswordHash, FullName, Email, Role, IsActive, CreatedAt, LastLogin
    FROM Users
    WHERE UserId = @UserId;
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Username, '' AS PasswordHash, FullName, Email, Role, IsActive, CreatedAt, LastLogin
    FROM Users
    ORDER BY FullName;
END
GO

CREATE OR ALTER PROCEDURE sp_CreateUser
    @Username     NVARCHAR(50),
    @PasswordHash NVARCHAR(200),
    @FullName     NVARCHAR(100),
    @Email        NVARCHAR(100),
    @Role         NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Users (Username, PasswordHash, FullName, Email, Role)
    VALUES (@Username, @PasswordHash, @FullName, @Email, @Role);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateUser
    @UserId   INT,
    @FullName NVARCHAR(100),
    @Email    NVARCHAR(100),
    @Role     NVARCHAR(20),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET FullName = @FullName, Email = @Email, Role = @Role, IsActive = @IsActive
    WHERE UserId = @UserId;
END
GO

-- =============================================
-- CATEGORIAS
-- =============================================

CREATE OR ALTER PROCEDURE sp_GetAllCategories
    @IncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryId, Name, Description, IsActive
    FROM Categories
    WHERE IsActive = 1 OR @IncludeInactive = 1
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE sp_GetCategoryById
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryId, Name, Description, IsActive
    FROM Categories
    WHERE CategoryId = @CategoryId;
END
GO

CREATE OR ALTER PROCEDURE sp_CreateCategory
    @Name        NVARCHAR(100),
    @Description NVARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Categories (Name, Description)
    VALUES (@Name, @Description);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateCategory
    @CategoryId  INT,
    @Name        NVARCHAR(100),
    @Description NVARCHAR(300),
    @IsActive    BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Categories
    SET Name = @Name, Description = @Description, IsActive = @IsActive
    WHERE CategoryId = @CategoryId;
END
GO

-- =============================================
-- PRODUCTOS
-- =============================================

CREATE OR ALTER PROCEDURE sp_GetAllProducts
    @Search     NVARCHAR(200) = NULL,
    @CategoryId INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.ProductId, p.Code, p.Name, p.Description, p.CategoryId,
        c.Name AS CategoryName,
        p.UnitPrice, p.CurrentStock, p.MinimumStock,
        p.Barcode, p.QRCode, p.RFIDTag, p.Unit,
        p.IsActive, p.CreatedAt, p.UpdatedAt
    FROM Products p
    INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE p.IsActive = 1
      AND (@Search IS NULL OR p.Name LIKE '%' + @Search + '%' OR p.Code LIKE '%' + @Search + '%')
      AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
    ORDER BY p.Name;
END
GO

CREATE OR ALTER PROCEDURE sp_GetProductById
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.ProductId, p.Code, p.Name, p.Description, p.CategoryId,
        c.Name AS CategoryName,
        p.UnitPrice, p.CurrentStock, p.MinimumStock,
        p.Barcode, p.QRCode, p.RFIDTag, p.Unit,
        p.IsActive, p.CreatedAt, p.UpdatedAt
    FROM Products p
    INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE p.ProductId = @ProductId;
END
GO

CREATE OR ALTER PROCEDURE sp_GetProductByCode
    @Code NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.ProductId, p.Code, p.Name, p.Description, p.CategoryId,
        c.Name AS CategoryName,
        p.UnitPrice, p.CurrentStock, p.MinimumStock,
        p.Barcode, p.QRCode, p.RFIDTag, p.Unit,
        p.IsActive, p.CreatedAt, p.UpdatedAt
    FROM Products p
    INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE p.Code = @Code AND p.IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE sp_GetProductByScanValue
    @ScanValue NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1
        p.ProductId, p.Code, p.Name, p.Description, p.CategoryId,
        c.Name AS CategoryName,
        p.UnitPrice, p.CurrentStock, p.MinimumStock,
        p.Barcode, p.QRCode, p.RFIDTag, p.Unit,
        p.IsActive, p.CreatedAt, p.UpdatedAt
    FROM Products p
    INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE p.IsActive = 1
      AND (p.Barcode = @ScanValue OR p.QRCode = @ScanValue OR p.RFIDTag = @ScanValue);
END
GO

CREATE OR ALTER PROCEDURE sp_CreateProduct
    @Code         NVARCHAR(30),
    @Name         NVARCHAR(200),
    @Description  NVARCHAR(500),
    @CategoryId   INT,
    @UnitPrice    DECIMAL(12,2),
    @CurrentStock INT,
    @MinimumStock INT,
    @Barcode      NVARCHAR(50),
    @QRCode       NVARCHAR(100),
    @RFIDTag      NVARCHAR(100),
    @Unit         NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Products (Code, Name, Description, CategoryId, UnitPrice, CurrentStock, MinimumStock, Barcode, QRCode, RFIDTag, Unit)
    VALUES (@Code, @Name, @Description, @CategoryId, @UnitPrice, @CurrentStock, @MinimumStock, @Barcode, @QRCode, @RFIDTag, @Unit);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateProduct
    @ProductId    INT,
    @Code         NVARCHAR(30),
    @Name         NVARCHAR(200),
    @Description  NVARCHAR(500),
    @CategoryId   INT,
    @UnitPrice    DECIMAL(12,2),
    @MinimumStock INT,
    @Barcode      NVARCHAR(50),
    @QRCode       NVARCHAR(100),
    @RFIDTag      NVARCHAR(100),
    @Unit         NVARCHAR(10),
    @IsActive     BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Products
    SET Code = @Code, Name = @Name, Description = @Description,
        CategoryId = @CategoryId, UnitPrice = @UnitPrice,
        MinimumStock = @MinimumStock, Barcode = @Barcode,
        QRCode = @QRCode, RFIDTag = @RFIDTag, Unit = @Unit,
        IsActive = @IsActive, UpdatedAt = GETDATE()
    WHERE ProductId = @ProductId;
END
GO

CREATE OR ALTER PROCEDURE sp_GetLowStockProducts
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.ProductId, p.Code, p.Name, p.Description, p.CategoryId,
        c.Name AS CategoryName,
        p.UnitPrice, p.CurrentStock, p.MinimumStock,
        p.Barcode, p.QRCode, p.RFIDTag, p.Unit,
        p.IsActive, p.CreatedAt, p.UpdatedAt
    FROM Products p
    INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE p.IsActive = 1 AND p.CurrentStock <= p.MinimumStock
    ORDER BY p.CurrentStock ASC;
END
GO

-- =============================================
-- ALMACENES
-- =============================================

CREATE OR ALTER PROCEDURE sp_GetAllWarehouses
AS
BEGIN
    SET NOCOUNT ON;
    SELECT WarehouseId, Name, Location, IsActive
    FROM Warehouses
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE sp_CreateWarehouse
    @Name     NVARCHAR(100),
    @Location NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Warehouses (Name, Location)
    VALUES (@Name, @Location);
    SELECT SCOPE_IDENTITY();
END
GO

-- =============================================
-- MOVIMIENTOS DE STOCK
-- =============================================

CREATE OR ALTER PROCEDURE sp_CreateStockMovement
    @ProductId         INT,
    @WarehouseId       INT,
    @Type              INT,
    @Quantity           INT,
    @ReferenceDocument NVARCHAR(50),
    @Notes             NVARCHAR(500),
    @CaptureMethod     NVARCHAR(20),
    @UserId            INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @PreviousStock INT;
    DECLARE @NewStock INT;

    SELECT @PreviousStock = CurrentStock FROM Products WHERE ProductId = @ProductId;

    SET @NewStock = CASE
        WHEN @Type IN (1, 4) THEN @PreviousStock + @Quantity
        WHEN @Type = 2       THEN @PreviousStock - @Quantity
        ELSE @PreviousStock
    END;

    IF @NewStock < 0
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50001, 'Stock insuficiente para realizar esta operacion.', 1;
    END

    UPDATE Products SET CurrentStock = @NewStock, UpdatedAt = GETDATE()
    WHERE ProductId = @ProductId;

    INSERT INTO StockMovements (ProductId, WarehouseId, Type, Quantity, PreviousStock, NewStock, ReferenceDocument, Notes, CaptureMethod, UserId)
    VALUES (@ProductId, @WarehouseId, @Type, @Quantity, @PreviousStock, @NewStock, @ReferenceDocument, @Notes, @CaptureMethod, @UserId);

    SELECT SCOPE_IDENTITY();

    COMMIT TRANSACTION;
END
GO

CREATE OR ALTER PROCEDURE sp_GetStockMovements
    @FromDate     DATETIME = NULL,
    @ToDate       DATETIME = NULL,
    @ProductId    INT      = NULL,
    @MovementType INT      = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        m.MovementId, m.ProductId, p.Name AS ProductName, p.Code AS ProductCode,
        m.WarehouseId, w.Name AS WarehouseName,
        m.Type, m.Quantity, m.PreviousStock, m.NewStock,
        m.ReferenceDocument, m.Notes, m.CaptureMethod,
        m.UserId, u.FullName AS UserFullName,
        m.CreatedAt
    FROM StockMovements m
    INNER JOIN Products   p ON m.ProductId   = p.ProductId
    INNER JOIN Warehouses w ON m.WarehouseId = w.WarehouseId
    INNER JOIN Users      u ON m.UserId      = u.UserId
    WHERE (@FromDate IS NULL OR m.CreatedAt >= @FromDate)
      AND (@ToDate IS NULL OR m.CreatedAt <= @ToDate)
      AND (@ProductId IS NULL OR m.ProductId = @ProductId)
      AND (@MovementType IS NULL OR m.Type = @MovementType)
    ORDER BY m.CreatedAt DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_GetRecentMovements
    @Count INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@Count)
        m.MovementId, m.ProductId, p.Name AS ProductName, p.Code AS ProductCode,
        m.WarehouseId, w.Name AS WarehouseName,
        m.Type, m.Quantity, m.PreviousStock, m.NewStock,
        m.ReferenceDocument, m.Notes, m.CaptureMethod,
        m.UserId, u.FullName AS UserFullName,
        m.CreatedAt
    FROM StockMovements m
    INNER JOIN Products   p ON m.ProductId   = p.ProductId
    INNER JOIN Warehouses w ON m.WarehouseId = w.WarehouseId
    INNER JOIN Users      u ON m.UserId      = u.UserId
    ORDER BY m.CreatedAt DESC;
END
GO

-- =============================================
-- EVENTOS DE ESCANEO
-- =============================================

CREATE OR ALTER PROCEDURE sp_CreateScanEvent
    @ScanValue   NVARCHAR(200),
    @Type        INT,
    @ProductId   INT,
    @WasResolved BIT,
    @UserId      INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ScanEvents (ScanValue, Type, ProductId, WasResolved, UserId)
    VALUES (@ScanValue, @Type, @ProductId, @WasResolved, @UserId);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_GetRecentScanEvents
    @Count INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@Count)
        s.ScanEventId, s.ScanValue, s.Type, s.ProductId,
        p.Name AS ProductName, s.WasResolved, s.UserId, s.ScannedAt
    FROM ScanEvents s
    LEFT JOIN Products p ON s.ProductId = p.ProductId
    ORDER BY s.ScannedAt DESC;
END
GO

-- =============================================
-- DASHBOARD
-- =============================================

CREATE OR ALTER PROCEDURE sp_GetDashboardSummary
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM Products WHERE IsActive = 1) AS TotalProducts,
        (SELECT COUNT(*) FROM Products WHERE IsActive = 1 AND CurrentStock <= MinimumStock AND CurrentStock > 0) AS LowStockProducts,
        (SELECT COUNT(*) FROM Products WHERE IsActive = 1 AND CurrentStock = 0) AS OutOfStockProducts,
        (SELECT COUNT(*) FROM StockMovements WHERE CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE)) AS TotalMovementsToday,
        (SELECT COUNT(*) FROM StockMovements WHERE CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE) AND Type = 1) AS EntriesToday,
        (SELECT COUNT(*) FROM StockMovements WHERE CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE) AND Type = 2) AS ExitsToday,
        (SELECT ISNULL(SUM(UnitPrice * CurrentStock), 0) FROM Products WHERE IsActive = 1) AS TotalInventoryValue;
END
GO
