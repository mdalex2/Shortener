/*SQL_SERVER*/
USE [Shortener]
GO
/****** Object:  StoredProcedure [dbo].[ElementosAlta_noexception]    Script Date: 19/2/2021 16:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SE ELIMINA EL SP SI EXISTE   --
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[crear_guardar_url_corta]') AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[crear_guardar_url_corta]
GO  

-- SE CREA EL SP CON LAS NUEVAS CONFIGURACIONES
CREATE PROC [dbo].[crear_guardar_url_corta]
@ID					int				= null output,
@UrlLarga			varchar(6000)	= null,
@UrlCorta			varchar(100)	= null,
@FechaCreacion		datetime		= null, 
@FechaExpira		datetime		= null, 
@Habilitado			bit				= null,
@LongitudToken		int				= 6 

AS  

DECLARE @tokenAleatorio varchar(40) = ''
DECLARE @NumVisitas int = 0
DECLARE @CaracteresPermitidos varchar(40) = 'ABCDEFGHIJKLMNPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
DECLARE @LenPermitidos INT
DECLARE @LoopWhile int
CREATE TABLE #RandomString(tokenUrl nvarchar(40))

IF @FechaExpira IS NULL OR @FechaExpira = ''	 
	SET @FechaExpira = (SELECT DATEADD(MONTH,1, GETDATE()))	
	
if @UrlLarga = '' or @UrlLarga is null
	BEGIN
		RAISERROR ('No se recibió la Url a acortar', 16, 1);  
		return
	END

--SI EXISTE LA URL DEVUELVO ERROR
--IF NOT EXISTS (SELECT @UrlLarga from UrlConfigs where UrlLarga = @UrlLarga)

	--BEGIN
		EXECUTE @tokenAleatorio = dbo.GenerarTokenAleatorio @LongitudToken,@CaracteresPermitidos
		set @tokenAleatorio = (SELECT tokenUrl from #RandomString)	
		SET @LenPermitidos = LEN(@CaracteresPermitidos)

		SET @LoopWhile = 0
		WHILE (@tokenAleatorio =  (select DISTINCT(UrlCorta) from UrlConfigs where UrlCorta = @tokenAleatorio ) and  @LoopWhile<(@LenPermitidos * @LongitudToken))
			BEGIN  	
				EXECUTE @tokenAleatorio = dbo.GenerarTokenAleatorio @LongitudToken,@CaracteresPermitidos
				set @tokenAleatorio = (SELECT tokenUrl from #RandomString)
				SET @LoopWhile = @LoopWhile + 1			 
			END 

		IF (not exists (select UrlCorta from UrlConfigs where UrlCorta = @tokenAleatorio))
			BEGIN		
				INSERT INTO [UrlConfigs] (UrlLarga,UrlCorta,FechaCreacion,Habilitado,NumVisitas,FechaExpira) VALUES (@UrlLarga,@tokenAleatorio,GETDATE(),@Habilitado,@NumVisitas,@FechaExpira)
				SELECT @@IDENTITY ID, UrlLarga,UrlCorta,FechaCreacion,Habilitado,NumVisitas,FechaExpira from UrlConfigs WHERE ID = @@IDENTITY
			END
		ELSE
			RAISERROR ('Se llegó al tope de combinaciones de  URL cortas.', 16,1);  
	--END
	--ELSE
		--BEGIN			
			--RAISERROR ('Ya existe la URL: "%s".', 16, 1, @UrlLarga);  
		--END
	
	

		

GO  

