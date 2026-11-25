USE InvenTrackDB;
GO

CREATE OR ALTER TRIGGER tr_Products_UpdateTimestamp
ON Products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Products
    SET UpdatedAt = GETDATE()
    FROM Products p
    INNER JOIN inserted i ON p.ProductId = i.ProductId;
END
GO

CREATE OR ALTER TRIGGER tr_StockMovements_ValidateStock
ON StockMovements
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Products p ON i.ProductId = p.ProductId
        WHERE i.Type = 2 AND p.CurrentStock < i.Quantity
    )
    BEGIN
        THROW 50001, 'No se puede registrar la salida: stock insuficiente.', 1;
        RETURN;
    END

    INSERT INTO StockMovements (ProductId, WarehouseId, Type, Quantity, PreviousStock, NewStock, ReferenceDocument, Notes, CaptureMethod, UserId, CreatedAt)
    SELECT ProductId, WarehouseId, Type, Quantity, PreviousStock, NewStock, ReferenceDocument, Notes, CaptureMethod, UserId, CreatedAt
    FROM inserted;

    UPDATE p
    SET CurrentStock = CASE
            WHEN i.Type IN (1, 4) THEN p.CurrentStock + i.Quantity
            WHEN i.Type = 2       THEN p.CurrentStock - i.Quantity
            ELSE p.CurrentStock
        END
    FROM Products p
    INNER JOIN inserted i ON p.ProductId = i.ProductId;
END
GO
