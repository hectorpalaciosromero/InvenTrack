# Manual de Usuario - InvenTrack

## 1. Inicio de Sesión

Al iniciar la aplicación se muestra la ventana de login.

**Credenciales por defecto:**

| Usuario | Contraseña | Rol |
|---|---|---|
| admin | Admin123! | Administrador |
| operario | Oper123! | Operario |

Ingrese su usuario y contraseña, luego presione **Iniciar Sesión**.

---

## 2. Pantalla Principal

La pantalla principal tiene dos áreas:

- **Barra lateral izquierda**: Menú de navegación con los módulos del sistema.
- **Área de contenido**: Muestra el módulo seleccionado.

Los módulos disponibles dependen del rol del usuario:
- **Administrador**: Acceso completo a todos los módulos incluyendo gestión de usuarios.
- **Operario**: Acceso a Dashboard, Productos, Simulador, Movimientos y Reportes.

---

## 3. Dashboard

Muestra indicadores clave del inventario en tiempo real:

- **Total de Productos**: Cantidad de productos activos registrados.
- **Stock Bajo**: Productos cuyo stock actual es menor o igual al stock mínimo.
- **Sin Stock**: Productos con stock en cero.
- **Valor Total del Inventario**: Sumatoria del valor de todo el inventario.
- **Movimientos del Día**: Entradas y salidas registradas en el día.
- **Alertas de Stock Bajo**: Lista de productos que requieren reposición.
- **Movimientos Recientes**: Últimos 10 movimientos registrados.

---

## 4. Gestión de Productos

### 4.1 Lista de Productos
- Visualizar todos los productos activos en una tabla.
- Filtrar por texto (código o nombre) y por categoría.
- Botón **Limpiar** para restablecer filtros.

### 4.2 Crear Producto
1. Presionar **Nuevo Producto**.
2. Completar los campos obligatorios: Código, Nombre y Categoría.
3. Usar el botón ⚡ para generar automáticamente código, barcode, QR y RFID.
4. Configurar precio, stock inicial y stock mínimo.
5. Presionar **Guardar**.

### 4.3 Editar Producto
1. Seleccionar un producto de la lista.
2. Presionar el botón ✏️ en la columna de acciones.
3. Modificar los campos necesarios.
4. Presionar **Guardar**.

### 4.4 Eliminar Producto
1. Seleccionar un producto de la lista.
2. Presionar el botón 🗑️ en la columna de acciones.
3. Confirmar la eliminación.

> La eliminación es lógica (el producto se desactiva, no se borra).

---

## 5. Simulador de Captura Automática

Módulo que simula la lectura de dispositivos RFID, QR y código de barras.

### 5.1 Seleccionar Tipo de Captura
Elegir entre: Código de Barras, Código QR o RFID.

### 5.2 Escaneo Simulado
Presionar **Simular Escaneo** para generar un valor de captura aleatorio.
El sistema buscará en la base de datos si algún producto tiene ese identificador.

### 5.3 Escaneo Manual
Ingresar un valor conocido (ej: "7501234567001") y presionar **Buscar**.

### 5.4 Registrar Movimiento desde Escaneo
Cuando un producto es identificado:
1. Seleccionar tipo de movimiento: Entrada o Salida.
2. Ingresar la cantidad.
3. Seleccionar el almacén.
4. Presionar **Registrar Movimiento**.

### 5.5 Historial de Escaneos
El panel derecho muestra el historial de escaneos de la sesión actual.

---

## 6. Movimientos de Almacén

Visualiza el historial completo de movimientos de inventario.

### 6.1 Filtros
- **Desde / Hasta**: Rango de fechas para consultar.
- Presionar **Actualizar** para aplicar.

### 6.2 Tipos de Movimiento
| Tipo | Descripción | Color |
|---|---|---|
| 📥 Entrada | Ingreso de mercadería al almacén | Verde |
| 📤 Salida | Despacho o consumo de mercadería | Rojo |
| 🔄 Transferencia | Movimiento entre almacenes | Azul |
| ⚙️ Ajuste | Corrección de inventario | Amarillo |

---

## 7. Reportes

### 7.1 Tipos de Reporte
- **Stock Actual**: Listado completo del inventario con valores.
- **Movimientos**: Detalle de movimientos en un rango de fechas.
- **Stock Bajo**: Productos que requieren reposición urgente.

### 7.2 Generar Reporte
1. Seleccionar el tipo de reporte.
2. Si es "Movimientos", configurar el rango de fechas.
3. Presionar **Generar Reporte**.

### 7.3 Exportar
Presionar **Exportar TXT** para guardar el reporte en un archivo de texto.

---

## 8. Gestión de Usuarios (Solo Administrador)

### 8.1 Crear Usuario
1. Presionar **Nuevo Usuario**.
2. Completar los campos: Usuario, Nombre, Email, Contraseña y Rol.
3. Presionar **Crear Usuario**.

### 8.2 Habilitar / Deshabilitar
1. Seleccionar un usuario de la lista.
2. Presionar **Habilitar/Deshabilitar** para cambiar su estado.

### 8.3 Resetear Contraseña
1. Seleccionar un usuario.
2. Presionar **Resetear Clave**.
3. La contraseña se restablece a "Reset123!".
