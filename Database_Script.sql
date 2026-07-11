-- Create Database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EnhanzerDB')
BEGIN
    CREATE DATABASE EnhanzerDB;
END
GO

USE EnhanzerDB;
GO

-- Create Location_Details table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Location_Details')
BEGIN
    CREATE TABLE Location_Details (
        Location_Code NVARCHAR(100) PRIMARY KEY,
        Location_Name NVARCHAR(255) NOT NULL
    );
END
GO

-- Create PurchaseBills table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseBills')
BEGIN
    CREATE TABLE PurchaseBills (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CreatedAt DATETIME2 NOT NULL,
        TotalItems INT NOT NULL,
        TotalQuantity INT NOT NULL,
        TotalCost DECIMAL(18,2) NOT NULL,
        TotalSelling DECIMAL(18,2) NOT NULL
    );
END
GO

-- Create PurchaseBillItems table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseBillItems')
BEGIN
    CREATE TABLE PurchaseBillItems (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PurchaseBillId INT NOT NULL FOREIGN KEY REFERENCES PurchaseBills(Id) ON DELETE CASCADE,
        ItemName NVARCHAR(MAX) NOT NULL,
        Batch NVARCHAR(MAX) NOT NULL,
        StandardCost DECIMAL(18,2) NOT NULL,
        StandardPrice DECIMAL(18,2) NOT NULL,
        Margin DECIMAL(18,2) NOT NULL,
        Qty INT NOT NULL,
        FreeQty INT NOT NULL,
        Discount DECIMAL(18,2) NOT NULL,
        TotalCost DECIMAL(18,2) NOT NULL,
        TotalSelling DECIMAL(18,2) NOT NULL
    );
END
GO
