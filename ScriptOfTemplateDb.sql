USE [master]
GO
/****** Object:  Database [FamilyEconomic_TEST_db]    Script Date: 10.05.2017 12:42:49 ******/
CREATE DATABASE [FamilyEconomic_TEST_db]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FamilyEconomic_TEST_db', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\FamilyEconomic_TEST_db.mdf' , SIZE = 4160KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'FamilyEconomic_TEST_db_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\FamilyEconomic_TEST_db_log.ldf' , SIZE = 1040KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FamilyEconomic_TEST_db].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET ARITHABORT OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET  ENABLE_BROKER 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET  MULTI_USER 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [FamilyEconomic_TEST_db]
GO
/****** Object:  User [IIS APPPOOL\DefaultAppPool]    Script Date: 10.05.2017 12:42:50 ******/
CREATE USER [IIS APPPOOL\DefaultAppPool] FOR LOGIN [IIS APPPOOL\DefaultAppPool] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\DefaultAppPool]
GO
/****** Object:  StoredProcedure [dbo].[GetCurrentIdMonthly]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetCurrentIdMonthly]
	@Id int output,
	@Name nvarchar(100)
AS
	SELECT @Id = Id FROM CurrentPrices WHERE Id_monthly_prices = (SELECT Id FROM MonthlyPrices WHERE Name = @Name)
GO
/****** Object:  StoredProcedure [dbo].[GetCurrentIdPermanent]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetCurrentIdPermanent]
	@Id int output,
	@Name nvarchar(100)
AS
	SELECT @Id = Id FROM CurrentPrices WHERE Id_permanent_prices = (SELECT Id FROM PermanentPrices WHERE Name = @Name)
GO
/****** Object:  StoredProcedure [dbo].[GetCurrentMonthlyId]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetCurrentMonthlyId]
	@Id_monthly_prices int output,
	@Id int
AS
	SELECT @Id_monthly_prices = Id_monthly_prices FROM CurrentPrices WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[GetCurrentPermanentId]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetCurrentPermanentId]
	@Id_permanent_prices int output,
	@Id int
AS
	SELECT @Id_permanent_prices = Id_permanent_prices FROM CurrentPrices WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[GetCurrentPriceById]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetCurrentPriceById]
	@Id int,
	@Price nvarchar(20) output
AS
	SELECT @Price = Price FROM CurrentPrices WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[GetId]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetId]
	@Name NVARCHAR (100),
	@Id int output
AS
	SELECT @Id = Id FROM PermanentPrices WHERE Name = @Name
GO
/****** Object:  StoredProcedure [dbo].[GetMonthlyGroupByName]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetMonthlyGroupByName]
	@Group nvarchar(20) output,
	@Name nvarchar(100)
AS
	SELECT @Group = [Group] FROM MonthlyPrices WHERE Name = @Name
GO
/****** Object:  StoredProcedure [dbo].[GetMonthlyPriceId]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetMonthlyPriceId]
	@Name NVARCHAR (100),
	@Id int output
AS
	SELECT @Id = Id FROM MonthlyPrices WHERE Name = @Name
GO
/****** Object:  StoredProcedure [dbo].[GetPermanentGroupByName]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPermanentGroupByName]
	@Group nvarchar(20) output,
	@Name nvarchar(100)
AS
	SELECT @Group = [Group] FROM PermanentPrices WHERE Name = @Name
GO
/****** Object:  StoredProcedure [dbo].[GetPriceById]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPriceById]
	@Id int,
	@Price nvarchar(20) output
AS
	SELECT @Price = Price FROM PermanentPrices WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[GetPriceByName]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPriceByName]
	@Name nvarchar(100),
	@Price nvarchar(20) output
AS
	SELECT @Price = Price FROM PermanentPrices WHERE Name = @Name;
GO
/****** Object:  StoredProcedure [dbo].[GetUserDataValueByName]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserDataValueByName]
	@Name nvarchar(20),
	@Value nvarchar(20) output
AS
	SELECT @Value = [Value] FROM UserData WHERE Name = @Name
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CurrentPrices]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CurrentPrices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Id_monthly_prices] [int] NULL,
	[Id_permanent_prices] [int] NULL,
	[Price] [nvarchar](20) NULL,
	[Check_box] [int] NULL,
	[Consideration] [int] NULL,
 CONSTRAINT [Prim_currentPrices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MonthlyPrices]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyPrices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Group] [nvarchar](20) NULL,
 CONSTRAINT [Prim_monthlyPrices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PermanentPrices]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermanentPrices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Price] [nvarchar](20) NOT NULL,
	[Group] [nvarchar](20) NULL,
 CONSTRAINT [Prim_permanentPrices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserData]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Value] [nvarchar](20) NULL,
 CONSTRAINT [Prim_userData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 10.05.2017 12:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoginName] [nvarchar](20) NOT NULL,
	[Mail] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.UserInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[CurrentPrices]  WITH CHECK ADD  CONSTRAINT [Foreign_monthlyPrices] FOREIGN KEY([Id_monthly_prices])
REFERENCES [dbo].[MonthlyPrices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CurrentPrices] CHECK CONSTRAINT [Foreign_monthlyPrices]
GO
ALTER TABLE [dbo].[CurrentPrices]  WITH CHECK ADD  CONSTRAINT [Foreign_permanentPrices] FOREIGN KEY([Id_permanent_prices])
REFERENCES [dbo].[PermanentPrices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CurrentPrices] CHECK CONSTRAINT [Foreign_permanentPrices]
GO
ALTER TABLE [dbo].[CurrentPrices]  WITH CHECK ADD  CONSTRAINT [Check_check_box] CHECK  (([Check_box]=(1) OR [Check_box]=(0)))
GO
ALTER TABLE [dbo].[CurrentPrices] CHECK CONSTRAINT [Check_check_box]
GO
USE [master]
GO
ALTER DATABASE [FamilyEconomic_TEST_db] SET  READ_WRITE 
GO
