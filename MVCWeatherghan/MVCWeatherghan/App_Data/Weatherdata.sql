CREATE TABLE [dbo].[Table]
(
	[AirportCode] NCHAR(4) NOT NULL , 
    [Year] INT NOT NULL, 
    [MaxTempData] NVARCHAR(MAX) NULL, 
    [CloudCoverData] NVARCHAR(MAX) NULL, 
    PRIMARY KEY ([Year], [AirportCode])
)
