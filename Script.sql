CREATE DATABASE bduniversidade;
USE bduniversidade;

-- Tabela de Usuários (Independente)
CREATE TABLE Usuarios(
    id int primary key auto_increment,
    nome varchar(100),
    email varchar(100),
    senha_hash varchar(255),
    role enum ("Biblioteca","Admin"),
    ativo tinyint(1) default 1,
    criado_Em datetime default current_timestamp
);

-- Tabela Departamento (Independente)
CREATE TABLE departamento(
    id_depto INT PRIMARY KEY,
    nome VARCHAR(80) NOT NULL,
    sigla VARCHAR(10) NOT NULL
);

-- Tabela Professor (Depende de Departamento)
CREATE TABLE professor(
    id_professor INT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cpf CHAR(11) UNIQUE,
    titulacao VARCHAR(30) NOT NULL,
    id_depto INT,
    FOREIGN KEY (id_depto) REFERENCES departamento(id_depto)
);

-- Tabela Aluno (Independente)
CREATE TABLE aluno(
    id_aluno INT PRIMARY KEY,
    ra VARCHAR(20) UNIQUE NOT NULL,
    nome VARCHAR(100) NOT NULL,
    cpf CHAR(11) UNIQUE,
    data_ingresso DATE NOT NULL
);

-- Tabela Disciplina (Depende de Departamento)
CREATE TABLE disciplina(
    id_disciplina INT PRIMARY KEY,
    codigo VARCHAR(10) UNIQUE NOT NULL,
    nome VARCHAR(100) NOT NULL,
    carga_horaria INT NOT NULL,
    id_depto INT,
    FOREIGN KEY (id_depto) REFERENCES departamento(id_depto)
);

-- Tabela Turma (Depende de Disciplina e Professor)
CREATE TABLE turma(
    id_turma INT PRIMARY KEY,
    semestre VARCHAR(10) NOT NULL,
    id_disciplina INT,
    id_professor INT,
    FOREIGN KEY (id_disciplina) REFERENCES disciplina(id_disciplina),
    FOREIGN KEY (id_professor) REFERENCES professor(id_professor)
);

-- Tabela Historico (Depende de Aluno e Turma)
CREATE TABLE historico(
    id_historico INT PRIMARY KEY,
    nota DECIMAL(4,2) NOT NULL,
    frequencia_pct DECIMAL(5,2) NOT NULL,
    situacao VARCHAR(20) NOT NULL,
    id_aluno INT,
    id_turma INT,
    FOREIGN KEY (id_aluno) REFERENCES aluno(id_aluno),
    FOREIGN KEY (id_turma) REFERENCES turma(id_turma)
);

DELIMITER $$
	DROP PROCEDURE IF EXISTS sp_usuario_criar $$
	CREATE PROCEDURE sp_usuario_criar (
		IN p_nome VARCHAR(100),
		IN p_email VARCHAR(100),
		IN p_senha_hash VARCHAR(255),
		IN p_role VARCHAR(20)  -- precisa ser VARCHAR, não ENUM
	)
	BEGIN
		INSERT INTO Usuarios (nome, email, senha_Hash, role, ativo, criado_Em)
		VALUES (p_nome, p_email, p_senha_hash, p_role, 1, NOW());
	END $$
DELIMITER ;

-- Exemplo de uso (ATENÇÃO: role deve ser 'Adm', não 'Admin')
CALL sp_usuario_criar(
    'João Admin',
    'joao@biblioteca.com',
    '$2a$11$Q91fiPYPec73pUA4DKByXeSNOZ6TYn2ZY5jWSWpr57rkfUEyKjWq2',
    'Admin'
);
select * from usuarios;


DELIMITER $$

DROP PROCEDURE IF EXISTS sp_usuario_obter_por_email $$
CREATE PROCEDURE sp_usuario_obter_por_email(IN p_email VARCHAR(100))
BEGIN
    SELECT id, nome, email, senha_hash, role, ativo
    FROM usuarios
    WHERE email = p_email
    LIMIT 1;
END $$

DELIMITER ;

SELECT * FROM Usuarios;
SELECT * FROM departamento;
SELECT * FROM professor;
SELECT * FROM aluno;
SELECT * FROM disciplina;
SELECT * FROM turma;
SELECT * FROM historico;