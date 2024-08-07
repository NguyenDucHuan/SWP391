﻿USE master;

CREATE DATABASE [DiamondShopManagement] 

USE [DiamondShopManagement] 

-- B?ng vai trò
CREATE TABLE [dbo].[tblRole](
    [roleID] INT NOT NULL,
    [roleName] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_tblRole] PRIMARY KEY CLUSTERED ([roleID] ASC)
);

-- B?ng ng??i dùng
CREATE TABLE [dbo].[tblUsers](
    [userID] NVARCHAR(50) NOT NULL,
    [userName] NVARCHAR(50) NOT NULL UNIQUE,
    [fullName] NVARCHAR(50) NOT NULL,
    [email] NVARCHAR(50) UNIQUE NOT NULL,
    [password] NVARCHAR(50) NOT NULL,
    [roleID] INT NOT NULL,
    [status] BIT DEFAULT 1,
    [resetCode] NVARCHAR(10),
    [bonusPoint] INT NULL,
	createDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_tblUsers] PRIMARY KEY CLUSTERED ([userID] ASC),
    CONSTRAINT [FK_tblUsers_tblRole] FOREIGN KEY ([roleID]) REFERENCES dbo.tblRole ([roleID]) ON DELETE CASCADE
);

-- B?ng voucher
CREATE TABLE [dbo].[tblVoucher](
    [voucherID] INT IDENTITY(1,1) NOT NULL,
    [startTime] DATETIME NOT NULL,
    [endTime] DATETIME NOT NULL,
    [discount] INT,
    [quantity] INT,
	[targetUserID] NVARCHAR(50) NOT NULL,
    [status] BIT DEFAULT 1,
    CONSTRAINT [PK_tblVoucher] PRIMARY KEY CLUSTERED ([voucherID] ASC)
);


-- B?ng s? h?u voucher
CREATE TABLE [dbo].[tblVoucherCatch](
    [voucherID] INT NOT NULL,
    [userID] NVARCHAR(50) NOT NULL,
	CONSTRAINT [PK_tblVoucherCatch] PRIMARY KEY CLUSTERED ([voucherID] asc, [userID] asc),
    CONSTRAINT [FK_tblVoucherCatch_tblUsers] FOREIGN KEY ([userID]) REFERENCES dbo.tblUsers ([userID]),
    CONSTRAINT [FK_tblVoucherCatch_tblVoucher] FOREIGN KEY ([voucherID]) REFERENCES dbo.tblVoucher([voucherID])
);

-- B?ng tin nh?n
CREATE TABLE [dbo].[tblChat](
    [chatID] INT NOT NULL,
    [chatDetail] NVARCHAR(512) NOT NULL,
    [date] DATETIME NOT NULL,
    [senderID] NVARCHAR(50) NOT NULL,
    [receiverID] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_tblChat] PRIMARY KEY CLUSTERED ([chatID] ASC),
    CONSTRAINT [FK_tblChat_tblUsers_sender] FOREIGN KEY ([senderID]) REFERENCES dbo.tblUsers ([userID]),
    CONSTRAINT [FK_tblChat_tblUsers_receiver] FOREIGN KEY ([receiverID]) REFERENCES dbo.tblUsers ([userID])
);

-- B?ng thông báo
CREATE TABLE [dbo].[tblNotification](
    [notificationID] NVARCHAR(50) NOT NULL,
    [userID] NVARCHAR(50),
    [date] DATETIME NOT NULL,
    [detail] NVARCHAR(512) NOT NULL,
    [status] BIT DEFAULT 1,
    CONSTRAINT [PK_tblNotification] PRIMARY KEY CLUSTERED ([notificationID] ASC),
    CONSTRAINT [FK_tblNotification_tblUsers] FOREIGN KEY ([userID]) REFERENCES dbo.tblUsers ([userID])
);
-- Bước 1: Tạo cột tạm thời để lưu notificationID hiện tại (nếu cần)
ALTER TABLE tblNotification ADD tempNotificationID NVARCHAR(50);

-- Bước 2: Sao chép dữ liệu từ cột notificationID hiện tại vào cột tạm thời
UPDATE tblNotification SET tempNotificationID = notificationID;

-- Bước 3: Xóa ràng buộc khóa chính hiện tại
ALTER TABLE tblNotification DROP CONSTRAINT PK_tblNotification;

-- Bước 4: Xóa cột notificationID hiện tại
ALTER TABLE tblNotification DROP COLUMN notificationID;

-- Bước 5: Thêm cột notificationID mới với kiểu INT IDENTITY(1,1) và đặt làm khóa chính
ALTER TABLE tblNotification ADD notificationID INT IDENTITY(1,1) PRIMARY KEY;

-- Bước 6: Nếu cần, sao chép lại dữ liệu từ cột tạm thời vào cột mới (nếu có dữ liệu cũ cần lưu trữ)

-- Bước 7: Xóa cột tạm thời
ALTER TABLE tblNotification DROP COLUMN tempNotificationID;

-- Bước 8: Tạo lại ràng buộc khóa ngoại (nếu cần)
-- ALTER TABLE tblNotification ADD CONSTRAINT FK_tblNotification_tblUsers FOREIGN KEY (userID) REFERENCES dbo.tblUsers (userID);


-- B?ng kim c??ng
CREATE TABLE [dbo].[tblDiamonds](
    [diamondID] INT IDENTITY(1,1) NOT NULL,
    [diamondName] NVARCHAR(50) NOT NULL,
    [diamondPrice] MONEY NOT NULL,
    [diamondDescription] NVARCHAR(512) NOT NULL,
    [caratWeight] FLOAT NOT NULL,
    [clarityID] NVARCHAR(20) NOT NULL,
    [cutID] NVARCHAR(20) NOT NULL,
    [colorID] NVARCHAR(20) NOT NULL,
    [shapeID] NVARCHAR(20) NOT NULL,
    [diamondImagePath] NVARCHAR(512) NOT NULL,
    [status] BIT DEFAULT 1,
    [quantity] INT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_tblDiamonds] PRIMARY KEY CLUSTERED ([diamondID] ASC)
);
ALTER TABLE [dbo].[tblDiamonds]
ADD [rebuyPrice] MONEY NULL,
    [stillHaveCertification] BIT NULL;
ALTER TABLE tblDiamonds ADD detailStatus NVARCHAR(50) DEFAULT 'To Receive';



-- B?ng ch?ng ch?
CREATE TABLE [dbo].[tblCertificate](
    [certificateID] INT IDENTITY(1,1) NOT NULL,
    [diamondID] INT NOT NULL,
    [certificateNumber] NVARCHAR(50) NOT NULL,
    [issueDate] DATE NOT NULL,
    [certifyingAuthority] NVARCHAR(100) NOT NULL,
    [cerImagePath] NVARCHAR(512) NOT NULL,
    CONSTRAINT [PK_tblCertificate] PRIMARY KEY CLUSTERED ([certificateID] ASC),
    CONSTRAINT [FK_tblCertificate_tblDiamonds] FOREIGN KEY ([diamondID]) REFERENCES dbo.tblDiamonds ([diamondID]) ON DELETE CASCADE
);

-- B?ng bình lu?n
CREATE TABLE [dbo].[tblComment](
    [commentID] NVARCHAR(50) NOT NULL,
    [userID] NVARCHAR(50) NOT NULL,
    [diamondID] INT NOT NULL,
    [comment] NVARCHAR(512),
    [rating] INT,
    CONSTRAINT [PK_tblComment] PRIMARY KEY CLUSTERED ([commentID] ASC, [userID] ASC, [diamondID] ASC),
    CONSTRAINT [FK_tblComment_tblUsers] FOREIGN KEY ([userID]) REFERENCES dbo.tblUsers ([userID]) ON DELETE CASCADE,
    CONSTRAINT [FK_tblComment_tblDiamonds] FOREIGN KEY ([diamondID]) REFERENCES dbo.tblDiamonds ([diamondID]) ON DELETE CASCADE
);

-- B?ng ??n hàng
CREATE TABLE [dbo].[tblOrder](
    [orderID] NVARCHAR(50) NOT NULL,
    [customerID] NVARCHAR(50) NOT NULL,
    [deliveryStaffID] NVARCHAR(50) NULL,
    [saleStaffID] NVARCHAR(50) NULL,
    [totalMoney] FLOAT NOT NULL,
    [paidAmount] FLOAT NOT NULL DEFAULT 0,
    [remainingAmount] AS ([totalMoney] - [paidAmount]),
    [status] NVARCHAR(50) DEFAULT 'NOT READY',
    [paymentStatus] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [address] NVARCHAR(100) NOT NULL,
    [phone] NVARCHAR(50) NOT NULL,
    [saleDate] DATETIME NOT NULL,
    CONSTRAINT [PK_tblOrder] PRIMARY KEY CLUSTERED ([orderID] ASC),
    CONSTRAINT [FK_tblOrder_tblUsers_1] FOREIGN KEY ([customerID]) REFERENCES dbo.tblUsers ([userID]),
    CONSTRAINT [FK_tblOrder_tblUsers_2] FOREIGN KEY ([deliveryStaffID]) REFERENCES dbo.tblUsers ([userID]),
    CONSTRAINT [FK_tblOrder_tblUsers_3] FOREIGN KEY ([saleStaffID]) REFERENCES dbo.tblUsers ([userID])
);

ALTER TABLE [dbo].[tblOrder]
ADD [customerName] NVARCHAR(50) NULL;

ALTER TABLE [dbo].[tblOrder]
ADD [deliveryStaffName] NVARCHAR(250) NULL;

-- Thêm cột voucherID vào bảng tblOrder
ALTER TABLE [dbo].[tblOrder]
ADD [voucherID] INT NULL;

-- B?ng giao d?ch
   CREATE TABLE [dbo].[tblTransaction](
    [transactionID] INT IDENTITY(1,1) NOT NULL,
    [orderID] NVARCHAR(50) NULL,
    [userID] NVARCHAR(50) NOT NULL,
    [transactionType] NVARCHAR(50) NOT NULL,
    [transactionDate] DATETIME NOT NULL,
    [amount] FLOAT NOT NULL,
    [paymentMethod] NVARCHAR(50) NOT NULL DEFAULT 'PayPal',
    CONSTRAINT [PK_tblTransaction] PRIMARY KEY CLUSTERED ([transactionID] ASC),
    CONSTRAINT [FK_tblTransaction_tblOrder] FOREIGN KEY ([orderID]) REFERENCES dbo.tblOrder ([orderID]) ON DELETE CASCADE,
    CONSTRAINT [FK_tblTransaction_tblUsers] FOREIGN KEY ([userID]) REFERENCES dbo.tblUsers ([userID]) ON DELETE CASCADE
);


-- B?ng v? kim c??ng
CREATE TABLE [dbo].[tblSettings](
    [settingID] INT IDENTITY(1,1) NOT NULL,
    [settingType] NVARCHAR(50) NOT NULL,
    [material] NVARCHAR(50) NOT NULL,
    [priceTax] MONEY NOT NULL,
    [quantityStones] INT NOT NULL,
    [description] NVARCHAR(512),
    [imagePath] NVARCHAR(512),
    [status] BIT DEFAULT 1,
    CONSTRAINT [PK_tblSettings] PRIMARY KEY CLUSTERED ([settingID] ASC)
);
-- B?ng kim c??ng ph?
CREATE TABLE [dbo].[tblAccentStones](
    [accentStoneID] INT IDENTITY(1,1) NOT NULL,
	[accentStonesName] NVARCHAR(250) NOT NULL,
    [shape] NVARCHAR(50) NOT NULL,
    [caratWeight] FLOAT NOT NULL,
    [clarity] NVARCHAR(20) NOT NULL,
    [color] NVARCHAR(20) NOT NULL,
    [price] MONEY NOT NULL,
    [quantity] INT NOT NULL DEFAULT 0,
    [imagePath] NVARCHAR(512),
    [status] BIT DEFAULT 1,
    CONSTRAINT [PK_tblAccentStones] PRIMARY KEY CLUSTERED ([accentStoneID] ASC)
);


-- B?ng l?u s? l??ng kim c??ng ph? c?n thi?t cho m?i v? kim c??ng
CREATE TABLE [dbo].[tblItem](
    [ItemID] INT IDENTITY(1,1) NOT NULL,
	[settingID] INT NULL,
    [accentStoneID] INT NULL,
    [quantityAccent] INT NULL,
	[diamondID] INT NULL,
	[diamondPrice] MONEY NOT NULL,
	[settingPrice] MONEY NOT NULL,
	[accentStonePrice] MONEY NULL,
	CONSTRAINT [PK_tblItem] PRIMARY KEY CLUSTERED ([ItemID] ASC),
    CONSTRAINT [FK_tblItem_tblSettings] FOREIGN KEY ([settingID]) REFERENCES [dbo].[tblSettings]([settingID]),
	CONSTRAINT [FK_tblItem_tblDiamond] FOREIGN KEY ([diamondID]) REFERENCES [dbo].[tblDiamonds]([diamondID]),
    CONSTRAINT [FK_tblItem_tblAccentStones] FOREIGN KEY ([accentStoneID]) REFERENCES [dbo].[tblAccentStones]([accentStoneID])
);
-- B?ng chi ti?t ??n hàng
CREATE TABLE [dbo].[tblOrderItem](
    [orderID] NVARCHAR(50) NOT NULL,
    [ItemID] INT NOT NULL,
    [salePriceItem] MONEY NOT NULL,
    [warrantyCode] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_tblOrderItem] PRIMARY KEY CLUSTERED ([orderID], [ItemID], [warrantyCode]),
    CONSTRAINT [FK_tblOrderItem_tblOrder] FOREIGN KEY ([orderID]) REFERENCES dbo.tblOrder ([orderID]) ON DELETE CASCADE,
    CONSTRAINT [FK_tblOrderItem_tblItem] FOREIGN KEY ([ItemID]) REFERENCES dbo.tblItem ([ItemID]) ON DELETE CASCADE
);

-- Warranty table
CREATE TABLE [dbo].[tblWarranty](
    [warrantyID] INT IDENTITY(1,1) NOT NULL,
    [orderID] NVARCHAR(50) NOT NULL,
    [ItemID] INT NOT NULL,
    [warrantyStartDate] DATETIME NOT NULL,
    [warrantyEndDate] DATETIME NOT NULL,
    [warrantyDetails] NVARCHAR(512),
    [warrantyCode] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_tblWarranty] PRIMARY KEY CLUSTERED ([warrantyID] ASC),
    CONSTRAINT [FK_tblWarranty_tblOrderItem] FOREIGN KEY ([orderID], [ItemID], [warrantyCode]) REFERENCES [dbo].[tblOrderItem]([orderID], [ItemID], [warrantyCode]) ON DELETE CASCADE,
    CONSTRAINT [UQ_tblWarranty_tblOrderItem] UNIQUE ([orderID], [ItemID], [warrantyCode])
);
-- Xóa ràng buộc UNIQUE KEY hiện tại
ALTER TABLE [dbo].[tblWarranty] 
DROP CONSTRAINT [UQ_tblWarranty_tblOrderItem];
-- Thêm cột status vào bảng tblWarranty
ALTER TABLE [dbo].[tblWarranty]
ADD [status] NVARCHAR(50) NULL;

-- B?ng c?p nh?t tr?ng thái ??n hàng
CREATE TABLE [dbo].[tblOrderStatusUpdates](
    [updateID] INT IDENTITY(1,1) NOT NULL,
    [orderID] NVARCHAR(50) NOT NULL,
    [status] NVARCHAR(50) NOT NULL,
    [updateTime] DATETIME NOT NULL,
    CONSTRAINT [PK_tblOrderStatusUpdates] PRIMARY KEY CLUSTERED ([updateID] ASC),
    CONSTRAINT [FK_tblOrderStatusUpdates_tblOrder] FOREIGN KEY ([orderID]) REFERENCES dbo.tblOrder ([orderID]) ON DELETE CASCADE
);


-- Thêm d? li?u m?u vào b?ng tblRole
INSERT INTO [dbo].[tblRole] ([roleID], [roleName]) VALUES 
(1, 'user'),
(2, 'admin'),
(3, 'manager'),
(4, 'deliverystaff'),
(5, 'salestaff');

-- Thêm d? li?u m?u vào b?ng tblSettings
INSERT INTO [dbo].[tblSettings] ([settingType], [material],[priceTax],[quantityStones] , [description], [imagePath]) VALUES 
('Ring', 'Gold', 500, 10, 'Gold ring setting', '/Image/Settings/ring_gold.png'),
('Necklace', 'Silver', 300, 15, 'Silver necklace setting', '/Image/Settings/necklace_silver.png');

-- Thêm d? li?u m?u vào b?ng tblAccentStones
INSERT INTO [dbo].[tblAccentStones] ([shape], [caratWeight], [clarity], [color], [price], [quantity], [imagePath], [accentStonesName]) VALUES 
('Round', 0.25, 'VS2', 'H', 200, 50, '/Image/AccentStones/round_025.png', 'Round Diamond'),
('Princess', 0.2, 'VS1', 'G', 180, 40, '/Image/AccentStones/princess_02.png', 'Princess Diamond');

-- Thêm d? li?u m?u vào b?ng tblDiamonds
INSERT INTO [dbo].[tblDiamonds] 
    ([diamondName], [diamondPrice], [diamondDescription], [caratWeight], [clarityID], [cutID], [colorID], [shapeID], [diamondImagePath], [status]) 
VALUES 
    ('1.01 Carat Round Diamond', 4080, 'This 1.01 round H diamond is sold exclusively.', 1.01, 'VS2', 'Excellent', 'H', 'Round', '/Image/DiamondDTO/Diamonds/dia1A.png|/Image/DiamondDTO/Diamonds/dia1B.png|/Image/DiamondDTO/Diamonds/dia1C.png', 1),
    ('1.00 Carat Princess Diamond', 2400, 'This 1.00 Carat Princess Diamond is exclusively', 1.00, 'VS2', 'Ideal', 'H', 'Princess', '/Image/DiamondDTO/Diamonds/dia2A.png|/Image/DiamondDTO/Diamonds/dia2B.png|/Image/DiamondDTO/Diamonds/dia2C.png', 1),
    ('2.08 Carat Cushion Modified Diamond', 11190, 'This 2.08 cushion modified H diamond is sold exclusively.', 2.08, 'VS1', 'Ideal', 'H', 'Cushion', '/Image/DiamondDTO/Diamonds/dia3A.png|/Image/DiamondDTO/Diamonds/dia3B.png|/Image/DiamondDTO/Diamonds/dia3C.png', 1),
    ('2.51 Carat Emerald Diamond', 23490, 'This 2.51 Carat Emerald Diamond is sold exclusive.', 2.51, 'VS1', 'Ideal', 'H', 'Emerald', '/Image/DiamondDTO/Diamonds/dia4A.png|/Image/DiamondDTO/Diamonds/dia4B.png|/Image/DiamondDTO/Diamonds/dia4C.png', 1),
    ('2.50 Carat Oval Diamond', 24630, '2.50 Carat Oval Diamond is sold exclusive.', 2.50, 'VS2', 'Ideal', 'G', 'Oval', '/Image/DiamondDTO/Diamonds/dia5A.png|/Image/DiamondDTO/Diamonds/dia5B.png|/Image/DiamondDTO/Diamonds/dia5C.png', 1),
    ('1.81 Carat Radiant Diamond', 12480, '1.81 Carat Radiant Diamond is sold exclusive', 1.81, 'VS2', 'Ideal', 'H', 'Radiant', '/Image/DiamondDTO/Diamonds/dia6A.png|/Image/DiamondDTO/Diamonds/dia6B.png|/Image/DiamondDTO/Diamonds/dia6C.png', 1),
    ('1.00 Carat Round Diamond', 4020, '1.00 Carat Round H Diamond is sold exclusive', 1.00, 'VS2', 'Excellent', 'H', 'Round', '/Image/DiamondDTO/Diamonds/dia7A.png|/Image/DiamondDTO/Diamonds/dia7B.png|/Image/DiamondDTO/Diamonds/dia7C.png', 1),
    ('1.00 Carat Round Diamond', 4240, '1.00 Round H diamond is sold exclusive', 1.00, 'VS2', 'Excellent', 'H', 'Round', '/Image/DiamondDTO/Diamonds/dia8A.png|/Image/DiamondDTO/Diamonds/dia8B.png|/Image/DiamondDTO/Diamonds/dia8C.png', 1),
    ('1.08 Carat Round Diamond', 4370, '1.08 Round H diamond is sold exclusive', 1.08, 'VS2', 'Excellent', 'H', 'Round', '/Image/DiamondDTO/Diamonds/dia9A.png|/Image/DiamondDTO/Diamonds/dia9B.png|/Image/DiamondDTO/Diamonds/dia9C.png', 1),
    ('1.03 Carat Round Diamond', 4440, '1.03 Round H diamond is sold exclusive', 1.03, 'VS2', 'Excellent', 'H', 'Round', '/Image/DiamondDTO/Diamonds/dia10A.png|/Image/DiamondDTO/Diamonds/dia10B.png|/Image/DiamondDTO/Diamonds/dia10C.png', 1),
    ('1.02 Carat Princess Diamond', 2410, 'This 1.02 Carat Princess Diamond is exclusively', 1.02, 'VS1', 'Ideal', 'H', 'Princess', '/Image/DiamondDTO/Diamonds/dia11A.png|/Image/DiamondDTO/Diamonds/dia11B.png|/Image/DiamondDTO/Diamonds/dia11C.png', 1),
    ('1.00 Carat Princess Diamond', 2630, 'This 1.00 Carat Princess Diamond is exclusively', 1.00, 'VS2', 'Ideal', 'H', 'Princess', '/Image/DiamondDTO/Diamonds/dia12A.png|/Image/DiamondDTO/Diamonds/dia12B.png|/Image/DiamondDTO/Diamonds/dia12C.png', 1),
    ('1.00 Carat Princess Diamond', 2650, 'This 1.00 Carat Princess Diamond is exclusively', 1.00, 'VS2', 'Ideal', 'G', 'Princess', '/Image/DiamondDTO/Diamonds/dia13A.png|/Image/DiamondDTO/Diamonds/dia13B.png|/Image/DiamondDTO/Diamonds/dia13C.png', 1),
    ('1.00 Carat Princess Diamond', 2700, 'This 1.00 Carat Princess Diamond is exclusively', 1.00, 'VS2', 'Ideal', 'G', 'Princess', '/Image/DiamondDTO/Diamonds/dia14A.png|/Image/DiamondDTO/Diamonds/dia14B.png|/Image/DiamondDTO/Diamonds/dia14C.png', 1),
    ('2.08 Carat Cushion Modified Diamond', 11190, 'This 2.08 cushion modified H diamond is sold exclusively.', 2.08, 'VS1', 'Ideal', 'H', 'Cushion', '/Image/DiamondDTO/Diamonds/dia15A.png|/Image/DiamondDTO/Diamonds/dia15B.png|/Image/DiamondDTO/Diamonds/dia15C.png', 1),
    ('1.06 Carat Cushion Modified Diamond', 5090, 'This 1.06 cushion modified G diamond is sold exclusively.', 1.06, 'VVS1', 'Ideal', 'G', 'Cushion', '/Image/DiamondDTO/Diamonds/dia16A.png|/Image/DiamondDTO/Diamonds/dia16B.png|/Image/DiamondDTO/Diamonds/dia16C.png', 1),
    ('1.50 Carat Cushion Modified Diamond', 8380, 'This 1.50 cushion modified G diamond is sold exclusively.', 1.50, 'VVS2', 'Ideal', 'G', 'Cushion', '/Image/DiamondDTO/Diamonds/dia17A.png|/Image/DiamondDTO/Diamonds/dia17B.png|/Image/DiamondDTO/Diamonds/dia17C.png', 1),
    ('1.83 Carat Cushion Modified Diamond', 15760, 'This 1.83 cushion modified E diamond is sold exclusively.', 1.83, 'VVS2', 'Ideal', 'E', 'Cushion', '/Image/DiamondDTO/Diamonds/dia18A.png|/Image/DiamondDTO/Diamonds/dia18B.png|/Image/DiamondDTO/Diamonds/dia18C.png', 1),
    ('1.00 Carat Emerald Diamond', 2620, 'This 1.00 Carat Emerald Diamond is sold exclusive.', 1.00, 'VS2', 'Ideal', 'H', 'Emerald', '/Image/DiamondDTO/Diamonds/dia19A.png|/Image/DiamondDTO/Diamonds/dia19B.png|/Image/DiamondDTO/Diamonds/dia19C.png', 1),
    ('3.00 Carat Emerald Diamond', 38890, 'This 3.00 Carat Emerald Diamond is sold exclusive.', 3.00, 'VS1', 'Ideal', 'H', 'Emerald', '/Image/DiamondDTO/Diamonds/dia20A.png|/Image/DiamondDTO/Diamonds/dia20B.png|/Image/DiamondDTO/Diamonds/dia20C.png', 1),
    ('1.21 Carat Emerald Diamond', 3620, 'This 1.21 Carat Emerald Diamond is sold exclusive.', 1.21, 'VS2', 'Ideal', 'H', 'Emerald', '/Image/DiamondDTO/Diamonds/dia21A.png|/Image/DiamondDTO/Diamonds/dia21B.png|/Image/DiamondDTO/Diamonds/dia21C.png', 1),
    ('2.51 Carat Emerald Diamond', 23490, 'This 2.51 Carat Emerald Diamond is sold exclusive.', 2.51, 'VS2', 'Ideal', 'F', 'Emerald', '/Image/DiamondDTO/Diamonds/dia22A.png|/Image/DiamondDTO/Diamonds/dia22B.png|/Image/DiamondDTO/Diamonds/dia22C.png', 1),
    ('1.01 Carat Oval Diamond', 29920, '1.01 Carat Oval Diamond is sold exclusive.', 1.01, 'VS2', 'Ideal', 'H', 'Oval', '/Image/DiamondDTO/Diamonds/dia23A.png|/Image/DiamondDTO/Diamonds/dia23B.png|/Image/DiamondDTO/Diamonds/dia23C.png', 1),
    ('2.50 Carat Oval Diamond', 24630, '2.50 Carat Oval Diamond is sold exclusive.', 2.50, 'VS2', 'Ideal', 'F', 'Oval', '/Image/DiamondDTO/Diamonds/dia24A.png|/Image/DiamondDTO/Diamonds/dia24B.png|/Image/DiamondDTO/Diamonds/dia24C.png', 1),
    ('2.01 Carat Oval Diamond', 29160, '2.01 Carat Oval Diamond is sold exclusive.', 2.01, 'IF', 'Ideal', 'H', 'Oval', '/Image/DiamondDTO/Diamonds/dia25A.png|/Image/DiamondDTO/Diamonds/dia25B.png|/Image/DiamondDTO/Diamonds/dia25C.png', 1),
    ('1.81 Carat Oval Diamond', 12480, '1.81 Carat Oval Diamond is sold exclusive', 1.81, 'SI1', 'Ideal', 'H', 'Oval', '/Image/DiamondDTO/Diamonds/dia26A.png|/Image/DiamondDTO/Diamonds/dia26B.png|/Image/DiamondDTO/Diamonds/dia26C.png', 1),
    ('2.08 Carat Radiant Diamond', 22320, '2.08 Carat Radiant Diamond is sold exclusive', 2.08, 'VS2', 'Ideal', 'H', 'Radiant', '/Image/DiamondDTO/Diamonds/dia27A.png|/Image/DiamondDTO/Diamonds/dia27B.png|/Image/DiamondDTO/Diamonds/dia27C.png', 1),
    ('2.08 Carat Radiant Diamond', 22320, '2.08 Carat Radiant Diamond is sold exclusive', 2.08, 'IF', 'Ideal', 'H', 'Radiant', '/Image/DiamondDTO/Diamonds/dia28A.png|/Image/DiamondDTO/Diamonds/dia28B.png|/Image/DiamondDTO/Diamonds/dia28C.png', 1),
    ('1.00 Carat Radiant Diamond', 2200, '1.00 Carat Radiant Diamond is sold exclusive.', 1.00, 'VVS1', 'Ideal', 'H', 'Radiant', '/Image/DiamondDTO/Diamonds/dia29A.png|/Image/DiamondDTO/Diamonds/dia29B.png|/Image/DiamondDTO/Diamonds/dia29C.png', 1),
    ('2.08 Carat Radiant Diamond', 11190, 'This 2.08 Radiant H diamond is sold exclusively.', 2.08, 'VS1', 'Ideal', 'G', 'Radiant', '/Image/DiamondDTO/Diamonds/dia30A.png|/Image/DiamondDTO/Diamonds/dia30B.png|/Image/DiamondDTO/Diamonds/dia30C.png', 1),
	('2.51 Carat Pear Diamond', 23490, 'This 2.51 Carat Pear Diamond is sold exclusive.', 2.51, 'VS1', 'Ideal', 'D', 'Pear', '/Image/DiamondDTO/Diamonds/dia31A.png|/Image/DiamondDTO/Diamonds/dia31B.png|/Image/DiamondDTO/Diamonds/dia31C.png', 1),
    ('1.02 Carat Pear Diamond', 2680, 'This 1.02 Carat Pear Diamond is sold exclusive.', 1.02, 'IF', 'Ideal', 'D', 'Pear', '/Image/DiamondDTO/Diamonds/dia32A.png|/Image/DiamondDTO/Diamonds/dia32B.png|/Image/DiamondDTO/Diamonds/dia32C.png', 1),
    ('1.04 Carat Pear Diamond', 2720, 'This 1.04 Carat Pear Diamond is sold exclusive.', 1.04, 'IF', 'Ideal', 'E', 'Pear', '/Image/DiamondDTO/Diamonds/dia33A.png|/Image/DiamondDTO/Diamonds/dia33B.png|/Image/DiamondDTO/Diamonds/dia33C.png', 1),
    ('1.03 Carat Pear Diamond', 2730, 'This 1.03 Carat Pear Diamond is sold exclusive.', 1.03, 'VS1', 'Ideal', 'D', 'Pear', '/Image/DiamondDTO/Diamonds/dia34A.png|/Image/DiamondDTO/Diamonds/dia34B.png|/Image/DiamondDTO/Diamonds/dia34C.png', 1),
    ('1.01 Carat Pear Diamond', 2800, 'This 1.01 Carat Pear Diamond is sold exclusive.', 1.01, 'VS2', 'Ideal', 'H', 'Pear', '/Image/DiamondDTO/Diamonds/dia35A.png|/Image/DiamondDTO/Diamonds/dia35B.png|/Image/DiamondDTO/Diamonds/dia35C.png', 1),
    ('1.00 Carat Asscher Diamond', 2900, 'This 1.00 Carat Asscher Diamond is sold exclusive.', 1.00, 'VS1', 'Ideal', 'H', 'Asscher', '/Image/DiamondDTO/Diamonds/dia36A.png|/Image/DiamondDTO/Diamonds/dia36B.png|/Image/DiamondDTO/Diamonds/dia36C.png', 1),
    ('1.00 Carat Asscher Diamond', 3200, 'This 1.00 Carat Asscher Diamond is sold exclusive.', 1.00, 'VS2', 'Ideal', 'G', 'Asscher', '/Image/DiamondDTO/Diamonds/dia37A.png|/Image/DiamondDTO/Diamonds/dia37B.png|/Image/DiamondDTO/Diamonds/dia37C.png', 1),
    ('2.00 Carat Asscher Diamond', 13500, 'This 2.00 Carat Asscher Diamond is sold exclusive.', 2.00, 'VS1', 'Ideal', 'H', 'Asscher', '/Image/DiamondDTO/Diamonds/dia38A.png|/Image/DiamondDTO/Diamonds/dia38B.png|/Image/DiamondDTO/Diamonds/dia38C.png', 1),
    ('1.50 Carat Asscher Diamond', 9000, 'This 1.50 Carat Asscher Diamond is sold exclusive.', 1.50, 'VS1', 'Ideal', 'H', 'Asscher', '/Image/DiamondDTO/Diamonds/dia39A.png|/Image/DiamondDTO/Diamonds/dia39B.png|/Image/DiamondDTO/Diamonds/dia39C.png', 1),
    ('1.02 Carat Asscher Diamond', 2400, 'This 1.02 Carat Asscher Diamond is sold exclusive.', 1.02, 'VS2', 'Ideal', 'H', 'Asscher', '/Image/DiamondDTO/Diamonds/dia40A.png|/Image/DiamondDTO/Diamonds/dia40B.png|/Image/DiamondDTO/Diamonds/dia40C.png', 1),
    ('1.08 Carat Heart Diamond', 2600, 'This 1.08 Carat Heart Diamond is sold exclusive.', 1.08, 'VVS2', 'Ideal', 'E', 'Heart', '/Image/DiamondDTO/Diamonds/dia41A.png|/Image/DiamondDTO/Diamonds/dia41B.png|/Image/DiamondDTO/Diamonds/dia41C.png', 1),
    ('1.10 Carat Heart Diamond', 2700, 'This 1.10 Carat Heart Diamond is sold exclusive.', 1.10, 'VS2', 'Ideal', 'E', 'Heart', '/Image/DiamondDTO/Diamonds/dia42A.png|/Image/DiamondDTO/Diamonds/dia42B.png|/Image/DiamondDTO/Diamonds/dia42C.png', 1),
    ('1.12 Carat Heart Diamond', 2800, 'This 1.12 Carat Heart Diamond is sold exclusive.', 1.12, 'VS2', 'Ideal', 'H', 'Heart', '/Image/DiamondDTO/Diamonds/dia43A.png|/Image/DiamondDTO/Diamonds/dia43B.png|/Image/DiamondDTO/Diamonds/dia43C.png', 1),
    ('1.15 Carat Heart Diamond', 2900, 'This 1.15 Carat Heart Diamond is sold exclusive.', 1.15, 'VS2', 'Ideal', 'D', 'Heart', '/Image/DiamondDTO/Diamonds/dia44A.png|/Image/DiamondDTO/Diamonds/dia44B.png|/Image/DiamondDTO/Diamonds/dia44C.png', 1),
    ('1.18 Carat Heart Diamond', 3000, 'This 1.18 Carat Heart Diamond is sold exclusive.', 1.18, 'VS1', 'Ideal', 'H', 'Heart', '/Image/DiamondDTO/Diamonds/dia45A.png|/Image/DiamondDTO/Diamonds/dia45B.png|/Image/DiamondDTO/Diamonds/dia45C.png', 1),
    ('1.21 Carat Marquise Diamond', 3100, 'This 1.21 Carat Marquise Diamond is sold exclusive.', 1.21, 'VS2', 'Ideal', 'F', 'Marquise', '/Image/DiamondDTO/Diamonds/dia46A.png|/Image/DiamondDTO/Diamonds/dia46B.png|/Image/DiamondDTO/Diamonds/dia46C.png', 1),
    ('1.25 Carat Marquise Diamond', 3200, 'This 1.25 Carat Marquise Diamond is sold exclusive.', 1.25, 'VS2', 'Ideal', 'F', 'Marquise', '/Image/DiamondDTO/Diamonds/dia47A.png|/Image/DiamondDTO/Diamonds/dia47B.png|/Image/DiamondDTO/Diamonds/dia47C.png', 1),
    ('1.30 Carat Marquise Diamond', 3300, 'This 1.30 Carat Marquise Diamond is sold exclusive.', 1.30, 'VS1', 'Ideal', 'F', 'Marquise', '/Image/DiamondDTO/Diamonds/dia48A.png|/Image/DiamondDTO/Diamonds/dia48B.png|/Image/DiamondDTO/Diamonds/dia48C.png', 1),
    ('1.35 Carat Marquise Diamond', 3400, 'This 1.35 Carat Marquise Diamond is sold exclusive.', 1.35, 'VVS2', 'Ideal', 'D', 'Marquise', '/Image/DiamondDTO/Diamonds/dia49A.png|/Image/DiamondDTO/Diamonds/dia49B.png|/Image/DiamondDTO/Diamonds/dia49C.png', 1),
    ('1.40 Carat Marquise Diamond', 3500, 'This 1.40 Carat Marquise Diamond is sold exclusive.', 1.40, 'IF', 'Ideal', 'D', 'Marquise', '/Image/DiamondDTO/Diamonds/dia50A.png|/Image/DiamondDTO/Diamonds/dia50B.png|/Image/DiamondDTO/Diamonds/dia50C.png', 1);

INSERT INTO [dbo].[tblCertificate] ([diamondID], [certificateNumber], [issueDate], [certifyingAuthority], [cerImagePath]) VALUES 
    (1, 'CERT000001', '2024-05-26', 'GIA', '/Image/DiamondDTO/Certificates/CER01.jpg'),
    (2, 'CERT000002', '2024-05-27', 'GIA', '/Image/DiamondDTO/Certificates/CER02.jpg'),
    (3, 'CERT000003', '2024-05-28', 'GIA', '/Image/DiamondDTO/Certificates/CER03.jpg'),
    (4, 'CERT000004', '2024-05-29', 'GIA', '/Image/DiamondDTO/Certificates/CER04.jpg'),
    (5, 'CERT000005', '2024-05-30', 'GIA', '/Image/DiamondDTO/Certificates/CER05.jpg'),
    (6, 'CERT000006', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER06.jpg'),
	(7, 'CERT000007', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER07.jpg'),
	(8, 'CERT000008', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER08.jpg'),
	(9, 'CERT000009', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER09.jpg'),
	(10, 'CERT000010', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER10.jpg'),
	(11, 'CERT000011', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER11.jpg'),
	(12, 'CERT000012', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER12.jpg'),
	(13, 'CERT000013', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER13.jpg'),
	(14, 'CERT000014', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER14.jpg'),
	(15, 'CERT000015', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER15.jpg'),
	(16, 'CERT000016', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER16.jpg'),
	(17, 'CERT000017', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER17.jpg'),
	(18, 'CERT000018', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER18.jpg'),
	(19, 'CERT000019', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER19.jpg'),
	(20, 'CERT000020', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER20.jpg'),
	(21, 'CERT000021', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER21.jpg'),
	(22, 'CERT000022', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER22.jpg'),
	(23, 'CERT000023', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER23.jpg'),
	(24, 'CERT000024', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER24.jpg'),
	(25, 'CERT000025', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER25.jpg'),
	(26, 'CERT000026', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER26.jpg'),
	(27, 'CERT000027', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER27.jpg'),
	(28, 'CERT000028', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER28.jpg'),
	(29, 'CERT000029', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER29.jpg'),
	(30, 'CERT000030', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER30.jpg'),
	(31, 'CERT000031', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER31.jpg'),
	(32, 'CERT000032', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER32.jpg'),
	(33, 'CERT000033', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER33.jpg'),
	(34, 'CERT000034', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER34.jpg'),
	(35, 'CERT000035', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER35.jpg'),
	(36, 'CERT000036', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER36.jpg'),
	(37, 'CERT000037', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER37.jpg'),
	(38, 'CERT000038', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER38.jpg'),
	(39, 'CERT000039', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER39.jpg'),
	(40, 'CERT000040', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER40.jpg'),
	(41, 'CERT000041', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER41.jpg'),
	(42, 'CERT000042', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER42.jpg'),
	(43, 'CERT000043', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER43.jpg'),
	(44, 'CERT000044', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER44.jpg'),
	(45, 'CERT000045', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER45.jpg'),
	(46, 'CERT000046', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER46.jpg'),
	(47, 'CERT000047', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER47.jpg'),
	(48, 'CERT000048', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER48.jpg'),
	(49, 'CERT000049', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER49.jpg'),
	(50, 'CERT000050', '2024-04-30', 'GIA', '/Image/DiamondDTO/Certificates/CER50.jpg');
	ALTER TABLE [dbo].[tblItem]
ADD [settingSize] Int NULL;

ALTER TABLE [dbo].[tblOrder]
ADD [Note] NVARCHAR(512) NULL;