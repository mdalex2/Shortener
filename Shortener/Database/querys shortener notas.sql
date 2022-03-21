DECLARE @LoopWhile int
DECLARE @link nvarchar(100)
DECLARE @NumInserts int
SET @LoopWhile = 0
SET @NumInserts = 1000
	WHILE (@LoopWhile<@NumInserts)
		BEGIN
			SET @link = 'https://google.com' + CAST(@LoopWhile as varchar(10))
			print @link	
			EXEC crear_guardar_url_corta 0,@link,'UrlCorta','20/12/1980','20/12/1980',1,6
			SET @LoopWhile = @LoopWhile + 1			 
		END 
select * from urlconfigs

exec GenerarTokenAleatorio 2

select LEN(trim('ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789'))
SELECT count(*) FROM URLCONFIGS

EXEC crear_guardar_url_corta 0,'http://google.com','UrlCorta','20/12/1980','20/12/1980',1,6

truncate table urlconfigs


select * from UrlConfigs



sp_who2 'active'
SELECT * FROM URLCONFIGS
--saber cuantos repetidos si hay
SELECT urlcorta FROM URLCONFIGS 
     GROUP BY urlcorta 
     HAVING COUNT(*)>1;

	 select * from urlconfigs where urlcorta='L'

	 update UrlConfigs set UrlLarga = 'https://www.google.com' 
	 update UrlConfigs set UrlCorta = ID