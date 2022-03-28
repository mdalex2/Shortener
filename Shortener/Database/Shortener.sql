USE Shortener
drop table if exists UrlShorts
CREATE TABLE UrlShorts
(
	Id int NOT NULL PRIMARY KEY IDENTITY,
    UrlCorta nvarchar(20) NOT NULL UNIQUE,
    UrlLarga nvarchar(3000) not null,
    FechaCreacion DATETIME not null default getdate(),
    FechaModificacion DATETIME not null default getdate(),
    FechaExpira DATETIME NULL,
    Habilitado BIT NOT NULL DEFAULT 1,
    NumVisitas BIGINT NOT NULL DEFAULT 0,
	CodProducto nvarchar(20),
	Producto nvarchar(100),
	Observaciones nvarchar(300),
    Eliminado bit not null default 0
)

