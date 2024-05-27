USE master;

CREATE DATABASE [DiamondShopManagement] 

USE [DiamondShopManagement] 
--bảng role
CREATE TABLE [dbo].[tblRole](
	[roleID] [INT] NOT NULL,
	[roleName] [NVARCHAR](20) NOT NULL,
	CONSTRAINT [PK_tblRole] PRIMARY KEY CLUSTERED (
	[roleID]  ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	) ;
--bang users
CREATE TABLE [dbo].[tblUsers](
	[userID] [NVARCHAR](50) NOT NULL,
	[userName] [NVARCHAR](50) NOT NULL UNIQUE ,
	[fullName] [NVARCHAR](50)NOT NULL,
	[email] [NVARCHAR](50) UNIQUE NOT NULL,
	[password] [NVARCHAR](50) NOT NULL,
	[roleID] [INT] NOT NULL,
	[status] [BIT] DEFAULT '1',
	[resetCode] NVARCHAR(10),
	[bonusPoint] [INT] null,
	CONSTRAINT [PK_tblUsers] PRIMARY KEY CLUSTERED (
	[userID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	CONSTRAINT [FK_tblUsers_tblRole] FOREIGN KEY ([roleID]) REFERENCES dbo.tblRole ([roleID]) ON DELETE CASCADE,
	) ;
	-- bang voucher
CREATE TABLE [dbo].[tblVoucher](
    [voucherID] [int] NOT NULL,
    [startTime] DATETIME NOT NULL,
	[endTime] DATETIME NOT NULL,
	[discount] [INT],
	[quantity] [int],
    [status] [BIT] DEFAULT '1',
    CONSTRAINT [PK_tblVoucher] PRIMARY KEY CLUSTERED 
    (
        [voucherID] ASC
    ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
);
-- bảng sở hưu voucher
CREATE TABLE [dbo].[tblVoucherCatch](
	[voucherID] [int] NOT NULL,
	[userID] [NVARCHAR](50) NOT NULL,
	CONSTRAINT [FK_tblVoucherCatch_tblUsers] FOREIGN KEY ([userID]) REFERENCES dbo.tblUsers ([userID]),
    CONSTRAINT [FK_tblVoucherCatch_tblVoucher] FOREIGN KEY ([voucherID]) REFERENCES dbo.tblVoucher([voucherID])
);
--chat 
CREATE TABLE [dbo].[tblChat](
	[chatID] INT NOT NULL,
	[chatDetail] NVARCHAR(512) NOT NULL,
	[date] DATETIME NOT NULL,
	[senderID]  NVARCHAR(50) NOT NULL,
	[receiverID]  NVARCHAR(50) NOT NULL,
	CONSTRAINT [PK_tblChat] PRIMARY KEY CLUSTERED (
	[chatID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	CONSTRAINT [FK_tblChat_tblUsers_sender] FOREIGN KEY ([senderID]) REFERENCES dbo.tblUsers ([userID]),
    CONSTRAINT [FK_tblChat_tblUsers_receiver] FOREIGN KEY ([receiverID]) REFERENCES dbo.tblUsers ([userID])
);
--bang thong bao
CREATE TABLE [dbo].[tblNotification](
	[notificationID] [NVARCHAR](50) NOT NULL,
	[userID] [NVARCHAR](50) ,
	[date] DATETIME NOT NULL,
	[detail] NVARCHAR(512) NOT NULL, 
	[status] [BIT] DEFAULT '1',
	CONSTRAINT [PK_tblNotification] PRIMARY KEY CLUSTERED (
	[notificationID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	CONSTRAINT [FK_tblNotification_tblUsers] FOREIGN KEY ([userID]) REFERENCES dbo.tblUsers ([userID]),
);
-- Tạo bảng kim cương
CREATE TABLE [dbo].[tblDiamonds](
    [diamondID] INT IDENTITY(1,1) NOT NULL,
    [diamondName] NVARCHAR(50) NOT NULL,
    [diamondPrice] MONEY NOT NULL,
    [diamondDescription] NVARCHAR(512) NOT NULL,
    [caratWeight] FLOAT NOT NULL,
    [clarityID] NVARCHAR(20) NOT NULL, -- Thêm cột clarityID
    [cutID] NVARCHAR(20) NOT NULL, -- Thêm cột cutID
    [colorID] NVARCHAR(20) NOT NULL,
    [shapeID] NVARCHAR(20) NOT NULL,
    [diamondImagePath] NVARCHAR(512) NOT NULL,
    [status] BIT DEFAULT '1',
    CONSTRAINT [PK_Diamonds] PRIMARY KEY CLUSTERED (
        [diamondID] ASC
    )
);
-- bảng chứng chỉ -- done
CREATE TABLE [dbo].[tblCertificate](
    [certificateID] INT IDENTITY(1,1) NOT NULL,
    [diamondID] INT NOT NULL,
    [certificateNumber] NVARCHAR(50) NOT NULL,
    [issueDate] DATE NOT NULL,
    [certifyingAuthority] NVARCHAR(100) NOT NULL,
	[cerImagePath] NVARCHAR(512) NOT NULL,
    CONSTRAINT [PK_tblCertificate] PRIMARY KEY CLUSTERED (
        [certificateID] ASC
    ),
    CONSTRAINT [FK_tblCertificate_tblDiamonds] FOREIGN KEY ([diamondID]) REFERENCES dbo.tblDiamonds ([diamondID]) ON DELETE CASCADE
);
-- bang comment
CREATE TABLE  [dbo].[tblComment](
	[commentID] NVARCHAR(50) NOT NULL,
    [userID] [NVARCHAR](50) NOT NULL,
    [diamondID] INT NOT NULL,
    [commond] [NVARCHAR](50),
    [rating] [INT],
    CONSTRAINT [PK_tblComment] PRIMARY KEY CLUSTERED 
    (	
		[commentID] ASC,
        [userID] ASC,
        [diamondID] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    CONSTRAINT [FK_tblComment_tblUsers] FOREIGN KEY ([userID]) REFERENCES dbo.tblUsers ([userID]) ON DELETE CASCADE,
    CONSTRAINT [FK_tblComment_tblDiamonds] FOREIGN KEY ([diamondID]) REFERENCES dbo.tblDiamonds ([diamondID]) ON DELETE CASCADE
	);
-- bang order
CREATE TABLE  [dbo].[tblOrder](
	[orderID] [NVARCHAR](50) NOT NULL,
    [customerID] [NVARCHAR](50) NOT NULL,
	[deliveryStaffID] [NVARCHAR](50) NOT NULL,
    [totalMoney] [FLOAT] NOT NULL,
	[status] [BIT] DEFAULT '1',
	[address] [NVARCHAR](100) NOT NULL,
	[phone] [NVARCHAR](50) NOT NULL,
	[saleDate] DATETIME NOT NULL,
    CONSTRAINT [PK_tblOrder] PRIMARY KEY CLUSTERED 
    (
        [orderID] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    CONSTRAINT [FK_tblOrder_tblUsers_1] FOREIGN KEY ([customerID]) REFERENCES dbo.tblUsers ([userID]),
    CONSTRAINT [FK_tblOrder_tblUsers_2] FOREIGN KEY ([deliveryStaffID]) REFERENCES dbo.tblUsers ([userID])
	);
--Bảng transition -- Done
CREATE TABLE [dbo].[tblTransaction](
    [transactionID] INT IDENTITY(1,1) NOT NULL,
    [orderID] NVARCHAR(50) NULL,
    [userID] NVARCHAR(50) NOT NULL,
    [transactionType] NVARCHAR(50) NOT NULL,
    [transactionDate] DATETIME NOT NULL,
    CONSTRAINT [PK_tblTransaction] PRIMARY KEY CLUSTERED (
        [transactionID] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY],
    CONSTRAINT [FK_tblTransaction_tblOrder] FOREIGN KEY ([orderID]) REFERENCES dbo.tblOrder ([orderID]) ON DELETE CASCADE,
    CONSTRAINT [FK_tblTransaction_tblUsers] FOREIGN KEY ([userID]) REFERENCES dbo.tblUsers ([userID]) ON DELETE CASCADE,
	CONSTRAINT [UQ_tblTransaction_tblOrder] UNIQUE ([orderID])
);
-- bang OrderItem -- Done
CREATE TABLE [dbo].[tblOrderItem](
	[diamondID] int,
	[orderID] [NVARCHAR](50) NOT NULL,
	[salePrice] [MONEY] NOT NULL,
	CONSTRAINT [PK_tblOrderItem] PRIMARY KEY CLUSTERED 
    (
        [orderID] ASC,
		[diamondID] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_tblOrderItem_tblOrder] FOREIGN KEY ([orderID]) REFERENCES  dbo.tblOrder ([orderID]) ON DELETE CASCADE,
	CONSTRAINT [FK_tblOrderItem_tblDiamond] FOREIGN KEY ([diamondID]) REFERENCES  dbo.tblDiamonds([diamondID])
);
-- bảng phiếu bảo hành  --Done
CREATE TABLE [dbo].[tblWarranty](
    [warrantyID] INT IDENTITY(1,1) NOT NULL,
    [orderID] NVARCHAR(50) NOT NULL,
    [diamondID] INT NOT NULL,
    [warrantyStartDate] DATETIME NOT NULL,
    [warrantyEndDate] DATETIME NOT NULL,
    [warrantyDetails] NVARCHAR(512) NOT NULL,
    CONSTRAINT [PK_tblWarranty] PRIMARY KEY CLUSTERED (
        [warrantyID] ASC
    ) WITH (
        PAD_INDEX  = OFF, 
        STATISTICS_NORECOMPUTE  = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS  = ON, 
        ALLOW_PAGE_LOCKS  = ON
    ) ON [PRIMARY],
    CONSTRAINT [FK_tblWarranty_tblOrderItem] FOREIGN KEY ([orderID], [diamondID]) REFERENCES dbo.tblOrderItem ([orderID], [diamondID]) ON DELETE CASCADE,
    CONSTRAINT [UQ_tblWarranty_tblOrderItem] UNIQUE ([orderID], [diamondID])
);


--Insert Role
INSERT INTO [dbo].tblRole([roleID],[roleName]) VALUES 
( 1 , 'user'),
( 2 , 'admin'),
( 3 , 'manager'),
( 4 , 'deliverystaff'),
( 5 , 'salestaff');
--Insert Shape

INSERT INTO [dbo].[tblDiamonds] ([diamondName], [diamondPrice], [diamondDescription], [caratWeight],[clarityID], [cutID],  [colorID], [shapeID],[diamondImagePath],[status]) VALUES 
('1.01 Carat Round Diamond',4080,'This 1.01 round H diamond is sold exclusively.', 1.01,'VS2','Excellent','H', 'Round','~/Image/diamond/dia1A.jpg|~/Image/DiamondDTO/Diamonds/dia1B.png|~/Image/DiamondDTO/Diamonds/dia1C.png',1 ),
('1.00 Carat Princess Diamond',2400,'This 1.00 Carat Princess Diamond is exclusively',1.00,'VS2','Ideal','H', 'Princess','~/Image/diamond/dia2A.jpg|~/Image/DiamondDTO/Diamonds/dia2B.png|~/Image/DiamondDTO/Diamonds/dia2C.png',1),
('2.08 Carat Cushion Modified Diamond',11190,'This 2.08 cushion modified H diamond is sold exclusively.',2.08,'VS1','Ideal','H', 'Cushion','~/Image/diamond/dia3A.jpg|~/Image/DiamondDTO/Diamonds/dia3B.png|~/Image/DiamondDTO/Diamonds/dia3C.png',1),
('2.51 Carat Emerald Diamond',23490,'This 2.51 Carat Emerald Diamond is sold exclusive. ',2.51,'VS1','Ideal','H', 'Emerald','~/Image/diamond/dia4A.jpg|~/Image/DiamondDTO/Diamonds/dia4B.png|~/Image/DiamondDTO/Diamonds/dia4C.png',1),
('2.50 Carat Oval Diamond',24630,'2.50 Carat Oval Diamond is sold exclusive.',2.50,'VS2','Ideal','G', 'Oval','~/Image/diamond/dia5A.jpg|~/Image/DiamondDTO/Diamonds/dia5B.png|~/Image/DiamondDTO/Diamonds/dia5C.png',1),
('1.81 Carat Radiant Diamond',12480,'1.81 Carat Radiant Diamond is sold eclusive',1.81,'VS2','Ideal','H', 'Radiant','~/Image/diamond/dia6A.jpg|~/Image/DiamondDTO/Diamonds/dia6B.png|~/Image/DiamondDTO/Diamonds/dia6C.png',1)
;
INSERT INTO [dbo].[tblCertificate] ([diamondID], [certificateNumber], [issueDate], [certifyingAuthority], [cerImagePath]) VALUES 
    (1, 'CERT000001', '2024-05-26', 'GIA', '~/Image/DiamondDTO/Certificates/CER01.jpg'),
    (2, 'CERT000002', '2024-05-27', 'GIA', '~/Image/DiamondDTO/Certificates/CER01.jpg'),
    (3, 'CERT000003', '2024-05-28', 'GIA', '~/Image/DiamondDTO/Certificates/CER01.jpg'),
    (4, 'CERT000004', '2024-05-29', 'GIA', '~/Image/DiamondDTO/Certificates/CER01.jpg'),
    (5, 'CERT000005', '2024-05-30', 'GIA', '~/Image/DiamondDTO/Certificates/CER01.jpg'),
    (6, 'CERT000006', '2024-04-30', 'GIA', '~/Image/DiamondDTO/Certificates/CER01.jpg');