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
