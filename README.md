DataBase:
USE TaskManagerDB;
GO

CREATE TABLE [dbo].[Users] (
    [UserID] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE [dbo].[Statuses] (
    [StatusID] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL,
    [Color] NVARCHAR(7) NOT NULL
);
GO

CREATE TABLE [dbo].[Priorities] (
    [PriorityID] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL,
    [Color] NVARCHAR(7) NOT NULL
);
GO

CREATE TABLE [dbo].[Tasks] (
    [TaskID] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX),
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    [DueDate] DATETIME,
    [PriorityID] INT FOREIGN KEY REFERENCES Priorities(PriorityID),
    [StatusID] INT FOREIGN KEY REFERENCES Statuses(StatusID),
    [UserID] INT FOREIGN KEY REFERENCES Users(UserID)
);
GO

INSERT INTO Statuses (Name, Color) VALUES
    (N'К выполнению', '#757575'),
    (N'В процессе', '#2196F3'),
    (N'Завершено', '#4CAF50');
GO

INSERT INTO Priorities (Name, Color) VALUES
    (N'Высокий', '#F44336'),
    (N'Средний', '#FF9800'),
    (N'Низкий', '#4CAF50');
GO
