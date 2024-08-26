

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL, 
    AvatarUrl NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);
ALTER TABLE Users
ADD VerificationToken NVARCHAR(255) NULL;
DROP TABLE Users;

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    VerificationToken NVARCHAR(255) NULL,
    EmailVerified BIT NOT NULL DEFAULT 0
);
ALTER TABLE Users
ADD InvalidLoginAttempts INT DEFAULT 0,
    LockoutEndDate DATETIME NULL;

ALTER TABLE Users
ADD PasswordResetToken NVARCHAR(255) NULL,
    PasswordResetTokenExpiry DATETIME NULL;


delete from Users where UserID = 17


update Users set InvalidLoginAttempts = 0
where UserID = 15

ALTER TABLE Users
ADD HasSetPreferences BIT DEFAULT 0;

CREATE TABLE Preferences (
    PreferenceId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    BookField VARCHAR(255) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

INSERT INTO Categories (CategoryName) VALUES ('Fiction');
INSERT INTO Categories (CategoryName) VALUES ('Non-Fiction');
INSERT INTO Categories (CategoryName) VALUES ('Science');
INSERT INTO Categories (CategoryName) VALUES ('History');

CREATE TABLE Books (
    BookID INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255),
    Author NVARCHAR(255),
    Price DECIMAL(10, 2),
    Subject NVARCHAR(255)
);

CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY,
    CustomerID INT,
    OrderDate DATETIME,
    Status NVARCHAR(50) 
);


CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    Quantity INT
);



select * from UserPreferences
select * from Users
