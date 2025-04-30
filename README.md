DataBase:
USE TaskManagerDB

CREATE TABLE [dbo].[Users] (
    [UserID] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL,
    [Password] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL
);

CREATE TABLE [dbo].[Tasks] (
    [TaskID] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX),
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [DueDate] DATETIME,
    [Priority] INT,
    [Status] INT,
    [UserID] INT FOREIGN KEY REFERENCES Users(UserID)
);
