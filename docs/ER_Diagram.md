# Diagrama Entidad-Relación

## Modelo Relacional

```
┌──────────────────────┐       ┌──────────────────────────────────────────────────────┐
│       Users          │       │                    Products                          │
├──────────────────────┤       ├──────────────────────────────────────────────────────┤
│ PK  UserId       INT │       │ PK  ProductId       INT                             │
│     Username     NV  │       │     Code             NVARCHAR(30)   UNIQUE           │
│     PasswordHash NV  │       │     Name             NVARCHAR(200)                   │
│     FullName     NV  │       │     Description      NVARCHAR(500)  NULL             │
│     Email        NV  │       │ FK  CategoryId       INT            → Categories     │
│     Role         NV  │       │     UnitPrice         DECIMAL(12,2)                  │
│     IsActive     BIT │       │     CurrentStock      INT                            │
│     CreatedAt    DT  │       │     MinimumStock      INT                            │
│     LastLogin    DT  │       │     Barcode           NVARCHAR(50)   NULL             │
└──────────┬───────────┘       │     QRCode            NVARCHAR(100)  NULL             │
           │                   │     RFIDTag           NVARCHAR(100)  NULL             │
           │                   │     Unit              NVARCHAR(10)                    │
           │                   │     IsActive          BIT                             │
           │                   │     CreatedAt         DATETIME                        │
           │                   │     UpdatedAt         DATETIME       NULL             │
           │                   └───────────┬───────────────────────────────────────────┘
           │                               │
           │    ┌──────────────────────┐    │    ┌──────────────────────┐
           │    │    Categories        │    │    │    Warehouses        │
           │    ├──────────────────────┤    │    ├──────────────────────┤
           │    │ PK CategoryId    INT │    │    │ PK WarehouseId   INT │
           │    │    Name          NV  │    │    │    Name          NV  │
           │    │    Description   NV  │    │    │    Location      NV  │
           │    │    IsActive      BIT │    │    │    IsActive      BIT │
           │    └──────────────────────┘    │    └──────────┬───────────┘
           │                               │               │
           │    ┌──────────────────────────────────────────────────────────────┐
           │    │                    StockMovements                            │
           │    ├──────────────────────────────────────────────────────────────┤
           ├───→│ PK  MovementId        INT                                   │
           │    │ FK  ProductId          INT            → Products             │
           │    │ FK  WarehouseId        INT            → Warehouses           │
           │    │     Type               INT (1=Entrada, 2=Salida, 3=Transfer)│
           │    │     Quantity           INT                                   │
           │    │     PreviousStock      INT                                   │
           │    │     NewStock           INT                                   │
           │    │     ReferenceDocument  NVARCHAR(50)   NULL                   │
           │    │     Notes              NVARCHAR(500)  NULL                   │
           │    │     CaptureMethod      NVARCHAR(20)                          │
           ├───→│ FK  UserId             INT            → Users                │
           │    │     CreatedAt          DATETIME                              │
           │    └─────────────────────────────────────────────────────────────-┘
           │
           │    ┌──────────────────────────────────────────────────────────────┐
           │    │                      ScanEvents                             │
           │    ├──────────────────────────────────────────────────────────────┤
           └───→│ PK  ScanEventId       INT                                   │
                │     ScanValue          NVARCHAR(200)                         │
                │     Type               INT (1=Barcode, 2=QR, 3=RFID)        │
                │ FK  ProductId          INT            → Products  NULL       │
                │     WasResolved        BIT                                   │
                │ FK  UserId             INT            → Users                │
                │     ScannedAt          DATETIME                              │
                └─────────────────────────────────────────────────────────────-┘
```

## Relaciones

| Relación | Tipo | Descripción |
|---|---|---|
| Categories → Products | 1:N | Una categoría tiene muchos productos |
| Products → StockMovements | 1:N | Un producto tiene muchos movimientos |
| Warehouses → StockMovements | 1:N | Un almacén registra muchos movimientos |
| Users → StockMovements | 1:N | Un usuario registra muchos movimientos |
| Products → ScanEvents | 1:N | Un producto puede tener muchos escaneos |
| Users → ScanEvents | 1:N | Un usuario realiza muchos escaneos |

## Índices

| Tabla | Columna(s) | Tipo | Propósito |
|---|---|---|---|
| Products | Code | Non-Clustered | Búsqueda rápida por código |
| Products | Barcode | Non-Clustered (Filtered) | Resolución de escaneo |
| Products | QRCode | Non-Clustered (Filtered) | Resolución de escaneo |
| Products | RFIDTag | Non-Clustered (Filtered) | Resolución de escaneo |
| Products | CategoryId | Non-Clustered | Filtrado por categoría |
| StockMovements | ProductId | Non-Clustered | Historial por producto |
| StockMovements | CreatedAt DESC | Non-Clustered | Movimientos recientes |
| ScanEvents | ScannedAt DESC | Non-Clustered | Escaneos recientes |
