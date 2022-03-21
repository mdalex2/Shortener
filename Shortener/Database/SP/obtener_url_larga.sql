/*SQL_SERVER*/
USE [Shortener]
GO
/****** Object:  StoredProcedure [dbo].[obtener_url_larga]    Script Date: 19/2/2021 16:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SE ELIMINA EL SP SI EXISTE   --
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[obtener_url_larga]') AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[obtener_url_larga]
GO  

-- SE CREA EL SP CON LAS NUEVAS CONFIGURACIONES
CREATE PROC [dbo].[obtener_url_larga]    
@tokenUrlCorta			nvarchar(40)
AS  

declare @UrlLarga nvarchar(2000)
declare @ID int

SELECT @UrlLarga = UrlLarga,@ID = ID from urlconfigs where urlcorta = @tokenUrlCorta COLLATE SQL_Latin1_General_CP1_CS_AS
if @UrlLarga is null or @UrlLarga = ''
	RAISERROR ('No se encontró la Url', -- Message text.  
               16, -- Severity.  
               1 -- State.  
               ); 
ELSE
	begin
		update urlconfigs set NumVisitas = NumVisitas + 1 where ID = @ID
		SELECT @UrlLarga UrlLarga
	end
