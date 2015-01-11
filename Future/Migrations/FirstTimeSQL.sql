drop table IF EXISTS ViFutureInfo;

CREATE TABLE  [ViFutureInfo] (
    [AppName]         VARCHAR (255) NOT NULL,
    [MainPhone]       VARCHAR (255) NULL,
    [AfterHousePhone] VARCHAR (255) NULL,
    [SupportEmail]    VARCHAR (255) NULL,
    [MarketingEmail]  VARCHAR (255) NULL,
    [GeneralEmail]    VARCHAR (255) NULL,
    [AddressStreet]   VARCHAR (255) NULL,
    [AddressCity]     VARCHAR (255) NULL,
    [AddressState]    VARCHAR (255) NULL,
    [AddressZipCode]  VARCHAR (255) NULL,
    [AboutInfo]       VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([AppName] ASC)
);


INSERT INTO [ViFutureInfo] ([AppName], [MainPhone], [AfterHousePhone], [SupportEmail], [MarketingEmail], [GeneralEmail], [AddressStreet], [AddressCity], [AddressState], [AddressZipCode], [AboutInfo]) 
VALUES (N'Future', N'(814)-753-0917', N'(814)-753-0917', N'lukezeng@live.com', N'lukezeng@live.com', N'lukezeng@live.com', N'206 S 13th St', N'Philadelphia', N'PA', N'19107', N'This is the AboutInfo in the database');
