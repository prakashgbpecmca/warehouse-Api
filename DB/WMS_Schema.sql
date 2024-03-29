USE [master]
GO
/****** Object:  Database [WMS]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[Block]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[Country]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[ModuleScreen]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[ModuleScreenDetail]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[Product]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[Rack]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[RackRowDetail]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[RackRowSection]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[Timezone]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[UserConfiguration]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[UserModuleScreen]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[Warehouse]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[WMSRole]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[WMSUser]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[WMSUserAdditional]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[WMSUserAddress]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[WMSUserClaim]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[WMSUserLogin]    Script Date: 1/31/2019 1:22:34 PM ******/
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
/****** Object:  Table [dbo].[WMSUserRole]    Script Date: 1/31/2019 1:22:34 PM ******/
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
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
USE [master]
GO
ALTER DATABASE [WMS] SET  READ_WRITE 
GO
