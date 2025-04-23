-- Drop the database if it exists and create a new one
DROP DATABASE IF EXISTS milktea_pos_db;
CREATE DATABASE milktea_pos_db;
USE milktea_pos_db;

-- Categories Table
CREATE TABLE Categories (
    CategoryId INT AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(50),
    Description VARCHAR(200),
    ImageUrl VARCHAR(255),
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
    SizeId ENUM('Parent', 'Small', 'Medium', 'Large'),
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
    ('Desserts', 'Sweet treats like cakes and ice creams', TRUE),
    ('Smoothies', 'Healthy smoothies made with fruits', TRUE),
    ('Juices', 'Freshly pressed juices', TRUE),
    ('Ice Cream', 'Cold and sweet ice creams', TRUE),
    ('Coffee', 'Different varieties of coffee', TRUE),
    ('Topping', 'More wonderful thing  ', TRUE),
    ('Combo', 'Save your Money', TRUE);

-- Insert sample data into Accounts Table with FullName
INSERT INTO Accounts (Username, FullName, Password_hash, Email, ImageUrl, Phone, Role, Created_at, Updated_at, Status)
VALUES 
    ('john_doe', 'John Doe', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'john@example.com', 'https://example.com/images/john.jpg', '1234567890', 'Manager', NOW(), NOW(), TRUE),
    ('jane_smith', 'Jane Smith', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'jane@example.com', 'https://example.com/images/jane.jpg', '9876543210', 'Staff', NOW(), NOW(), TRUE),
    ('alice_brown', 'Alice Brown', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'alice@example.com', 'https://example.com/images/alice.jpg', '5555555555', 'Staff', NOW(), NOW(), TRUE);

INSERT INTO Products (ProductName, CategoryId, Description, ImageURL, Prize, ProductType, ParentID, SizeId, Create_at, Create_by, Update_at, Update_by, Disable_at, Disable_by, Status)
VALUES 
	('Black Sugar Milk Tea', 1, 'Delicious milk tea with tapioca pearls', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745412959/bsgitc52q8tiisfwp3ly.jpg', 0, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Green Tea', 1, 'Refreshing green tea with a touch of honey', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745414931/2_ce10a7f5-3695-4534-bfb6-0fec1761c129_lowvjy.webp', 0, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Coffee', 7, 'Rich brewed coffee with a creamy finish', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415029/Coffe_wxej7m.jpg', 0, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Smoothie', 6, 'Fresh fruit smoothies for a healthy treat', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Smoothie_lebze0.jpg', 0, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Tropical Juice', 7, 'Freshly pressed juices in various flavors', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Tropical_Juice_ozyy64.jpg', 0, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Ice Cream', 6, 'Delicious creamy ice cream', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415359/Ice_Cream_muvg62.jpg', 0, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Cake', 3, 'Freshly baked cakes in various flavors', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Cake_bcuts4.jpg', 0, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Pastries', 3, 'Freshly baked pastries and bread', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Pastries_u8lkgg.jpg', 0, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
  
    ('Black Sugar Milk Tea', 1, 'Delicious milk tea with tapioca pearls', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745412959/bsgitc52q8tiisfwp3ly.jpg', 30.00, 'SingleProduct', 1, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Black Sugar Milk Tea', 1, 'Delicious milk tea with tapioca pearls', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745412959/bsgitc52q8tiisfwp3ly.jpg', 35.00, 'SingleProduct', 1, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Black Sugar Milk Tea', 1, 'Delicious milk tea with tapioca pearls', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745412959/bsgitc52q8tiisfwp3ly.jpg', 40.00, 'SingleProduct', 1, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	
    ('Green Tea', 1, 'Refreshing green tea with a touch of honey', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745414931/2_ce10a7f5-3695-4534-bfb6-0fec1761c129_lowvjy.webp', 25.00, 'SingleProduct', 2, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Green Tea', 1, 'Refreshing green tea with a touch of honey', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745414931/2_ce10a7f5-3695-4534-bfb6-0fec1761c129_lowvjy.webp', 28.00, 'SingleProduct', 2, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Green Tea', 1, 'Refreshing green tea with a touch of honey', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745414931/2_ce10a7f5-3695-4534-bfb6-0fec1761c129_lowvjy.webp', 32.00, 'SingleProduct', 2, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	
    ('Coffee', 7, 'Rich brewed coffee with a creamy finish', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415029/Coffe_wxej7m.jpg', 35.00, 'SingleProduct', 3, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Coffee', 7, 'Rich brewed coffee with a creamy finish', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415029/Coffe_wxej7m.jpg', 40.00, 'SingleProduct', 3, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Coffee', 7, 'Rich brewed coffee with a creamy finish', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415029/Coffe_wxej7m.jpg', 45.00, 'SingleProduct', 3, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	
    ('Smoothie', 6, 'Fresh fruit smoothies for a healthy treat', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Smoothie_lebze0.jpg', 40.00, 'SingleProduct', 4, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Smoothie', 6, 'Fresh fruit smoothies for a healthy treat', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Smoothie_lebze0.jpg', 45.00, 'SingleProduct', 4, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Smoothie', 6, 'Fresh fruit smoothies for a healthy treat', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Smoothie_lebze0.jpg', 50.00, 'SingleProduct', 4, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	
    ('Tropical Juice', 7, 'Freshly pressed juices in various flavors', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Tropical_Juice_ozyy64.jpg', 20.00, 'SingleProduct', 5, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Tropical Juice', 7, 'Freshly pressed juices in various flavors', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Tropical_Juice_ozyy64.jpg', 22.00, 'SingleProduct', 5, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Tropical Juice', 7, 'Freshly pressed juices in various flavors', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Tropical_Juice_ozyy64.jpg', 25.00, 'SingleProduct', 5, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    
    ('Ice Cream', 6, 'Delicious creamy ice cream', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415359/Ice_Cream_muvg62.jpg', 30.00, 'SingleProduct', 6, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Ice Cream', 6, 'Delicious creamy ice cream', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415359/Ice_Cream_muvg62.jpg', 35.00, 'SingleProduct', 6, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Ice Cream', 6, 'Delicious creamy ice cream', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415359/Ice_Cream_muvg62.jpg', 40.00, 'SingleProduct', 6, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
   
	('Cake', 3, 'Freshly baked cakes in various flavors', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Cake_bcuts4.jpg', 50.00, 'SingleProduct', 7, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Cake', 3, 'Freshly baked cakes in various flavors', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Cake_bcuts4.jpg', 55.00, 'SingleProduct', 7, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Cake', 3, 'Freshly baked cakes in various flavors', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Cake_bcuts4.jpg', 60.00, 'SingleProduct', 7, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    
    ('Pastries', 3, 'Freshly baked pastries and bread', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Pastries_u8lkgg.jpg', 20.00, 'SingleProduct', 8, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Pastries', 3, 'Freshly baked pastries and bread', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Pastries_u8lkgg.jpg', 25.00, 'SingleProduct', 8, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Pastries', 3, 'Freshly baked pastries and bread', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Pastries_u8lkgg.jpg', 30.00, 'SingleProduct', 8, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	-- Toppings
    ('Tapioca Pearls', 8, 'Sweet chewy tapioca pearls for your drinks', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417620/qorlc2r4vsawfhtqfa2e.jpg', 5.00, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Pudding', 8, 'Creamy pudding for extra sweetness', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/hapbpncswf1i0cp1kbo7.webp', 3.00, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	('Coconut Jelly', 8, 'Sweet coconut jelly for a tropical taste', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/vv8nspzp3exszetujroi.webp', 3.50, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Mango', 8, 'Fresh mango pieces to add a fruity twist', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/uandxjh6yozjunbhqp9k.jpg', 5.00, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	('Coffee Jelly', 8, 'Coffee-flavored jelly to enhance your drink', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/c64ff6fjwjrhuo4xhbvy.webp', 4.00, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Lychee Jelly', 8, 'Sweet lychee-flavored jelly for extra flavor', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/hekhohxjtq6eynpqxahu.avif', 3.50, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Oreo Crumbs', 8, 'Crunchy crushed Oreo pieces for a delicious topping', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/ltulutnxv2oldx016acl.webp', 4.00, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	('Milk Tea + Tapioca Pearls Combo', 9, 'Enjoy a milk tea with chewy tapioca pearls at a discounted price', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745418448/enfxjauozvwe7tfaffr2.jpg', 8.00, 'Combo', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE);
    
-- Insert sample data into Comboltems Tablepassword
INSERT INTO Comboltems (Combod, ProductID, Quantity, Discount, MasterID)
VALUES
    (40, 9, 1, 10, NULL),
    (40, 21, 1 , 5, NULL);

-- Insert sample data into PaymentMethods Table
INSERT INTO PaymentMethods (MethodName, Description, Status)
VALUES 
    ('Cash', 'Payment made in cash', TRUE),
    ('Credit Card', 'Payment made via credit card', TRUE),
    ('Online', 'Payment made through online services', TRUE);
