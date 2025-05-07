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
    ToppingAllowed BOOLEAN,
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
    MasterID INT,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductId)
);
CREATE TABLE ToppingForProduct (
    ProductId INT,
    ToppingId INT,
    Quantity INT DEFAULT 1,  
    PRIMARY KEY (ProductId, ToppingId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (ToppingId) REFERENCES Products(ProductId) 
);
-- PaymentMethods Table
CREATE TABLE PaymentMethods (
    PaymentMethodId INT AUTO_INCREMENT PRIMARY KEY,
    MethodName VARCHAR(20),
    Description VARCHAR(500),
    Status BOOLEAN
);
CREATE TABLE Vouchers (
    VoucherId INT AUTO_INCREMENT PRIMARY KEY,
    VoucherCode VARCHAR(50) UNIQUE,
    DiscountAmount DECIMAL(10, 2),
    DiscountType ENUM('Amount', 'Percentage'),
    ExpirationDate DATETIME, 
    MinimumOrderAmount DECIMAL(10, 2) DEFAULT 0, 
    Status BOOLEAN,
    Create_at DATETIME,
    Create_by INT,
    Update_at DATETIME,
    Update_by INT,
	Disable_at DATETIME,
    Disable_by INT
);


-- Orders Table
CREATE TABLE Orders (
    OrderId INT AUTO_INCREMENT PRIMARY KEY,
    Create_at DATETIME,
    TotalAmount DECIMAL(10, 2),
    Note TEXT,
    StaffID INT,
    FOREIGN KEY (StaffID) REFERENCES Accounts(AccountId)
);

CREATE TABLE VoucherUsages (
    VoucherUsageId INT AUTO_INCREMENT PRIMARY KEY,
    VoucherId INT,
    OrderId INT,
    AmountUsed DECIMAL(10, 2),  -- Số tiền đã được giảm
    Used_at DATETIME,  -- Thời gian sử dụng voucher
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(VoucherId),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);
-- Transactions Table
CREATE TABLE Transactions (
    TransactionId INT AUTO_INCREMENT PRIMARY KEY,
    TransactionDate DATETIME,
    Amount DECIMAL(10, 2),
    AmountPaid DECIMAL(10, 2),  -- Total amount paid by the customer
    ChangeGiven DECIMAL(10, 2), -- Total amount of change returned to the customer
    TransactionType VARCHAR(20),
    Description TEXT,
    OrderId INT,
    PaymentMethodId INT,
    StaffId INT,
    Created_at DATETIME,
    Updated_at DATETIME,
    Status BOOLEAN,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethods(PaymentMethodId),
    FOREIGN KEY (StaffId) REFERENCES Accounts(AccountId)
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
    OrderStatus ENUM('Pending', 'Shipped', 'Delivered','Success', 'Cancelled'),
    OrderId INT,
    AccountId INT,
    Updated_at DATETIME,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);

-- Insert sample data into Categories Table
INSERT INTO Categories (CategoryName, Description, Status)
VALUES 
    ('Thức uống', 'Các loại đồ uống như trà, cà phê, và trà sữa', TRUE),
    ('Đồ ăn vặt', 'Các món ăn nhẹ như snack và bánh quy', TRUE),
    ('Món tráng miệng', 'Món ngọt như bánh và kem', TRUE),
    ('Smoothie', 'Smoothie lành mạnh làm từ trái cây', TRUE),
    ('Nước ép', 'Nước ép tươi mới', TRUE),
    ('Kem', 'Kem lạnh và ngọt ngào', TRUE),
    ('Cà phê', 'Các loại cà phê khác nhau', TRUE),
    ('Topping', 'Các topping tuyệt vời để thêm vào', TRUE),
    ('Combo', 'Tiết kiệm chi phí', TRUE);

-- Insert sample data into Accounts Table with FullName
INSERT INTO Accounts (Username, FullName, Password_hash, Email, ImageUrl, Phone, Role, Created_at, Updated_at, Status)
VALUES 
    ('DucHuanADmin', 'John Doe', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'Admin@example.com', 'https://example.com/images/john.jpg', '1234567890', 'Manager', NOW(), NOW(), TRUE),
    ('DucHuanStaff', 'Jane Smith', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'Staff@example.com', 'https://example.com/images/jane.jpg', '9876543210', 'Staff', NOW(), NOW(), TRUE),
    ('DucHuanStaff2', 'Alice Brown', '$2a$11$A46icdJigFN1XwpE2VFfs.bvFKBtF7go.zey13OZbEJ6yzu3JppHm', 'Staff2@example.com', 'https://example.com/images/alice.jpg', '5555555555', 'Staff', NOW(), NOW(), TRUE);

INSERT INTO Products (ProductName, CategoryId, Description, ImageURL, Prize, ProductType, ParentID, SizeId, Create_at, Create_by, Update_at, Update_by, Disable_at, Disable_by, Status)
VALUES 
	('Trà Sữa Đường Đen', 1, 'Trà sữa thơm ngon với trân châu', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745412959/bsgitc52q8tiisfwp3ly.jpg', NULL, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Trà Xanh', 1, 'Trà xanh tươi mát với một chút mật ong', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745414931/2_ce10a7f5-3695-4534-bfb6-0fec1761c129_lowvjy.webp', NULL, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Cà Phê', 7, 'Cà phê đậm đà với vị kem mịn màng', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415029/Coffe_wxej7m.jpg', NULL, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Smoothie', 6, 'Smoothie trái cây tươi ngon cho bữa ăn nhẹ', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Smoothie_lebze0.jpg', NULL, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Nước Ép Nhiệt Đới', 7, 'Nước ép tươi với các hương vị nhiệt đới', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Tropical_Juice_ozyy64.jpg', NULL, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Kem', 6, 'Kem mềm và thơm ngon', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415359/Ice_Cream_muvg62.jpg', NULL, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Bánh', 3, 'Bánh ngọt tươi mới với nhiều hương vị', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Cake_bcuts4.jpg', NULL, 'MaterProduct', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Trà Sữa Đường Đen', 1, 'Trà sữa thơm ngon với trân châu', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745412959/bsgitc52q8tiisfwp3ly.jpg', 25000, 'SingleProduct', 1, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Trà Sữa Đường Đen', 1, 'Trà sữa thơm ngon với trân châu', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745412959/bsgitc52q8tiisfwp3ly.jpg', 28000, 'SingleProduct', 1, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Trà Sữa Đường Đen', 1, 'Trà sữa thơm ngon với trân châu', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745412959/bsgitc52q8tiisfwp3ly.jpg', 35000, 'SingleProduct', 1, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Trà Xanh', 1, 'Trà xanh tươi mát với một chút mật ong', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745414931/2_ce10a7f5-3695-4534-bfb6-0fec1761c129_lowvjy.webp', 25000, 'SingleProduct', 2, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Trà Xanh', 1, 'Trà xanh tươi mát với một chút mật ong', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745414931/2_ce10a7f5-3695-4534-bfb6-0fec1761c129_lowvjy.webp', 28000, 'SingleProduct', 2, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Trà Xanh', 1, 'Trà xanh tươi mát với một chút mật ong', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745414931/2_ce10a7f5-3695-4534-bfb6-0fec1761c129_lowvjy.webp', 35000, 'SingleProduct', 2, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Cà Phê', 7, 'Cà phê đậm đà với vị kem mịn màng', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415029/Coffe_wxej7m.jpg', 25000, 'SingleProduct', 3, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Cà Phê', 7, 'Cà phê đậm đà với vị kem mịn màng', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415029/Coffe_wxej7m.jpg', 28000, 'SingleProduct', 3, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Cà Phê', 7, 'Cà phê đậm đà với vị kem mịn màng', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415029/Coffe_wxej7m.jpg', 35000, 'SingleProduct', 3, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Smoothie', 6, 'Smoothie trái cây tươi ngon cho bữa ăn nhẹ', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Smoothie_lebze0.jpg', 25000, 'SingleProduct', 4, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Smoothie', 6, 'Smoothie trái cây tươi ngon cho bữa ăn nhẹ', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Smoothie_lebze0.jpg', 28000, 'SingleProduct', 4, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Smoothie', 6, 'Smoothie trái cây tươi ngon cho bữa ăn nhẹ', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Smoothie_lebze0.jpg', 35000, 'SingleProduct', 4, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Nước Ép Nhiệt Đới', 7, 'Nước ép tươi với các hương vị nhiệt đới', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Tropical_Juice_ozyy64.jpg', 25000, 'SingleProduct', 5, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Nước Ép Nhiệt Đới', 7, 'Nước ép tươi với các hương vị nhiệt đới', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Tropical_Juice_ozyy64.jpg', 28000, 'SingleProduct', 5, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Nước Ép Nhiệt Đới', 7, 'Nước ép tươi với các hương vị nhiệt đới', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Tropical_Juice_ozyy64.jpg', 35000, 'SingleProduct', 5, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Kem', 6, 'Kem mềm và thơm ngon', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415359/Ice_Cream_muvg62.jpg', 25000, 'SingleProduct', 6, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Kem', 6, 'Kem mềm và thơm ngon', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415359/Ice_Cream_muvg62.jpg', 28000, 'SingleProduct', 6, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Kem', 6, 'Kem mềm và thơm ngon', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415359/Ice_Cream_muvg62.jpg', 35000, 'SingleProduct', 6, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Bánh', 3, 'Bánh ngọt tươi mới với nhiều hương vị', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Cake_bcuts4.jpg', 25000, 'SingleProduct', 7, 'Small', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Bánh', 3, 'Bánh ngọt tươi mới với nhiều hương vị', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Cake_bcuts4.jpg', 28000, 'SingleProduct', 7, 'Medium', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Bánh', 3, 'Bánh ngọt tươi mới với nhiều hương vị', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745415357/Cake_bcuts4.jpg', 35000, 'SingleProduct', 7, 'Large', NOW(), 1, NOW(), 1, NULL, NULL, TRUE), 
	-- Toppings
    ('Trân Châu Đen', 8, 'Trân châu mềm ngọt ngào cho đồ uống của bạn', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417620/qorlc2r4vsawfhtqfa2e.jpg', 5000, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Pudding', 8, 'Pudding kem ngọt ngào', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/hapbpncswf1i0cp1kbo7.webp', 3000, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Jelly Dừa', 8, 'Thạch dừa ngọt ngào cho một hương vị nhiệt đới', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/vv8nspzp3exszetujroi.webp', 3500, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Mango', 8, 'Mảnh xoài tươi thêm vào để làm đồ uống thêm hấp dẫn', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/uandxjh6yozjunbhqp9k.jpg', 5000, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Coffee Jelly', 8, 'Thạch cà phê để tăng thêm hương vị cho đồ uống', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/c64ff6fjwjrhuo4xhbvy.webp', 4000, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Thạch Lychee', 8, 'Thạch vị vải ngọt ngào thêm hương vị', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/hekhohxjtq6eynpqxahu.avif', 3500, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
    ('Oreo Bánh Quy', 8, 'Miếng Oreo băm nhỏ giòn rụm', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745417619/ltulutnxv2oldx016acl.webp', 4000, 'Extra', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE),
	('Milk Tea + Tapioca Pearls Combo', 9, 'Thưởng thức trà sữa với trân châu mềm giá ưu đãi', 'https://res.cloudinary.com/ddf9yp2yo/image/upload/v1745418448/enfxjauozvwe7tfaffr2.jpg', 18800, 'Combo', NULL, 'Parent', NOW(), 1, NOW(), 1, NULL, NULL, TRUE);
    
-- Insert sample data into Comboltems Tablepassword
INSERT INTO Comboltems (Combod, ProductID, Quantity, MasterID)
VALUES
    (40, 9, 1,  NULL),
    (40, 21, 1 ,  NULL);

-- Insert sample data into PaymentMethods Table
INSERT INTO PaymentMethods (MethodName, Description, Status)
VALUES 
    ('Cash', 'Payment made in cash', TRUE),
    ('Credit Card', 'Payment made via credit card', TRUE),
    ('Online', 'Payment made through online services', TRUE);

insert Into toppingforproduct (ProductId,ToppingId,Quantity) values
(1, 36 , 1);
