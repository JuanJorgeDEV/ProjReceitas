-- Cria o banco e as tabelas básicas usadas pelo projeto
IF DB_ID(N'db_Receitas') IS NULL
BEGIN
    CREATE DATABASE db_Receitas;
END
GO

USE db_Receitas;
GO

CREATE TABLE Usuario (
    Id_Usuario INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    SenhaHash NVARCHAR(MAX) NOT NULL,
    DataCadastro DATETIME2 DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE Receita (
    Id_Receita INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(MAX) NULL,
    Temp_Preparo INT NULL,
    Tipo NVARCHAR(50) NULL,
    Preco DECIMAL(18,2) NULL,
    Modo_Preparo NVARCHAR(MAX) NULL,
    fotoReceita VARBINARY(MAX) NULL,
    fotoReceitaTipoMIME NVARCHAR(100) NULL,
    Id_Usuario INT NULL,
    CONSTRAINT FK_Receita_Usuario FOREIGN KEY (Id_Usuario) REFERENCES Usuario(Id_Usuario)
);
GO

CREATE TABLE Ingrediente (
    Id_ingrediente INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(150) NOT NULL,
    Preco DECIMAL(18,2) NULL
);
GO

CREATE TABLE Receita_Ingredientes (
    Id_Receita_Ingrediente INT IDENTITY(1,1) PRIMARY KEY,
    Id_Receita INT NOT NULL,
    Id_ingrediente INT NOT NULL,
    Quantidade NVARCHAR(100) NULL,
    CONSTRAINT FK_RI_Receita FOREIGN KEY (Id_Receita) REFERENCES Receita(Id_Receita) ON DELETE CASCADE,
    CONSTRAINT FK_RI_Ingrediente FOREIGN KEY (Id_ingrediente) REFERENCES Ingrediente(Id_ingrediente)
);
GO

CREATE TABLE Comentario (
    id_Comentario INT IDENTITY(1,1) PRIMARY KEY,
    Id_Usuario INT NULL,
    Id_Receita INT NOT NULL,
    Texto_Comentario NVARCHAR(1000) NULL,
    Nota TINYINT NULL,
    Data_Postagem DATETIME2 DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Comentario_Usuario FOREIGN KEY (Id_Usuario) REFERENCES Usuario(Id_Usuario),
    CONSTRAINT FK_Comentario_Receita FOREIGN KEY (Id_Receita) REFERENCES Receita(Id_Receita) ON DELETE CASCADE
);
GO