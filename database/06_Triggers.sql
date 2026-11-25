USE InvenTrackDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

CREATE OR ALTER TRIGGER tr_Products_UpdateTimestamp
ON Products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(UpdatedAt)
    BEGIN
        UPDATE p
        SET p.UpdatedAt = GETDATE()
        FROM Products p
        INNER JOIN inserted i ON p.ProductId = i.ProductId;
    END
END
GO
