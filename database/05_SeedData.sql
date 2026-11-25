USE InvenTrackDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Password: Admin123! (BCrypt hash)
INSERT INTO Users (Username, PasswordHash, FullName, Email, Role)
VALUES ('admin', '$2a$11$C02fBz.9MjY6GaVMrby/ouW8x4QwBOmoKns2Cprwy1M5iW.TKNsMi', 'Administrador del Sistema', 'admin@inventrackapp.com', 'Administrador');

-- Password: Oper123! (BCrypt hash)
INSERT INTO Users (Username, PasswordHash, FullName, Email, Role)
VALUES ('operario', '$2a$11$fZSCykh2ilK/GXnslEJFp.fDZ/Fzb0nvyfc335Ux8UaoD7p3GKTFG', 'Operario General', 'operario@inventrackapp.com', 'Operario');

INSERT INTO Categories (Name, Description) VALUES ('Electrónica', 'Dispositivos electrónicos y componentes');
INSERT INTO Categories (Name, Description) VALUES ('Accesorios', 'Cables, fundas, protectores y accesorios varios');
INSERT INTO Categories (Name, Description) VALUES ('Impresoras', 'Impresoras térmicas, de etiquetas y suministros');
INSERT INTO Categories (Name, Description) VALUES ('Lectores', 'Lectores de código de barras, QR y RFID');
INSERT INTO Categories (Name, Description) VALUES ('Redes', 'Equipos de red, access points y switches');
INSERT INTO Categories (Name, Description) VALUES ('Consumibles', 'Rollos de etiquetas, cintas, tóner y suministros');

INSERT INTO Warehouses (Name, Location) VALUES ('Almacén Central', 'Piso 1 - Zona A');
INSERT INTO Warehouses (Name, Location) VALUES ('Almacén Secundario', 'Piso 2 - Zona B');
INSERT INTO Warehouses (Name, Location) VALUES ('Despacho', 'Piso 1 - Zona C');

INSERT INTO Products (Code, Name, Description, CategoryId, UnitPrice, CurrentStock, MinimumStock, Barcode, QRCode, RFIDTag, Unit)
VALUES
('PROD-001', 'Lector RFID de Escritorio UHF',           'Lector de largo alcance para etiquetas RFID UHF',                                 4, 1250.00, 15, 5,  '7501234567001', 'QR-PROD-001', 'RFID-TAG-001', 'UND'),
('PROD-002', 'Terminal Portátil con Lector QR',          'Terminal portátil Android con lector de código QR integrado',                      4, 2800.00, 8,  3,  '7501234567002', 'QR-PROD-002', 'RFID-TAG-002', 'UND'),
('PROD-003', 'Impresora Térmica de Etiquetas ZT411',     'Impresora industrial de etiquetas con resolución 300dpi',                         3, 4500.00, 4,  2,  '7501234567003', 'QR-PROD-003', 'RFID-TAG-003', 'UND'),
('PROD-004', 'Etiquetas RFID UHF Autoadhesivas',         'Rollo de 500 etiquetas RFID UHF con adhesivo permanente',                        6, 180.00,  50, 20, '7501234567004', 'QR-PROD-004', 'RFID-TAG-004', 'ROL'),
('PROD-005', 'Access Point WiFi 6 Industrial',           'Access point para ambientes industriales con IP67',                               5, 1800.00, 12, 5,  '7501234567005', 'QR-PROD-005', 'RFID-TAG-005', 'UND'),
('PROD-006', 'Lector de Código de Barras 2D USB',        'Lector alámbrico USB para códigos 1D/2D',                                        4, 350.00,  25, 10, '7501234567006', 'QR-PROD-006', 'RFID-TAG-006', 'UND'),
('PROD-007', 'Ribbon de Cera para Impresora',            'Ribbon de transferencia térmica de cera, 110mm x 300m',                           6, 85.00,   3,  10, '7501234567007', 'QR-PROD-007', 'RFID-TAG-007', 'UND'),
('PROD-008', 'Antena RFID UHF Circular',                 'Antena de polarización circular para portales RFID',                              4, 950.00,  6,  3,  '7501234567008', 'QR-PROD-008', 'RFID-TAG-008', 'UND'),
('PROD-009', 'Cable USB-C para Terminal',                'Cable de datos USB-C de 1.5m para terminales portátiles',                         2, 25.00,   40, 15, '7501234567009', 'QR-PROD-009', 'RFID-TAG-009', 'UND'),
('PROD-010', 'Batería de Repuesto Terminal',              'Batería Li-Ion 5000mAh compatible con terminales portátiles',                     2, 120.00,  0,  5,  '7501234567010', 'QR-PROD-010', 'RFID-TAG-010', 'UND'),
('PROD-011', 'Switch PoE 8 Puertos Industrial',          'Switch gestionado con PoE+ para entornos industriales',                           5, 2200.00, 7,  3,  '7501234567011', 'QR-PROD-011', 'RFID-TAG-011', 'UND'),
('PROD-012', 'Funda Protectora Terminal',                 'Funda de silicona reforzada para terminal portátil',                              2, 45.00,   18, 8,  '7501234567012', 'QR-PROD-012', 'RFID-TAG-012', 'UND');

INSERT INTO StockMovements (ProductId, WarehouseId, Type, Quantity, PreviousStock, NewStock, ReferenceDocument, Notes, CaptureMethod, UserId, CreatedAt)
VALUES
(1, 1, 1, 20, 0,  20, 'OC-2024-001', 'Compra inicial de lectores RFID',     'Manual',  1, DATEADD(DAY, -10, GETDATE())),
(2, 1, 1, 10, 0,  10, 'OC-2024-002', 'Recepción de terminales portátiles',  'RFID',    1, DATEADD(DAY, -8,  GETDATE())),
(3, 1, 1, 6,  0,  6,  'OC-2024-003', 'Ingreso impresoras nuevas',           'Barcode', 1, DATEADD(DAY, -7,  GETDATE())),
(1, 1, 2, 5,  20, 15, 'GR-2024-001', 'Despacho a cliente Retail Corp',      'RFID',    1, DATEADD(DAY, -5,  GETDATE())),
(2, 1, 2, 2,  10, 8,  'GR-2024-002', 'Entrega a proyecto campo',            'QRCode',  1, DATEADD(DAY, -3,  GETDATE())),
(3, 1, 2, 2,  6,  4,  'GR-2024-003', 'Venta directa a Almacenes del Sur',   'Barcode', 1, DATEADD(DAY, -2,  GETDATE())),
(6, 1, 1, 30, 0,  30, 'OC-2024-004', 'Reposición de lectores USB',          'Manual',  1, DATEADD(DAY, -1,  GETDATE())),
(6, 1, 2, 5,  30, 25, 'GR-2024-004', 'Despacho a sucursal norte',           'Barcode', 1, GETDATE());
GO
