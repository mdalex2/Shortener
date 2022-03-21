/*SQL_SERVER*/
USE [Shortener]
GO
/****** Object:  StoredProcedure [dbo].[GenerarTokenAleatorio]    Script Date: 19/2/2021 16:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SE ELIMINA EL SP SI EXISTE   --
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GenerarTokenAleatorio]') AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GenerarTokenAleatorio]
GO  

-- SE CREA EL SP CON LAS NUEVAS CONFIGURACIONES
CREATE PROC [dbo].[GenerarTokenAleatorio]    
@Length			int = 6,
@CaracteresPermitidos varchar(40) = 'ABCDEFGHIJKLMNPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
AS  

DECLARE @PoolLength int
DECLARE @LoopCount int
DECLARE @RandomString VARCHAR(40)

SET @PoolLength = Len(@CaracteresPermitidos) + 1
SET @LoopCount = 0
SET @RandomString = ''

WHILE (@LoopCount < @Length or @RandomString='') 
BEGIN
    SELECT @RandomString = @RandomString + SUBSTRING(@CaracteresPermitidos, CONVERT(int, Rand() * @PoolLength), 1)
	SELECT @LoopCount = @LoopCount + 1
END
TRUNCATE TABLE #RandomString;
insert into #RandomString values (@RandomString)
