# Diccionario de Datos

## Tabla: Users
Almacena los usuarios del sistema con sus credenciales y roles.

| Columna | Tipo | Nulo | Default | Descripción |
|---|---|---|---|---|
| UserId | INT IDENTITY | No | Auto | Identificador único del usuario |
| Username | NVARCHAR(50) | No | - | Nombre de usuario (único) |
| PasswordHash | NVARCHAR(200) | No | - | Hash BCrypt de la contraseña |
| FullName | NVARCHAR(100) | No | - | Nombre completo del usuario |
| Email | NVARCHAR(100) | Sí | NULL | Correo electrónico |
| Role | NVARCHAR(20) | No | 'Operario' | Rol: 'Administrador' o 'Operario' |
| IsActive | BIT | No | 1 | Estado activo/inactivo |
| CreatedAt | DATETIME | No | GETDATE() | Fecha de creación del registro |
| LastLogin | DATETIME | Sí | NULL | Fecha del último inicio de sesión |

## Tabla: Categories
Catálogo de categorías para clasificar productos.

| Columna | Tipo | Nulo | Default | Descripción |
|---|---|---|---|---|
| CategoryId | INT IDENTITY | No | Auto | Identificador de categoría |
| Name | NVARCHAR(100) | No | - | Nombre de la categoría (único) |
| Description | NVARCHAR(300) | Sí | NULL | Descripción de la categoría |
| IsActive | BIT | No | 1 | Estado activo/inactivo |

## Tabla: Products
Registro maestro de productos con identificadores de captura automática.

| Columna | Tipo | Nulo | Default | Descripción |
|---|---|---|---|---|
| ProductId | INT IDENTITY | No | Auto | Identificador del producto |
| Code | NVARCHAR(30) | No | - | Código interno del producto (único) |
| Name | NVARCHAR(200) | No | - | Nombre del producto |
| Description | NVARCHAR(500) | Sí | NULL | Descripción detallada |
| CategoryId | INT (FK) | No | - | Referencia a Categories |
| UnitPrice | DECIMAL(12,2) | No | 0 | Precio unitario en soles |
| CurrentStock | INT | No | 0 | Stock actual en almacén |
| MinimumStock | INT | No | 5 | Stock mínimo para alerta |
| Barcode | NVARCHAR(50) | Sí | NULL | Código de barras EAN/UPC |
| QRCode | NVARCHAR(100) | Sí | NULL | Valor del código QR asignado |
| RFIDTag | NVARCHAR(100) | Sí | NULL | Identificador de tag RFID |
| Unit | NVARCHAR(10) | No | 'UND' | Unidad de medida |
| IsActive | BIT | No | 1 | Eliminación lógica |
| CreatedAt | DATETIME | No | GETDATE() | Fecha de registro |
| UpdatedAt | DATETIME | Sí | NULL | Fecha de última modificación |

## Tabla: Warehouses
Almacenes o ubicaciones físicas de almacenamiento.

| Columna | Tipo | Nulo | Default | Descripción |
|---|---|---|---|---|
| WarehouseId | INT IDENTITY | No | Auto | Identificador del almacén |
| Name | NVARCHAR(100) | No | - | Nombre del almacén (único) |
| Location | NVARCHAR(200) | Sí | NULL | Ubicación física |
| IsActive | BIT | No | 1 | Estado activo/inactivo |

## Tabla: StockMovements
Registro transaccional de todos los movimientos de inventario.

| Columna | Tipo | Nulo | Default | Descripción |
|---|---|---|---|---|
| MovementId | INT IDENTITY | No | Auto | Identificador del movimiento |
| ProductId | INT (FK) | No | - | Producto afectado |
| WarehouseId | INT (FK) | No | - | Almacén donde ocurrió |
| Type | INT | No | - | 1=Entrada, 2=Salida, 3=Transferencia, 4=Ajuste |
| Quantity | INT | No | - | Cantidad movida (siempre positiva) |
| PreviousStock | INT | No | - | Stock antes del movimiento |
| NewStock | INT | No | - | Stock después del movimiento |
| ReferenceDocument | NVARCHAR(50) | Sí | NULL | Documento de referencia (OC, GR) |
| Notes | NVARCHAR(500) | Sí | NULL | Notas del movimiento |
| CaptureMethod | NVARCHAR(20) | No | 'Manual' | Manual, Barcode, QRCode, RFID |
| UserId | INT (FK) | No | - | Usuario que registró |
| CreatedAt | DATETIME | No | GETDATE() | Fecha y hora del movimiento |

## Tabla: ScanEvents
Log de todos los eventos de escaneo del simulador de captura.

| Columna | Tipo | Nulo | Default | Descripción |
|---|---|---|---|---|
| ScanEventId | INT IDENTITY | No | Auto | Identificador del evento |
| ScanValue | NVARCHAR(200) | No | - | Valor capturado en el escaneo |
| Type | INT | No | - | 1=Barcode, 2=QRCode, 3=RFID |
| ProductId | INT (FK) | Sí | NULL | Producto resuelto (si aplica) |
| WasResolved | BIT | No | 0 | Si se encontró un producto |
| UserId | INT (FK) | No | - | Usuario que realizó el escaneo |
| ScannedAt | DATETIME | No | GETDATE() | Fecha y hora del escaneo |

## Reglas de Negocio Implementadas

1. **Stock no negativo**: CHECK constraint en Products (CurrentStock >= 0)
2. **Precio no negativo**: CHECK constraint en Products (UnitPrice >= 0)
3. **Cantidad positiva en movimientos**: CHECK constraint en StockMovements (Quantity > 0)
4. **Validación de roles**: CHECK constraint en Users (Role IN ('Administrador', 'Operario'))
5. **Validación de tipo de movimiento**: CHECK constraint (Type IN (1, 2, 3, 4))
6. **Validación de método de captura**: CHECK constraint (CaptureMethod IN ('Manual', 'Barcode', 'QRCode', 'RFID'))
7. **Eliminación lógica**: Productos y usuarios no se eliminan, se desactivan (IsActive = 0)
8. **Trigger de timestamp**: Actualiza automáticamente UpdatedAt en Products al modificar
9. **Trigger de validación**: Valida stock suficiente antes de registrar salidas
