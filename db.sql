-- Drop the database if it exists and create a new one
DROP DATABASE IF EXISTS milktea_pos_db;
CREATE DATABASE milktea_pos_db;
USE milktea_pos_db;

-- Categories Table
CREATE TABLE Categories (
    CategoryId INT AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(50),
    Description VARCHAR(200),
    Status BOOLEAN
);

-- Accounts Table (added FullName column)
CREATE TABLE Accounts (
    AccountId INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(100),
    FullName VARCHAR(100),
    Password_hash VARCHAR(255),
    Email VARCHAR(100),
    ImageUrl VARCHAR(255),
    Phone VARCHAR(20),
    Role ENUM('Staff', 'Manager'),
    Created_at DATETIME,
    Updated_at DATETIME,
    Status BOOLEAN
);

-- Products Table
CREATE TABLE Products (
    ProductId INT AUTO_INCREMENT PRIMARY KEY,
    ProductName VARCHAR(100),
    CategoryId INT,
    Description VARCHAR(500),
    ImageURL TEXT,
    Prize DECIMAL(10, 2),
    ProductType ENUM('MaterProduct', 'SingleProduct', 'Extra', 'Combo'),
    ParentID INT,
    SizeId INT,
    Create_at DATETIME,
    Create_by INT,
    Update_at DATETIME,
    Update_by INT,
    Disable_at DATETIME,
    Disable_by INT,
    Status BOOLEAN,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

-- Comboltems Table
CREATE TABLE Comboltems (
    ComboltemId INT AUTO_INCREMENT PRIMARY KEY,
    Combod INT,
    ProductID INT,
    Quantity INT,
    Discount INT,
    MasterID INT,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductId)
);

-- PaymentMethods Table
CREATE TABLE PaymentMethods (
    PaymentMethodId INT AUTO_INCREMENT PRIMARY KEY,
    MethodName VARCHAR(20),
    Description VARCHAR(500),
    Status BOOLEAN
);

-- Orders Table
CREATE TABLE Orders (
    OrderId INT AUTO_INCREMENT PRIMARY KEY,
    Create_at DATETIME,
    TotalAmount DECIMAL(10, 2),
    Note TEXT,
    StaffID INT,
    PaymentMethodId INT,
    FOREIGN KEY (StaffID) REFERENCES Accounts(AccountId),
    FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethods(PaymentMethodId)
);

-- OrderItems Table
CREATE TABLE OrderItems (
    OrderItemId INT AUTO_INCREMENT PRIMARY KEY,
    Quantity INT,
    Price DECIMAL(10, 2),
    MasterID INT,
    ProductId INT,
    OrderId INT,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);

-- OrderStatusUpdates Table
CREATE TABLE OrderStatusUpdates (
    OrderStatusUpdateId INT AUTO_INCREMENT PRIMARY KEY,
    OrderStatus ENUM('Pending', 'Shipped', 'Delivered', 'Cancelled'),
    OrderId INT,
    AccountId INT,
    Updated_at DATETIME,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);

-- Insert sample data into Categories Table
INSERT INTO Categories (CategoryName, Description, Status)
VALUES 
    ('Beverages', 'Drinks like tea, coffee, and milk tea', TRUE),
    ('Snacks', 'Light snacks like chips and cookies', TRUE),
    ('Desserts', 'Sweet treats like cakes and ice creams', TRUE);

-- Insert sample data into Accounts Table with FullName
INSERT INTO Accounts (Username, FullName, Password_hash, Email, ImageUrl, Phone, Role, Created_at, Updated_at, Status)
VALUES 
    ('john_doe', 'John Doe', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'john@example.com', 'https://example.com/images/john.jpg', '1234567890', 'Manager', NOW(), NOW(), TRUE),
    ('jane_smith', 'Jane Smith', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'jane@example.com', 'https://example.com/images/jane.jpg', '9876543210', 'Staff', NOW(), NOW(), TRUE),
    ('alice_brown', 'Alice Brown', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'alice@example.com', 'https://example.com/images/alice.jpg', '5555555555', 'Staff', NOW(), NOW(), TRUE);

INSERT INTO Products (ProductName, CategoryId, Description, ImageURL, Prize, ProductType, ParentID, SizeId, Create_at, Create_by, Update_at, Update_by, Disable_at, Disable_by, Status)
VALUES 
    ('Milk Tea', 1, 'Delicious milk tea with tapioca pearls', 'https://example.com/images/milk_tea.jpg', 5.99, 'SingleProduct', NULL, 1, NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Bubble Tea', 1, 'Classic bubble tea with fruit flavors', 'https://example.com/images/bubble_tea.jpg', 6.50, 'SingleProduct', NULL, 2, NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Chocolate Chip Cookies', 2, 'Freshly baked chocolate chip cookies', 'https://example.com/images/cookies.jpg', 3.00, 'SingleProduct', NULL, 3, NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Ice Cream Sundae', 3, 'A delicious chocolate and vanilla sundae', 'https://example.com/images/sundae.jpg', 4.50, 'SingleProduct', NULL, 4, NOW(), 1, NOW(), 1, NULL, NULL, TRUE);

-- Insert sample data into Comboltems Tablepassword
INSERT INTO Comboltems (Combod, ProductID, Quantity, Discount, MasterID)
VALUES
    (1, 1, 2, 10, NULL),
    (1, 2, 1, 5, NULL);

-- Insert sample data into PaymentMethods Table
INSERT INTO PaymentMethods (MethodName, Description, Status)
VALUES 
    ('Cash', 'Payment made in cash', TRUE),
    ('Credit Card', 'Payment made via credit card', TRUE),
    ('Online', 'Payment made through online services', TRUE);
