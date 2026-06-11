-- ============================================================
-- Task Management System - Database Setup Script
-- Run this manually if you prefer not to use EF Migrations.
-- The app also auto-applies migrations on startup.
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskManagementDb')
BEGIN
    CREATE DATABASE TaskManagementDb;
END
GO

USE TaskManagementDb;
GO

-- ── Users ────────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id        INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName  NVARCHAR(100) NOT NULL,
        Email     NVARCHAR(200) NOT NULL
    );
    CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
END
GO

-- ── Projects ─────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Projects' AND xtype='U')
BEGIN
    CREATE TABLE Projects (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL
    );
END
GO

-- ── TaskItems ─────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskItems' AND xtype='U')
BEGIN
    CREATE TABLE TaskItems (
        Id             INT IDENTITY(1,1) PRIMARY KEY,
        Title          NVARCHAR(200) NOT NULL,
        Description    NVARCHAR(1000) NULL,
        Status         INT NOT NULL DEFAULT 0,  -- 0=Todo, 1=InProgress, 2=Done, 3=Cancelled
        ProjectId      INT NOT NULL,
        AssignedUserId INT NULL,
        CONSTRAINT FK_TaskItems_Projects FOREIGN KEY (ProjectId)      REFERENCES Projects(Id) ON DELETE CASCADE,
        CONSTRAINT FK_TaskItems_Users    FOREIGN KEY (AssignedUserId) REFERENCES Users(Id)    ON DELETE SET NULL
    );
    CREATE INDEX IX_TaskItems_ProjectId      ON TaskItems(ProjectId);
    CREATE INDEX IX_TaskItems_AssignedUserId ON TaskItems(AssignedUserId);
END
GO

-- ── Comments ─────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Comments' AND xtype='U')
BEGIN
    CREATE TABLE Comments (
        Id      INT IDENTITY(1,1) PRIMARY KEY,
        Content NVARCHAR(2000) NOT NULL,
        TaskId  INT NOT NULL,
        CONSTRAINT FK_Comments_TaskItems FOREIGN KEY (TaskId) REFERENCES TaskItems(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_Comments_TaskId ON Comments(TaskId);
END
GO

-- ── EF Migrations history table (needed if mixing script + migrations) ───────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='__EFMigrationsHistory' AND xtype='U')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL,
        ProductVersion NVARCHAR(32)  NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );
    INSERT INTO __EFMigrationsHistory VALUES ('20240101000000_InitialCreate', '8.0.0');
END
GO

-- ── Sample seed data ─────────────────────────────────────────
INSERT INTO Users (FirstName, LastName, Email)
VALUES
    ('Alice', 'Johnson', 'alice@example.com'),
    ('Bob',   'Smith',   'bob@example.com');

INSERT INTO Projects (Name, Description)
VALUES
    ('Website Redesign', 'Full redesign of the company website'),
    ('Mobile App',       'iOS and Android mobile application');

INSERT INTO TaskItems (Title, Description, Status, ProjectId, AssignedUserId)
VALUES
    ('Design homepage mockup', 'Create Figma mockups', 0, 1, 1),
    ('Set up CI/CD pipeline',  'GitHub Actions setup', 1, 1, 2),
    ('Build login screen',     'JWT auth integration', 0, 2, 1);

INSERT INTO Comments (Content, TaskId)
VALUES
    ('Mockup looks great!',      1),
    ('Pipeline is almost done.', 2),
    ('Need clarification on UI', 3);
GO

PRINT 'Database setup complete!';
