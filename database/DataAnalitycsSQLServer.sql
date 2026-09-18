create table Productos
(
	ProductoID int identity (1,1) primary key,
	Nombre varchar(100) not null unique,
	Stock int not null,
	PrecioUnitario decimal(18,2) not null

);


create table Ventas
(
	VentaID int identity (1,1) primary key,
	ProductoID int foreign key references Productos(ProductoID),
	Cantidad int not null,
	MontoTotal decimal(18,2) not null,
	Fecha datetime default getdate()
);

insert into Productos (Nombre, Stock, PrecioUnitario)values 
('Laptop Asus',50,850.00),
('Monitor LG',30,200.00),
('Teclado Mecanico',100,45.00);


select * from Productos
--------------------------------------------------------------------------------------
--trigger se dispara al insertar una venta para restar stock automaticamente
create trigger trg_ActualizarStock
on Ventas
after insert
as
begin
	set nocount on;

	update p
	set p.Stock = p.Stock - i.Cantidad
	from Productos p
	inner join inserted i on p.ProductoID = i.ProductoID
end;


-------------------------------------------------------------------------------
-- sp que inserta la venta calculando el total y validando stock
CREATE DATABASE DataAnalyticsEngineDb;
GO

USE DataAnalyticsEngineDb;
GO

-- 1. Tablas principales
CREATE TABLE Productos (
    ProductoId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Stock INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL
);

CREATE TABLE Ventas (
    VentaId INT IDENTITY(1,1) PRIMARY KEY,
    ProductoId INT FOREIGN KEY REFERENCES Productos(ProductoId),
    Cantidad INT NOT NULL,
    MontoTotal DECIMAL(18,2) NOT NULL,
    Fecha DATETIME DEFAULT GETDATE()
);

-- Insertamos productos de prueba con Stock inicial
INSERT INTO Productos (Nombre, Stock, PrecioUnitario) VALUES 
('Laptop Asus', 50, 850.00),
('Monitor LG', 30, 200.00),
('Teclado Mecanico', 100, 45.00);
GO

-- 2. TRIGGER: Se dispara al insertar una Venta para restar Stock automáticamente
CREATE TRIGGER trg_ActualizarStock
ON Ventas
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE p
    SET p.Stock = p.Stock - i.Cantidad
    FROM Productos p
    INNER JOIN inserted i ON p.ProductoId = i.ProductoId;
END;
GO
-------------------------------------------------------------------------------------
-- 3. STORED PROCEDURE: Inserta la venta calculando el total y validando stock
CREATE PROCEDURE sp_RegistrarVenta
    @NombreProducto VARCHAR(100),
    @Cantidad INT,
    @Fecha DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ProductoId INT;
    DECLARE @Precio DECIMAL(18,2);
    DECLARE @StockActual INT;

    -- Buscar ID, Precio y Stock del producto
    SELECT @ProductoId = ProductoId, @Precio = PrecioUnitario, @StockActual = Stock 
    FROM Productos 
    WHERE Nombre = @NombreProducto;

    -- Validaciones
    IF @ProductoId IS NULL
    BEGIN
        RAISERROR('El producto no existe.', 16, 1);
        RETURN;
    END

    IF @StockActual < @Cantidad
    BEGIN
        RAISERROR('Stock insuficiente para realizar la venta.', 16, 1);
        RETURN;
    END

    -- Registrar la venta (El trigger restará el stock automáticamente)
    INSERT INTO Ventas (ProductoId, Cantidad, MontoTotal, Fecha)
    VALUES (@ProductoId, @Cantidad, (@Cantidad * @Precio), @Fecha);
END;
GO

-- 4. VIEW: La vista limpia optimizada para Power BI
CREATE VIEW vw_ReporteAnaliticoVentas AS
SELECT 
    v.VentaId,
    p.Nombre AS Producto,
    v.Cantidad,
    p.PrecioUnitario,
    v.MontoTotal,
    v.Fecha,
    p.Stock AS StockActualizado
FROM Ventas v
INNER JOIN Productos p ON v.ProductoId = p.ProductoId;



select * from Productos
select * from Ventas
