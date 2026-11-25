USE InvenTrackDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

CREATE OR ALTER VIEW vw_ProductStock AS
SELECT
    p.ProductId,
    p.Code,
    p.Name AS ProductName,
    c.Name AS CategoryName,
    p.UnitPrice,
    p.CurrentStock,
    p.MinimumStock,
    p.Unit,
    CASE
        WHEN p.CurrentStock = 0 THEN 'Sin Stock'
        WHEN p.CurrentStock <= p.MinimumStock THEN 'Stock Bajo'
        ELSE 'Normal'
    END AS StockStatus,
    p.UnitPrice * p.CurrentStock AS TotalValue
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.CategoryId
WHERE p.IsActive = 1;
GO

CREATE OR ALTER VIEW vw_MovementHistory AS
SELECT
    m.MovementId,
    m.CreatedAt,
    p.Code AS ProductCode,
    p.Name AS ProductName,
    w.Name AS WarehouseName,
    CASE m.Type
        WHEN 1 THEN 'Entrada'
        WHEN 2 THEN 'Salida'
        WHEN 3 THEN 'Transferencia'
        WHEN 4 THEN 'Ajuste'
    END AS MovementTypeName,
    m.Quantity,
    m.PreviousStock,
    m.NewStock,
    m.CaptureMethod,
    m.ReferenceDocument,
    u.FullName AS UserName
FROM StockMovements m
INNER JOIN Products   p ON m.ProductId   = p.ProductId
INNER JOIN Warehouses w ON m.WarehouseId = w.WarehouseId
INNER JOIN Users      u ON m.UserId      = u.UserId;
GO

CREATE OR ALTER VIEW vw_DailyMovementSummary AS
SELECT
    CAST(m.CreatedAt AS DATE) AS MovementDate,
    CASE m.Type
        WHEN 1 THEN 'Entrada'
        WHEN 2 THEN 'Salida'
        WHEN 3 THEN 'Transferencia'
        WHEN 4 THEN 'Ajuste'
    END AS MovementTypeName,
    COUNT(*)       AS TotalMovements,
    SUM(m.Quantity) AS TotalQuantity
FROM StockMovements m
GROUP BY CAST(m.CreatedAt AS DATE), m.Type;
GO
