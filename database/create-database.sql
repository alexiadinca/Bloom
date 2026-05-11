IF DB_ID('BloomDb') IS NOT NULL
BEGIN
    ALTER DATABASE BloomDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BloomDb;
END
GO

CREATE DATABASE BloomDb;
GO

USE BloomDb;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    StockQuantity INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ShippingAddress NVARCHAR(500) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Orders_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

CREATE TABLE OrderItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    LineTotal DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_OrderItems_Orders
        FOREIGN KEY (OrderId) REFERENCES Orders(Id),

    CONSTRAINT FK_OrderItems_Products
        FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO

INSERT INTO Products (Name, Description, Category, Price, ImageUrl, StockQuantity)
VALUES
(
    'Exam Week Survival Kit',
    'A curated kit for exam season with coffee, a focus mug, lip balm, a candle and small comfort essentials.',
    'Study',
    34.99,
    'assets/images/ExamWeekSurvivalKit.png',
    25
),
(
    'Long Study Night Kit',
    'A productivity kit for late nights, including a planner, coffee cup, pen, snack bar and calming candle.',
    'Study',
    32.99,
    'assets/images/LongStudyNightKit.png',
    20
),
(
    'Dorm Room Starter Kit',
    'Cozy essentials for making a student room feel more comfortable, including a towel, candle, room spray and small plant.',
    'Room',
    36.99,
    'assets/images/DormRoomStarterKit.png',
    18
),
(
    'Coffee Addict Kit',
    'A coffee-themed kit with a travel cup, coffee blend, scoop and relaxing candle for busy mornings.',
    'Comfort',
    29.99,
    'assets/images/CoffeeAddictKit.png',
    30
),
(
    'Self-Care After Deadline Kit',
    'A relaxing kit for after stressful deadlines, including a sleep mask, hand cream, lip mask, scrunchie and candle.',
    'Self-Care',
    33.99,
    'assets/images/SelfCareAfterDeadlineKit.png',
    22
),
(
    'Rainy Campus Day Kit',
    'Useful comfort items for rainy days, including an umbrella, warm drink cup, towel, lip balm and hot cocoa.',
    'Comfort',
    28.99,
    'assets/images/RainyCampusDayKit.png',
    24
),
(
    'Presentation Day Kit',
    'A confidence kit for presentation days with a notebook, pen, perfume, hair clip and mints.',
    'Confidence',
    26.99,
    'assets/images/PresentationDayKit.png',
    21
),
(
    'Fresh Start Kit',
    'A reset kit for new weeks and new goals, including a planner, water bottle, candle and scrunchie.',
    'Reset',
    30.99,
    'assets/images/FreshStartKit.png',
    20
),
(
    'Focus Mode Kit',
    'A productivity kit with headphones, a focus notebook, timer and desk clock for deep work sessions.',
    'Focus',
    35.99,
    'assets/images/FocusModeKit.png',
    16
),
(
    'Weekend Reset Kit',
    'A weekend self-care kit with a pouch, face roller, moisturizer, scrunchie and scented candle.',
    'Self-Care',
    31.99,
    'assets/images/WeekendResetKit.png',
    19
);
GO

USE BloomDb;
GO

DELETE FROM Products;
GO

DBCC CHECKIDENT ('Products', RESEED, 0);
GO

SELECT * FROM Products;
GO