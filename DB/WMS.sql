USE [master]
GO
/****** Object:  Database [WMS]    Script Date: 1/31/2019 1:25:09 PM ******/
CREATE DATABASE [WMS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'WMS', FILENAME = N'D:\Database\SQL Server 2014 Express\MSSQL12.ISPLSQLSERVER\MSSQL\DATA\WMS.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'WMS_log', FILENAME = N'D:\Database\SQL Server 2014 Express\MSSQL12.ISPLSQLSERVER\MSSQL\DATA\WMS_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [WMS] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [WMS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [WMS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [WMS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [WMS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [WMS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [WMS] SET ARITHABORT OFF 
GO
ALTER DATABASE [WMS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [WMS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [WMS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [WMS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [WMS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [WMS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [WMS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [WMS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [WMS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [WMS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [WMS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [WMS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [WMS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [WMS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [WMS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [WMS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [WMS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [WMS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [WMS] SET  MULTI_USER 
GO
ALTER DATABASE [WMS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [WMS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [WMS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [WMS] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [WMS] SET DELAYED_DURABILITY = DISABLED 
GO
USE [WMS]
GO
/****** Object:  Table [dbo].[Block]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Block](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WareHouseId] [int] NOT NULL,
	[Name] [varchar](10) NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_Block] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Country]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Country](
	[CountryId] [int] IDENTITY(1,1) NOT NULL,
	[CountryName] [varchar](100) NOT NULL,
	[CountryCode] [varchar](50) NOT NULL,
	[CountryPhoneCode] [varchar](20) NOT NULL,
	[TimeZoneId] [int] NULL,
	[CountryCode2] [varchar](50) NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ModuleScreen]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ModuleScreen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](30) NOT NULL,
	[DisplayName] [varchar](30) NULL,
	[Class] [varchar](80) NULL,
	[OrderNumber] [int] NULL,
 CONSTRAINT [PK_ModuleScreen_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ModuleScreenDetail]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ModuleScreenDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleScreenId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Class] [varchar](80) NULL,
	[State] [varchar](50) NULL,
	[ModuleScreenClass] [varchar](80) NULL,
	[OrderNumber] [int] NULL,
 CONSTRAINT [PK_ModuleScreenDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Product]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StockRangeId] [int] NOT NULL,
	[ProductName] [varchar](50) NOT NULL,
	[ProductCode] [varchar](5) NOT NULL,
	[SKUCode] [varchar](60) NOT NULL,
	[Type] [varchar](20) NULL,
	[Color] [varchar](20) NULL,
	[Size] [varchar](20) NOT NULL,
	[ProductDescription] [varchar](max) NULL,
	[WeightUnit] [varchar](20) NULL,
	[Quantity] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedBy] [varchar](50) NULL,
	[UpdatedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Rack]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Rack](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BlockId] [int] NOT NULL,
	[Name] [varchar](10) NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_Rack] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RackRowDetail]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RackRowDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BlockId] [int] NOT NULL,
	[RackId] [int] NOT NULL,
 CONSTRAINT [PK_RackRowDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RackRowSection]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RackRowSection](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WareHouseId] [int] NOT NULL,
	[BockId] [int] NOT NULL,
	[RackId] [int] NOT NULL,
	[RowRackId] [int] NOT NULL,
	[Name] [varchar](10) NOT NULL,
	[Description] [varchar](50) NULL,
	[BarCode] [varchar](30) NOT NULL,
 CONSTRAINT [PK_RackRowSection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Timezone]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Timezone](
	[TimezoneId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Offset] [varchar](200) NOT NULL,
	[OffsetShort] [varchar](20) NULL,
 CONSTRAINT [PK_Timezone] PRIMARY KEY CLUSTERED 
(
	[TimezoneId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserConfiguration]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserConfiguration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[MailHostName] [varchar](50) NOT NULL,
	[SMTPport] [varchar](10) NOT NULL,
	[SMTPUserName] [varchar](1000) NOT NULL,
	[SMTPPassword] [varchar](500) NOT NULL,
	[SMTPDisplayName] [varchar](500) NOT NULL,
	[FromMail] [varchar](500) NOT NULL,
	[EnableSsl] [varchar](3) NOT NULL,
 CONSTRAINT [PK_UserConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserModuleScreen]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserModuleScreen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ModuleScreenId] [int] NOT NULL,
	[ModuleScreenDetailId] [int] NOT NULL,
	[IsAllowed] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdateBy] [varchar](50) NULL,
 CONSTRAINT [PK_UserModuleScreen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Warehouse]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Warehouse](
	[WarehouseId] [int] IDENTITY(1,1) NOT NULL,
	[Address] [varchar](200) NOT NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[City] [varchar](50) NOT NULL,
	[State] [varchar](50) NULL,
	[Zip] [varchar](50) NULL,
	[CountryId] [int] NOT NULL,
	[LocationName] [varchar](200) NULL,
	[LocationLatitude] [decimal](18, 14) NULL,
	[LocationLongitude] [decimal](18, 14) NULL,
	[LocationZoom] [int] NULL,
	[MarkerLatitude] [decimal](18, 14) NULL,
	[MarkerLongitude] [decimal](18, 14) NULL,
	[LocationMapImage] [varchar](200) NULL,
	[ManagerId] [int] NULL,
	[Email] [varchar](50) NULL,
	[TelephoneNo] [varchar](50) NOT NULL,
	[MobileNo] [varchar](20) NULL,
	[Fax] [varchar](50) NOT NULL,
	[WorkingStartTime] [time](7) NOT NULL,
	[WorkingEndTime] [time](7) NOT NULL,
	[TimeZoneId] [int] NOT NULL,
	[WorkingWeekDayId] [int] NULL,
 CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED 
(
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WMSRole]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WMSRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[DisplayName] [varchar](500) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WMSUser]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WMSUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ActivationToken] [nvarchar](max) NULL,
	[PasswordAnswer] [nvarchar](max) NULL,
	[PasswordQuestion] [nvarchar](max) NULL,
	[CompanyName] [varchar](100) NOT NULL,
	[ContactFirstName] [varchar](100) NOT NULL,
	[ContactLastName] [varchar](100) NULL,
	[Email] [varchar](100) NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[TelephoneNo] [varchar](20) NULL,
	[MobileNo] [varchar](20) NULL,
	[FaxNumber] [varchar](50) NULL,
	[TimezoneId] [int] NOT NULL,
	[ShortName] [varchar](50) NULL,
	[Position] [varchar](100) NULL,
	[Skype] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_User_IsActive]  DEFAULT ((1)),
	[CreatedOnUtc] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedOnUtc] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[ProfileImage] [varchar](300) NULL,
	[LastLoginOnUtc] [datetime] NULL,
	[LastPasswordChangedOnUtc] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WMSUserAdditional]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WMSUserAdditional](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ManageUserId] [int] NULL,
	[PaymentCurrency] [varchar](3) NOT NULL,
	[PaymentTerm] [varchar](50) NULL,
	[CreditLimit] [decimal](6, 2) NOT NULL,
	[SalesUserId] [int] NOT NULL,
	[SalesCoOrdinatorId] [int] NOT NULL,
	[MerchandiseUserId] [int] NOT NULL,
	[WarehouseuserId] [int] NOT NULL,
 CONSTRAINT [PK_UserAdditional] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WMSUserAddress]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WMSUserAddress](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Address] [varchar](200) NOT NULL,
	[Address2] [varchar](200) NULL,
	[Address3] [varchar](200) NULL,
	[Suburb] [varchar](50) NULL,
	[City] [varchar](50) NOT NULL,
	[State] [varchar](50) NULL,
	[PostCode] [varchar](20) NULL,
	[CountryId] [int] NOT NULL,
 CONSTRAINT [PK_UserAddress] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WMSUserClaim]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WMSUserClaim](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_Claim] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WMSUserLogin]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WMSUserLogin](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [int] NOT NULL,
	[UserLoginId] [int] NULL,
	[IsActive] [bit] NULL,
	[IsLocked] [bit] NOT NULL,
 CONSTRAINT [PK_UserLogin_1] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WMSUserRole]    Script Date: 1/31/2019 1:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WMSUserRole](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Block] ON 

GO
INSERT [dbo].[Block] ([Id], [WareHouseId], [Name], [Description]) VALUES (1, 2, N'TestBlock', N'Test Block ')
GO
SET IDENTITY_INSERT [dbo].[Block] OFF
GO
SET IDENTITY_INSERT [dbo].[Country] ON 

GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (1, N'Afghanistan', N'AFG', N'93', 24, N'AF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (2, N'Albania', N'ALB', N'355', 37, N'AL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (3, N'Algeria', N'DZA', N'213', 40, N'DZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (4, N'American Samoa', N'ASM', N'1-684', 165, N'AS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (5, N'Andorra', N'AND', N'376', 36, N'AD')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (6, N'Angola', N'AGO', N'244', 40, N'AO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (7, N'Anguilla', N'AIA', N'1-264', 21, N'AI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (8, N'Antigua and Barbuda', N'ATG', N'1-268', 21, N'AG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (9, N'Argentina', N'ARG', N'54', 24, N'AR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (10, N'Armenia', N'ARM', N'374', 65, N'AM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (11, N'Aruba', N'ABW', N'297', 21, N'AW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (12, N'Australia', N'AUS', N'61', 91, N'AU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (13, N'Austria', N'AUT', N'43', 36, N'AT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (14, N'Azerbaijan', N'AZE', N'994', 61, N'AZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (15, N'Bahamas', N'BHS', N'1-242', 15, N'BS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (16, N'Bahrain', N'BHR', N'973', 55, N'BH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (17, N'Bangladesh', N'BGD', N'880', 74, N'BD')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (18, N'Barbados', N'BRB', N'1-246', 21, N'BB')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (19, N'Belarus', N'BLR', N'375', 56, N'BY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (20, N'Belgium', N'BEL', N'32', 38, N'BE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (21, N'Belize', N'BLZ', N'501', 9, N'BZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (22, N'Benin', N'BEN', N'229', 40, N'BJ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (23, N'Bermuda', N'BMU', N'1-441', 19, N'BM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (24, N'Bhutan', N'BTN', N'975', 74, N'BT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (25, N'Bolivia', N'BOL', N'591', 21, N'BO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (26, N'Bosnia and Herzegovina', N'BIH', N'387', 39, N'BA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (27, N'Botswana', N'BWA', N'267', 48, N'BW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (28, N'Brazil', N'BRA', N'55', 23, N'BR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (29, N'British Indian Ocean Territory', N'IOT', N'246', 73, N'IO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (30, N'British Virgin Islands', N'VGB', N'1-284', 19, N'VG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (31, N'Brunei', N'BRN', N'673', 81, N'BN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (32, N'Bulgaria', N'BGR', N'359', 49, N'BG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (33, N'Burkina Faso', N'BFA', N'226', 35, N'BF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (34, N'Burundi', N'BDI', N'257', 48, N'BI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (35, N'Cambodia', N'KHM', N'855', 77, N'KH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (36, N'Cameroon', N'CMR', N'237', 40, N'CM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (37, N'Canada', N'CAN', N'1', 15, N'CA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (38, N'Cape Verde', N'CPV', N'238', 32, N'CV')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (39, N'Cayman Islands', N'CYM', N'1-345', 13, N'KY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (40, N'Central African Republic', N'CAF', N'236', 40, N'CF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (41, N'Chad', N'TCD', N'235', 40, N'TD')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (42, N'Chile', N'CHL', N'56', 29, N'CL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (43, N'China', N'CHN', N'86', 79, N'CN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (44, N'Christmas Island', N'CXR', N'61', 77, N'CX')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (45, N'Cocos Islands', N'CCK', N'61', 76, N'CC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (46, N'Colombia', N'COL', N'57', 13, N'CO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (47, N'Comoros', N'COM', N'269', 58, N'KM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (48, N'Cook Islands', N'COK', N'682', 2, N'CK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (49, N'Costa Rica', N'CRI', N'506', 9, N'CR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (50, N'Croatia', N'HRV', N'385', 39, N'HR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (51, N'Cuba', N'CUB', N'53', 15, N'CU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (52, N'Curacao', N'CUW', N'599', 21, N'CW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (53, N'Cyprus', N'CYP', N'357', 47, N'CY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (54, N'Czech Republic', N'CZE', N'420', 37, N'CZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (55, N'Democratic Republic of the Congo', N'COD', N'243', 122, N'CD')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (56, N'Denmark', N'DNK', N'45', 38, N'DK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (57, N'Djibouti', N'DJI', N'253', 58, N'DJ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (58, N'Dominica', N'DMA', N'1-767', 21, N'DM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (59, N'Dominican Republic', N'DOM', N'1-849', 21, N'DO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (60, N'East Timor', N'TLS', N'670', 100, N'TL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (61, N'Ecuador', N'ECU', N'593', 13, N'EC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (62, N'Egypt', N'EGY', N'20', 45, N'EG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (63, N'El Salvador', N'SLV', N'503', 9, N'SV')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (64, N'Equatorial Guinea', N'GNQ', N'240', 40, N'GQ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (65, N'Eritrea', N'ERI', N'291', 58, N'ER')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (66, N'Estonia', N'EST', N'372', 49, N'EE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (67, N'Ethiopia', N'ETH', N'251', 58, N'ET')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (68, N'Falkland Islands', N'FLK', N'500', 25, N'FK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (69, N'Faroe Islands', N'FRO', N'298', 34, N'FO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (70, N'Fiji', N'FJI', N'679', 100, N'FJ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (71, N'Finland', N'FIN', N'358', 49, N'FI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (72, N'France', N'FRA', N'33', 38, N'FR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (73, N'French Polynesia', N'PYF', N'689', 2, N'PF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (74, N'Gabon', N'GAB', N'241', 40, N'GA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (75, N'Gambia', N'GMB', N'220', 35, N'GM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (76, N'Georgia', N'GEO', N'995', 64, N'GE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (77, N'Germany', N'DEU', N'49', 36, N'DE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (78, N'Ghana', N'GHA', N'233', 35, N'GH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (79, N'Gibraltar', N'GIB', N'350', 36, N'GI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (80, N'Greece', N'GRC', N'30', 43, N'GR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (81, N'Greenland', N'GRL', N'299', 127, N'GL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (82, N'Grenada', N'GRD', N'1-473', 21, N'GD')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (83, N'Guam', N'GUM', N'1-671', 92, N'GU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (84, N'Guatemala', N'GTM', N'502', 9, N'GT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (85, N'Guernsey', N'GGY', N'44-1481', 34, N'GG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (86, N'Guinea', N'GIN', N'224', 35, N'GN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (87, N'Guinea-Bissau', N'GNB', N'245', 21, N'GW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (88, N'Guyana', N'GUY', N'592', 21, N'GY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (89, N'Haiti', N'HTI', N'509', 15, N'HT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (90, N'Honduras', N'HND', N'504', 9, N'HN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (91, N'Hong Kong', N'HKG', N'852', 79, N'HK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (92, N'Hungary', N'HUN', N'36', 37, N'HU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (93, N'Iceland', N'ISL', N'354', 35, N'IS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (94, N'India', N'IND', N'91', 70, N'IN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (95, N'Indonesia', N'IDN', N'62', 77, N'ID')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (96, N'Iran', N'IRN', N'98', 59, N'IR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (97, N'Iraq', N'IRQ', N'964', 54, N'IQ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (98, N'Ireland', N'IRL', N'353', 34, N'IE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (99, N'Isle of Man', N'IMN', N'44-1624', 38, N'IM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (100, N'Israel', N'ISR', N'972', 51, N'IL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (101, N'Italy', N'ITA', N'39', 36, N'IT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (102, N'Ivory Coast', N'CIV', N'225', 35, N'CI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (103, N'Jamaica', N'JAM', N'1-876', 13, N'JM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (104, N'Japan', N'JPN', N'81', 85, N'JP')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (105, N'Jersey', N'JEY', N'44-1534', 34, N'JE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (106, N'Jordan', N'JOR', N'962', 42, N'JO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (107, N'Kazakhstan', N'KAZ', N'7', 131, N'KZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (108, N'Kenya', N'KEN', N'254', 58, N'KE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (109, N'Kiribati', N'KIR', N'686', 166, N'KI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (110, N'Kosovo', N'XKX', N'383', 39, N'XK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (111, N'Kuwait', N'KWT', N'965', 55, N'KW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (112, N'Kyrgyzstan', N'KGZ', N'996', 73, N'KG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (113, N'Laos', N'LAO', N'856', 77, N'LA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (114, N'Latvia', N'LVA', N'371', 49, N'LV')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (115, N'Lebanon', N'LBN', N'961', 44, N'LB')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (116, N'Lesotho', N'LSO', N'266', 48, N'LS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (117, N'Liberia', N'LBR', N'231', 35, N'LR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (118, N'Libya', N'LBY', N'218', 47, N'LY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (119, N'Liechtenstein', N'LIE', N'423', 36, N'LI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (120, N'Lithuania', N'LTU', N'370', 49, N'LT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (121, N'Luxembourg', N'LUX', N'352', 36, N'LU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (122, N'Macau', N'MAC', N'853', 79, N'MO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (123, N'Macedonia', N'MKD', N'389', 39, N'MK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (124, N'Madagascar', N'MDG', N'261', 58, N'MG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (125, N'Malawi', N'MWI', N'265', 48, N'MW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (126, N'Malaysia', N'MYS', N'60', 81, N'MY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (127, N'Maldives', N'MDV', N'960', 67, N'MV')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (128, N'Mali', N'MLI', N'223', 35, N'ML')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (129, N'Malta', N'MLT', N'356', 36, N'MT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (130, N'Marshall Islands', N'MHL', N'692', 166, N'MH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (131, N'Mauritania', N'MRT', N'222', 35, N'MR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (132, N'Mauritius', N'MUS', N'230', 63, N'MU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (133, N'Mayotte', N'MYT', N'262', 58, N'YT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (134, N'Mexico', N'MEX', N'52', 11, N'MX')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (135, N'Micronesia', N'FSM', N'691', 92, N'FM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (136, N'Moldova', N'MDA', N'373', 43, N'MD')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (137, N'Monaco', N'MCO', N'377', 36, N'MC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (138, N'Mongolia', N'MNG', N'976', 84, N'MN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (139, N'Montenegro', N'MNE', N'382', 39, N'ME')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (140, N'Montserrat', N'MSR', N'1-664', 21, N'MS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (141, N'Morocco', N'MAR', N'212', 33, N'MA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (142, N'Mozambique', N'MOZ', N'258', 48, N'MZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (143, N'Myanmar', N'MMR', N'95', 76, N'MM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (144, N'Namibia', N'NAM', N'264', 41, N'NA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (145, N'Nauru', N'NRU', N'674', 166, N'NR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (146, N'Nepal', N'NPL', N'977', 72, N'NP')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (147, N'Netherlands', N'NLD', N'31', 36, N'NL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (148, N'Netherlands Antilles', N'ANT', N'599', 19, N'AN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (149, N'New Caledonia', N'NCL', N'687', 97, N'NC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (150, N'New Zealand', N'NZL', N'64', 99, N'NZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (151, N'Nicaragua', N'NIC', N'505', 9, N'NI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (152, N'Niger', N'NER', N'227', 40, N'NE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (153, N'Nigeria', N'NGA', N'234', 40, N'NG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (154, N'Niue', N'NIU', N'683', 165, N'NU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (155, N'North Korea', N'PRK', N'850', 86, N'KP')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (157, N'Northern Mariana Islands', N'MNP', N'1-670', 92, N'MP')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (158, N'Norway', N'NOR', N'47', 36, N'NO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (159, N'Oman', N'OMN', N'968', 60, N'OM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (160, N'Pakistan', N'PAK', N'92', 69, N'PK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (161, N'Palau', N'PLW', N'680', 85, N'PW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (162, N'Palestine', N'PSE', N'970', 45, N'PS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (163, N'Panama', N'PAN', N'507', 13, N'PA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (164, N'Papua New Guinea', N'PNG', N'675', 92, N'PG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (165, N'Paraguay', N'PRY', N'595', 18, N'PY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (166, N'Peru', N'PER', N'51', 13, N'PE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (167, N'Philippines', N'PHL', N'63', 81, N'PH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (168, N'Pitcairn', N'PCN', N'64', 5, N'PN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (169, N'Poland', N'POL', N'48', 39, N'PL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (170, N'Portugal', N'PRT', N'351', 34, N'PT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (171, N'Puerto Rico', N'PRI', N'1-939', 21, N'PR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (172, N'Qatar', N'QAT', N'974', 55, N'QA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (173, N'Republic of the Congo', N'COG', N'242', 122, N'CG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (174, N'Reunion', N'REU', N'262', 63, N'RE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (175, N'Romania', N'ROU', N'40', 43, N'RO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (176, N'Russia', N'RUS', N'7', 57, N'RU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (177, N'Rwanda', N'RWA', N'250', 48, N'RW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (178, N'Saint Barthelemy', N'BLM', N'590', 21, N'BL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (179, N'Saint Helena', N'SHN', N'290', 35, N'SH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (180, N'Saint Kitts and Nevis', N'KNA', N'1-869', 21, N'KN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (181, N'Saint Lucia', N'LCA', N'1-758', 21, N'LC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (182, N'Saint Martin', N'MAF', N'590', 21, N'MF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (183, N'Saint Pierre and Miquelon', N'SPM', N'508', 26, N'PM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (184, N'Saint Vincent and the Grenadines', N'VCT', N'1-784', 21, N'VC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (185, N'Samoa', N'WSM', N'685', 103, N'WS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (186, N'San Marino', N'SMR', N'378', 36, N'SM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (187, N'Sao Tome and Principe', N'STP', N'239', 35, N'ST')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (188, N'Saudi Arabia', N'SAU', N'966', 55, N'SA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (189, N'Senegal', N'SEN', N'221', 35, N'SN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (190, N'Serbia', N'SRB', N'381', 37, N'RS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (191, N'Seychelles', N'SYC', N'248', 63, N'SC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (192, N'Sierra Leone', N'SLE', N'232', 35, N'SL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (193, N'Singapore', N'SGP', N'65', 81, N'SG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (194, N'Sint Maarten', N'SXM', N'1-721', 21, N'SX')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (195, N'Slovakia', N'SVK', N'421', 37, N'SK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (196, N'Slovenia', N'SVN', N'386', 37, N'SI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (197, N'Solomon Islands', N'SLB', N'677', 97, N'SB')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (198, N'Somalia', N'SOM', N'252', 58, N'SO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (199, N'South Africa', N'ZAF', N'27', 48, N'ZA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (200, N'South Korea', N'KOR', N'82', 86, N'KR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (201, N'South Sudan', N'SSD', N'211', 58, N'SS')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (202, N'Spain', N'ESP', N'34', 38, N'ES')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (203, N'Sri Lanka', N'LKA', N'94', 71, N'LK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (204, N'Sudan', N'SDN', N'249', 58, N'SD')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (205, N'Suriname', N'SUR', N'597', 25, N'SR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (206, N'Svalbard and Jan Mayen', N'SJM', N'47', 36, N'SJ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (207, N'Swaziland', N'SWZ', N'268', 48, N'SZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (208, N'Sweden', N'SWE', N'46', 36, N'SE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (209, N'Switzerland', N'CHE', N'41', 36, N'CH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (210, N'Syria', N'SYR', N'963', 46, N'SY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (211, N'Taiwan', N'TWN', N'886', 83, N'TW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (212, N'Tajikistan', N'TJK', N'992', 67, N'TJ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (213, N'Tanzania', N'TZA', N'255', 58, N'TZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (214, N'Thailand', N'THA', N'66', 77, N'TH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (215, N'Togo', N'TGO', N'228', 35, N'TG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (216, N'Tokelau', N'TKL', N'690', 102, N'TK')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (217, N'Tonga', N'TON', N'676', 102, N'TO')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (218, N'Trinidad and Tobago', N'TTO', N'1-868', 21, N'TT')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (219, N'Tunisia', N'TUN', N'216', 40, N'TN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (220, N'Turkey', N'TUR', N'90', 50, N'TR')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (221, N'Turkmenistan', N'TKM', N'993', 67, N'TM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (222, N'Turks and Caicos Islands', N'TCA', N'1-649', 15, N'TC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (223, N'Tuvalu', N'TUV', N'688', 166, N'TV')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (224, N'U.S. Virgin Islands', N'VIR', N'1-340', 19, N'VI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (225, N'Uganda', N'UGA', N'256', 58, N'UG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (226, N'Ukraine', N'UKR', N'380', 49, N'UA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (227, N'United Arab Emirates', N'ARE', N'971', 60, N'AE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (228, N'United Kingdom', N'GBR', N'44', 34, N'GB')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (229, N'United States', N'USA', N'1', 5, N'US')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (230, N'Uruguay', N'URY', N'598', 27, N'UY')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (231, N'Uzbekistan', N'UZB', N'998', 67, N'UZ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (232, N'Vanuatu', N'VUT', N'678', 97, N'VU')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (233, N'Vatican', N'VAT', N'379', 36, N'VA')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (234, N'Venezuela', N'VEN', N'58', 17, N'VE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (235, N'Vietnam', N'VNM', N'84', 77, N'VN')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (236, N'Wallis and Futuna', N'WLF', N'681', 166, N'WF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (237, N'Western Sahara', N'ESH', N'212', 139, N'EH')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (238, N'Yemen', N'YEM', N'967', 55, N'YE')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (239, N'Zambia', N'ZMB', N'260', 48, N'ZM')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (240, N'Zimbabwe', N'ZWE', N'263', 48, N'ZW')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (241, N'Tahiti', N'PYF', N'689', 126, N'PF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (242, N'Canary Islands', N'ICC', N'34', 139, N'IC')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (243, N'Norfolk Island', N'NFK', N'672', 97, N'NF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (244, N'French Gyuana', N'GUF', N'594', 25, N'GF')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (245, N'Bonaire/St Eustatius', N'BQSE', N'599', 21, N'BQ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (246, N'Cote Divoire', N'CIV', N'225', 35, N'CI')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (247, N'Congo (Brazzaville)', N'COG', N'242', 122, N'CG')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (248, N'Guadeloupe', N'GLP', N'590', 21, N'GP')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (249, N'Martinique', N'MTQ', N'595', 21, N'MQ')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (250, N'Timor-Leste', N'TLS', N'670', 100, N'TL')
GO
INSERT [dbo].[Country] ([CountryId], [CountryName], [CountryCode], [CountryPhoneCode], [TimeZoneId], [CountryCode2]) VALUES (251, N'Somaliland Rep. Of', N'SOM', N'252', 58, N'SO')
GO
SET IDENTITY_INSERT [dbo].[Country] OFF
GO
SET IDENTITY_INSERT [dbo].[ModuleScreen] ON 

GO
INSERT [dbo].[ModuleScreen] ([Id], [Name], [DisplayName], [Class], [OrderNumber]) VALUES (1, N'DYOITEMS', N'DYO Items', N'fa fa fa-building-o fa-5x', 1)
GO
INSERT [dbo].[ModuleScreen] ([Id], [Name], [DisplayName], [Class], [OrderNumber]) VALUES (2, N'CUSTOMER', N'Customer', N'fa fa fa-user fa-5x', 2)
GO
INSERT [dbo].[ModuleScreen] ([Id], [Name], [DisplayName], [Class], [OrderNumber]) VALUES (3, N'USER', N'User', N'fa fa fa-files-o fa-5x', 3)
GO
INSERT [dbo].[ModuleScreen] ([Id], [Name], [DisplayName], [Class], [OrderNumber]) VALUES (4, N'ACCESS CONTROL', N'Access Control', N'fa fa fa-file-text-o fa-5x', 4)
GO
INSERT [dbo].[ModuleScreen] ([Id], [Name], [DisplayName], [Class], [OrderNumber]) VALUES (5, N'PRODUCT', N'Product', N'fa fa fa-calendar-check-o fa-5x', 5)
GO
INSERT [dbo].[ModuleScreen] ([Id], [Name], [DisplayName], [Class], [OrderNumber]) VALUES (6, N'RACK', N'Rack', N'fa fa fa-wpforms fa-5x', 6)
GO
INSERT [dbo].[ModuleScreen] ([Id], [Name], [DisplayName], [Class], [OrderNumber]) VALUES (7, N'WAREHOUSE', N'WareHouse', N'fa fa fa-search fa-5x', 7)
GO
INSERT [dbo].[ModuleScreen] ([Id], [Name], [DisplayName], [Class], [OrderNumber]) VALUES (8, N'ORDER', N'Order', N'fa fa fa-pie-chart fa-5x', 8)
GO
SET IDENTITY_INSERT [dbo].[ModuleScreen] OFF
GO
SET IDENTITY_INSERT [dbo].[ModuleScreenDetail] ON 

GO
INSERT [dbo].[ModuleScreenDetail] ([Id], [ModuleScreenId], [Name], [Class], [State], [ModuleScreenClass], [OrderNumber]) VALUES (1, 2, N'View Customer', NULL, NULL, NULL, 1)
GO
INSERT [dbo].[ModuleScreenDetail] ([Id], [ModuleScreenId], [Name], [Class], [State], [ModuleScreenClass], [OrderNumber]) VALUES (2, 2, N'Add Customer', NULL, NULL, NULL, 2)
GO
SET IDENTITY_INSERT [dbo].[ModuleScreenDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[RackRowDetail] ON 

GO
INSERT [dbo].[RackRowDetail] ([Id], [BlockId], [RackId]) VALUES (1, 1, 0)
GO
INSERT [dbo].[RackRowDetail] ([Id], [BlockId], [RackId]) VALUES (2, 1, 0)
GO
INSERT [dbo].[RackRowDetail] ([Id], [BlockId], [RackId]) VALUES (3, 1, 0)
GO
INSERT [dbo].[RackRowDetail] ([Id], [BlockId], [RackId]) VALUES (4, 1, 0)
GO
INSERT [dbo].[RackRowDetail] ([Id], [BlockId], [RackId]) VALUES (5, 1, 0)
GO
INSERT [dbo].[RackRowDetail] ([Id], [BlockId], [RackId]) VALUES (6, 1, 0)
GO
INSERT [dbo].[RackRowDetail] ([Id], [BlockId], [RackId]) VALUES (7, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[RackRowDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[RackRowSection] ON 

GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (1, 1, 1, 0, 1, N'Test', NULL, N'BLK-ROW1-SEC1')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (2, 1, 1, 0, 1, N'Test', NULL, N'BLK-ROW1-SEC2')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (3, 1, 1, 0, 1, N'Test', NULL, N'BLK-ROW1-SEC3')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (4, 1, 1, 0, 1, N'Test', NULL, N'BLK-ROW1-SEC4')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (5, 1, 1, 0, 1, N'Test', NULL, N'BLK-ROW1-SEC5')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (6, 1, 1, 0, 2, N'Test', NULL, N'BLK-ROW2-SEC1')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (7, 1, 1, 0, 2, N'Test', NULL, N'BLK-ROW2-SEC2')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (8, 1, 1, 0, 2, N'Test', NULL, N'BLK-ROW2-SEC3')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (9, 1, 1, 0, 2, N'Test', NULL, N'BLK-ROW2-SEC4')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (10, 1, 1, 0, 2, N'Test', NULL, N'BLK-ROW2-SEC5')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (11, 1, 1, 0, 3, N'Test', NULL, N'BLK-ROW3-SEC1')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (12, 1, 1, 0, 3, N'Test', NULL, N'BLK-ROW3-SEC2')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (13, 1, 1, 0, 3, N'Test', NULL, N'BLK-ROW3-SEC3')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (14, 1, 1, 0, 3, N'Test', NULL, N'BLK-ROW3-SEC4')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (15, 1, 1, 0, 3, N'Test', NULL, N'BLK-ROW3-SEC5')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (16, 1, 1, 0, 4, N'Test', NULL, N'BLK-ROW4-SEC1')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (17, 1, 1, 0, 4, N'Test', NULL, N'BLK-ROW4-SEC2')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (18, 1, 1, 0, 4, N'Test', NULL, N'BLK-ROW4-SEC3')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (19, 1, 1, 0, 4, N'Test', NULL, N'BLK-ROW4-SEC4')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (20, 1, 1, 0, 4, N'Test', NULL, N'BLK-ROW4-SEC5')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (21, 1, 1, 0, 5, N'Test', NULL, N'BLK-ROW5-SEC1')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (22, 1, 1, 0, 5, N'Test', NULL, N'BLK-ROW5-SEC2')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (23, 1, 1, 0, 5, N'Test', NULL, N'BLK-ROW5-SEC3')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (24, 1, 1, 0, 5, N'Test', NULL, N'BLK-ROW5-SEC4')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (25, 1, 1, 0, 5, N'Test', NULL, N'BLK-ROW5-SEC5')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (26, 1, 1, 0, 6, N'Test', NULL, N'BLK-ROW1-SEC1')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (27, 1, 1, 0, 6, N'Test', NULL, N'BLK-ROW1-SEC2')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (28, 1, 1, 0, 7, N'Test', NULL, N'BLK-ROW2-SEC1')
GO
INSERT [dbo].[RackRowSection] ([Id], [WareHouseId], [BockId], [RackId], [RowRackId], [Name], [Description], [BarCode]) VALUES (29, 1, 1, 0, 7, N'Test', NULL, N'BLK-ROW2-SEC2')
GO
SET IDENTITY_INSERT [dbo].[RackRowSection] OFF
GO
SET IDENTITY_INSERT [dbo].[Timezone] ON 

GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (1, N'Hawaiian Standard Time', N'(GMT-10:00) Hawaii', N'GMT-10:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (2, N'Pacific Standard Time', N'(GMT-08:00) Pacific Time (US & Canada)', N'GMT-08:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (3, N'Central America Standard Time', N'(GMT-06:00) Central America', N'GMT-06:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (4, N'Central Standard Time (Mexico)', N'(GMT-06:00) Guadalajara, Mexico City, Monterrey', N'GMT-06:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (5, N'SA Pacific Standard Time', N'(GMT-05:00) Bogota, Lima, Quito, Rio Branco', N'GMT-05:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (6, N'Eastern Standard Time', N'(GMT-05:00) Eastern Time (US & Canada)', N'GMT-05:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (7, N'Venezuela Standard Time', N'(GMT-04:30) Caracas', N'GMT-04:30')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (8, N'Paraguay Standard Time', N'(GMT-04:00) Asuncion, Paraguay', N'GMT-04:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (9, N'Atlantic Standard Time', N'(GMT-04:00) Atlantic Time (Canada)', N'GMT-04:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (10, N'SA Western Standard Time', N'(GMT-04:00) Georgetown, La Paz, Manaus, San Juan', N'GMT-04:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (11, N'E. South America Standard Time', N'(GMT-03:00) Brasilia', N'GMT-03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (12, N'Argentina Standard Time', N'(GMT-03:00) Buenos Aires', N'GMT-03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (13, N'SA Eastern Standard Time', N'(GMT-03:00) Cayenne, Fortaleza', N'GMT-03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (14, N'Greenland Standard Time', N'(GMT-03:00) Greenland', N'GMT-03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (15, N'Montevideo Standard Time', N'(GMT-03:00) Montevideo', N'GMT-03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (16, N'Pacific SA Standard Time', N'(GMT-03:00) Santiago', N'GMT-03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (17, N'Cape Verde Standard Time', N'(GMT-01:00) Cabo Verde Is.', N'GMT-01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (18, N'Morocco Standard Time', N'(GMT) Casablanca', N'GMT Casa')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (19, N'GMT Standard Time', N'(GMT) Dublin, Edinburgh, Lisbon, London', N'GMT Dublin')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (20, N'Greenwich Standard Time', N'(GMT) Monrovia, Reykjavik', N'GMT Monr')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (21, N'W. Europe Standard Time', N'(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna', N'GMT+01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (22, N'Central Europe Standard Time', N'(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague', N'GMT+01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (23, N'Romance Standard Time', N'(GMT+01:00) Brussels, Copenhagen, Madrid, Paris', N'GMT+01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (24, N'Central European Standard Time', N'(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb, Portugal', N'GMT+01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (25, N'W. Central Africa Standard Time', N'(GMT+01:00) West Central Africa', N'GMT+01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (26, N'Namibia Standard Time', N'(GMT+01:00) Windhoek', N'GMT+01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (27, N'Jordan Standard Time', N'(GMT+02:00) Amman', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (28, N'GTB Standard Time', N'(GMT+02:00) Athens, Bucharest', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (29, N'Middle East Standard Time', N'(GMT+02:00) Beirut', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (30, N'Egypt Standard Time', N'(GMT+02:00) Cairo', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (31, N'Syria Standard Time', N'(GMT+02:00) Damascus', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (32, N'E. Europe Standard Time', N'(GMT+02:00) E. Europe', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (33, N'South Africa Standard Time', N'(GMT+02:00) Harare, Pretoria', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (34, N'FLE Standard Time', N'(GMT+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (35, N'Turkey Standard Time', N'(GMT+02:00) Istanbul', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (36, N'Israel Standard Time', N'(GMT+02:00) Jerusalem', N'GMT+02:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (37, N'Arabic Standard Time', N'(GMT+03:00) Baghdad', N'GMT+03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (38, N'Arab Standard Time', N'(GMT+03:00) Kuwait, Riyadh', N'GMT+03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (39, N'Belarus Standard Time', N'(GMT+03:00) Minsk', N'GMT+03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (40, N'Russian Standard Time', N'(GMT+03:00) Moscow, St. Petersburg, Volgograd (RTZ 2)', N'GMT+03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (41, N'E. Africa Standard Time', N'(GMT+03:00) Nairobi, South Sudan', N'GMT+03:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (42, N'Iran Standard Time', N'(GMT+03:30) Tehran', N'GMT+03:30')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (43, N'Arabian Standard Time', N'(GMT+04:00) Abu Dhabi, Muscat', N'GMT+04:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (44, N'Azerbaijan Standard Time', N'(GMT+04:00) Baku', N'GMT+04:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (45, N'Mauritius Standard Time', N'(GMT+04:00) Port Louis', N'GMT+04:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (46, N'Georgian Standard Time', N'(GMT+04:00) Tbilisi', N'GMT+04:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (47, N'Caucasus Standard Time', N'(GMT+04:00) Yerevan', N'GMT+04:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (48, N'West Asia Standard Time', N'(GMT+05:00) Ashgabat, Tashkent', N'GMT+05:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (49, N'Pakistan Standard Time', N'(GMT+05:00) Islamabad, Karachi', N'GMT+05:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (50, N'India Standard Time', N'(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi', N'GMT+05:30')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (51, N'Sri Lanka Standard Time', N'(GMT+05:30) Sri Jayawardenepura', N'GMT+05:30')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (52, N'Nepal Standard Time', N'(GMT+05:45) Kathmandu', N'GMT+05:45')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (53, N'Central Asia Standard Time', N'(GMT+06:00) Astana', N'GMT+06:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (54, N'Bangladesh Standard Time', N'(GMT+06:00) Dhaka', N'GMT+06:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (55, N'Myanmar Standard Time', N'(GMT+06:30) Yangon (Rangoon)', N'GMT+06:30')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (56, N'SE Asia Standard Time', N'(GMT+07:00) Bangkok, Hanoi, Jakarta', N'GMT+07:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (57, N'China Standard Time', N'(GMT+08:00) Beijing, Chongqing, Hong Kong, Urumqi', N'GMT+08:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (58, N'Singapore Standard Time', N'(GMT+08:00) Kuala Lumpur, Singapore', N'GMT+08:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (59, N'Taipei Standard Time', N'(GMT+08:00) Taipei', N'GMT+08:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (60, N'Ulaanbaatar Standard Time', N'(GMT+08:00) Ulaanbaatar', N'GMT+08:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (61, N'Tokyo Standard Time', N'(GMT+09:00) Osaka, Sapporo, Tokyo', N'GMT+09:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (62, N'Korea Standard Time', N'(GMT+09:00) Seoul', N'GMT+09:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (63, N'AUS Eastern Standard Time', N'(GMT+10:00) Canberra, Melbourne, Sydney', N'GMT+10:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (64, N'West Pacific Standard Time', N'(GMT+10:00) Guam, Port Moresby', N'GMT+10:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (65, N'Central Pacific Standard Time', N'(GMT+11:00) Solomon Is., New Caledonia', N'GMT+11:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (66, N'New Zealand Standard Time', N'(GMT+12:00) Auckland, Wellington', N'GMT+12:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (67, N'Fiji Standard Time', N'(GMT+12:00) Fiji', N'GMT+12:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (68, N'Tonga Standard Time', N'(GMT+13:00) Nuku''alofa', N'GMT+13:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (69, N'Samoa Standard Time', N'(GMT+13:00) Samoa', N'GMT+13:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (70, N'West Africa Standard Time', N'(GMT+01:00) Kinshasa, Democratic Republic of the Congo', N'GMT+01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (71, N'East Timor Time', N'(GMT+09:00) Timor-Leste', N'GMT+09:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (72, N'Tahiti Time', N'(GMT-10:00) Pape''ete, French Polynesia', N'GMT-10:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (74, N'(GMT-02:00) Nuuk, Greenland', N'GMT-02:00', NULL)
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (75, N'East Kazakhstan Time', N'(GMT+06:00) East Kazakhstan', N'GMT+06:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (76, N'Western European Summer Time', N'(GMT+01:00) Morocco,Western Sahara, Canary Islands, Spain', N'GMT+01:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (77, N'UTC-11', N'(UTC-11:00)', N'UTC-11:00')
GO
INSERT [dbo].[Timezone] ([TimezoneId], [Name], [Offset], [OffsetShort]) VALUES (78, N'Coordinated Universal Time', N'(UTC+12', NULL)
GO
SET IDENTITY_INSERT [dbo].[Timezone] OFF
GO
SET IDENTITY_INSERT [dbo].[UserConfiguration] ON 

GO
INSERT [dbo].[UserConfiguration] ([Id], [UserId], [MailHostName], [SMTPport], [SMTPUserName], [SMTPPassword], [SMTPDisplayName], [FromMail], [EnableSsl]) VALUES (2, 0, N'smtp.gmail.com', N'587', N'smtptest08112016@gmail.com', N'test1234#', N'Loin Star - System ', N'smtptest08112016@gmail.com', N'Y')
GO
SET IDENTITY_INSERT [dbo].[UserConfiguration] OFF
GO
SET IDENTITY_INSERT [dbo].[UserModuleScreen] ON 

GO
INSERT [dbo].[UserModuleScreen] ([Id], [UserId], [ModuleScreenId], [ModuleScreenDetailId], [IsAllowed], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdateBy]) VALUES (62, 1, 1, 0, 1, CAST(N'2019-01-21 05:28:59.783' AS DateTime), N'1', NULL, NULL)
GO
INSERT [dbo].[UserModuleScreen] ([Id], [UserId], [ModuleScreenId], [ModuleScreenDetailId], [IsAllowed], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdateBy]) VALUES (63, 1, 4, 0, 1, CAST(N'2019-01-21 05:28:59.783' AS DateTime), N'1', NULL, NULL)
GO
INSERT [dbo].[UserModuleScreen] ([Id], [UserId], [ModuleScreenId], [ModuleScreenDetailId], [IsAllowed], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdateBy]) VALUES (64, 1, 5, 0, 1, CAST(N'2019-01-21 05:28:59.783' AS DateTime), N'1', NULL, NULL)
GO
INSERT [dbo].[UserModuleScreen] ([Id], [UserId], [ModuleScreenId], [ModuleScreenDetailId], [IsAllowed], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdateBy]) VALUES (65, 1, 6, 0, 1, CAST(N'2019-01-21 05:28:59.783' AS DateTime), N'1', NULL, NULL)
GO
INSERT [dbo].[UserModuleScreen] ([Id], [UserId], [ModuleScreenId], [ModuleScreenDetailId], [IsAllowed], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdateBy]) VALUES (66, 1, 7, 0, 1, CAST(N'2019-01-21 05:28:59.783' AS DateTime), N'1', NULL, NULL)
GO
INSERT [dbo].[UserModuleScreen] ([Id], [UserId], [ModuleScreenId], [ModuleScreenDetailId], [IsAllowed], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdateBy]) VALUES (67, 1, 8, 0, 1, CAST(N'2019-01-21 05:28:59.783' AS DateTime), N'1', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[UserModuleScreen] OFF
GO
SET IDENTITY_INSERT [dbo].[Warehouse] ON 

GO
INSERT [dbo].[Warehouse] ([WarehouseId], [Address], [Address2], [Address3], [City], [State], [Zip], [CountryId], [LocationName], [LocationLatitude], [LocationLongitude], [LocationZoom], [MarkerLatitude], [MarkerLongitude], [LocationMapImage], [ManagerId], [Email], [TelephoneNo], [MobileNo], [Fax], [WorkingStartTime], [WorkingEndTime], [TimeZoneId], [WorkingWeekDayId]) VALUES (1, N'1 Mellstock Avenue', N'Ddd', N'', N'DORCHESTER', NULL, N'DT1 1UR', 228, N'Hong Kong', CAST(25.54693302916922 AS Decimal(18, 14)), CAST(115.89262962341309 AS Decimal(18, 14)), 4, CAST(22.78092970061147 AS Decimal(18, 14)), CAST(114.28587913513184 AS Decimal(18, 14)), N'1_16_05_2016_11_51_15.png', 0, N'warehouse@irasyssolutions.com', N'521421421', N'', N'', CAST(N'04:30:00' AS Time), CAST(N'13:30:00' AS Time), 70, 9)
GO
SET IDENTITY_INSERT [dbo].[Warehouse] OFF
GO
SET IDENTITY_INSERT [dbo].[WMSRole] ON 

GO
INSERT [dbo].[WMSRole] ([Id], [Name], [DisplayName]) VALUES (1, N'Admin', N'Admin')
GO
INSERT [dbo].[WMSRole] ([Id], [Name], [DisplayName]) VALUES (2, N'Sales', N'Sales User')
GO
SET IDENTITY_INSERT [dbo].[WMSRole] OFF
GO
SET IDENTITY_INSERT [dbo].[WMSUser] ON 

GO
INSERT [dbo].[WMSUser] ([Id], [ActivationToken], [PasswordAnswer], [PasswordQuestion], [CompanyName], [ContactFirstName], [ContactLastName], [Email], [PasswordHash], [EmailConfirmed], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [TelephoneNo], [MobileNo], [FaxNumber], [TimezoneId], [ShortName], [Position], [Skype], [IsActive], [CreatedOnUtc], [CreatedBy], [UpdatedOnUtc], [UpdatedBy], [ProfileImage], [LastLoginOnUtc], [LastPasswordChangedOnUtc]) VALUES (1, NULL, NULL, NULL, N'IRA', N'Prakash', N'Pant', N'prakash.pant@irasys.biz', N'AEfIqeXFTFzbPhfnzvZdBIASTwMCi2WQHWqC+pToBFa91B/FnWgmC2XQgXBr2rlpIg==', 0, N'b5afa36b-2950-4d08-b1f6-def811bf9da9', N'123456', 0, 0, NULL, 0, 0, N'prakash_irasys', N'123456', N'123456', NULL, 1, N'12qw', N'12', N'1', 1, CAST(N'2019-01-14 06:42:36.390' AS DateTime), 17, NULL, NULL, N'staff.png', NULL, NULL)
GO
INSERT [dbo].[WMSUser] ([Id], [ActivationToken], [PasswordAnswer], [PasswordQuestion], [CompanyName], [ContactFirstName], [ContactLastName], [Email], [PasswordHash], [EmailConfirmed], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [TelephoneNo], [MobileNo], [FaxNumber], [TimezoneId], [ShortName], [Position], [Skype], [IsActive], [CreatedOnUtc], [CreatedBy], [UpdatedOnUtc], [UpdatedBy], [ProfileImage], [LastLoginOnUtc], [LastPasswordChangedOnUtc]) VALUES (3, NULL, NULL, NULL, N'', N'Prakash Chandra', N'Pant', N'gaurav.dabral@irasys.biz', N'ADnTFOf9gBYQEN622nZmUCL7wmf3oGcQQoZ0E98GWvmKyz2N4WRhloosBaQueT3I/g==', 0, N'2cffa68e-4ae4-4d76-9651-70c214220312', N'0565464', 0, 0, NULL, 0, 0, N'gaurav.dabral@irasys.biz', N'0565464', N'', NULL, 0, N'Tradelane', N'Admin', N'asdasd', 1, CAST(N'2019-01-22 08:18:22.283' AS DateTime), 1, NULL, NULL, N'staff.png', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[WMSUser] OFF
GO
SET IDENTITY_INSERT [dbo].[WMSUserAddress] ON 

GO
INSERT [dbo].[WMSUserAddress] ([Id], [UserId], [Address], [Address2], [Address3], [Suburb], [City], [State], [PostCode], [CountryId]) VALUES (1, 1, N'G-92', N'sec 63', NULL, N'', N'Noida', N'Up', N'201301', 94)
GO
INSERT [dbo].[WMSUserAddress] ([Id], [UserId], [Address], [Address2], [Address3], [Suburb], [City], [State], [PostCode], [CountryId]) VALUES (2, 3, N'G-92', N'sec 63', NULL, N'', N'Noida', N'Up', N'201301', 94)
GO
SET IDENTITY_INSERT [dbo].[WMSUserAddress] OFF
GO
INSERT [dbo].[WMSUserRole] ([UserId], [RoleId]) VALUES (1, 1)
GO
INSERT [dbo].[WMSUserRole] ([UserId], [RoleId]) VALUES (2, 2)
GO
INSERT [dbo].[WMSUserRole] ([UserId], [RoleId]) VALUES (3, 2)
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
USE [master]
GO
ALTER DATABASE [WMS] SET  READ_WRITE 
GO
