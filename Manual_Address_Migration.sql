-- ================================================================
-- Manual SQL Migration: Add Address Fields to AspNetUsers
-- ================================================================
-- This script manually adds address fields if you prefer not to use EF migrations
-- 
-- ?? WARNING: Only run this if you're NOT using Entity Framework migrations!
-- ================================================================

USE RecyclingDB;
GO

-- Check if columns already exist before adding
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'City')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [City] NVARCHAR(100) NULL;
    PRINT '? Column [City] added successfully';
END
ELSE
BEGIN
    PRINT '?? Column [City] already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'Street')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [Street] NVARCHAR(200) NULL;
    PRINT '? Column [Street] added successfully';
END
ELSE
BEGIN
    PRINT '?? Column [Street] already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'BuildingNo')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [BuildingNo] NVARCHAR(50) NULL;
    PRINT '? Column [BuildingNo] added successfully';
END
ELSE
BEGIN
    PRINT '?? Column [BuildingNo] already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'Apartment')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [Apartment] NVARCHAR(50) NULL;
    PRINT '? Column [Apartment] added successfully';
END
ELSE
BEGIN
    PRINT '?? Column [Apartment] already exists';
END
GO

-- Verify the changes
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetUsers'
  AND COLUMN_NAME IN ('City', 'Street', 'BuildingNo', 'Apartment');
GO

PRINT '? Migration completed successfully!';
GO
