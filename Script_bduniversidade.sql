-- ============================================================
--  BANCO DE DADOS: bduniversidade  (#20 – Universidade)
--  Categoria: Educação
-- ============================================================
CREATE DATABASE bduniversidade CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE bduniversidade;

-- ============================================================
--  TABELAS
-- ============================================================

-- Usuários do sistema (Reitor / aluno / professor)
CREATE TABLE Usuarios (
    id           INT           PRIMARY KEY AUTO_INCREMENT,
    nome         VARCHAR(100)  NOT NULL,
    email        VARCHAR(100)  NOT NULL UNIQUE,
    senha_hash   VARCHAR(255)  NOT NULL,
    role         ENUM('Reitor','Gerente','Aluno','Professor') NOT NULL DEFAULT 'Aluno',
    ativo        TINYINT(1)    NOT NULL DEFAULT 1,
    criado_em    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Departamento (independente)
CREATE TABLE departamento (
    id_depto  INT          PRIMARY KEY AUTO_INCREMENT,
    nome      VARCHAR(80)  NOT NULL,
    sigla     VARCHAR(10)  NOT NULL
);

-- Professor (depende de departamento)
CREATE TABLE professor (
    id_professor  INT          PRIMARY KEY AUTO_INCREMENT,
    nome          VARCHAR(100) NOT NULL,
    cpf           CHAR(11)     NOT NULL UNIQUE,
    titulacao     VARCHAR(30)  NOT NULL,
    id_depto      INT          NOT NULL,
    FOREIGN KEY (id_depto) REFERENCES departamento(id_depto)
);

-- Aluno (independente)
CREATE TABLE aluno (
    id_aluno       INT          PRIMARY KEY AUTO_INCREMENT,
    ra             VARCHAR(20)  NOT NULL UNIQUE,
    nome           VARCHAR(100) NOT NULL,
    cpf            CHAR(11)     NOT NULL UNIQUE,
    data_ingresso  DATE         NOT NULL
);

-- Disciplina (depende de departamento)
CREATE TABLE disciplina (
    id_disciplina  INT          PRIMARY KEY AUTO_INCREMENT,
    codigo         VARCHAR(10)  NOT NULL UNIQUE,
    nome           VARCHAR(100) NOT NULL,
    carga_horaria  INT          NOT NULL,
    id_depto       INT          NOT NULL,
    FOREIGN KEY (id_depto) REFERENCES departamento(id_depto)
);

-- Turma (depende de disciplina e professor)
CREATE TABLE turma (
    id_turma      INT         PRIMARY KEY AUTO_INCREMENT,
    semestre      VARCHAR(10) NOT NULL,
    id_disciplina INT         NOT NULL,
    id_professor  INT         NOT NULL,
    FOREIGN KEY (id_disciplina) REFERENCES disciplina(id_disciplina),
    FOREIGN KEY (id_professor)  REFERENCES professor(id_professor)
);

-- Histórico (depende de aluno e turma)
CREATE TABLE historico (
    id_historico   INT            PRIMARY KEY AUTO_INCREMENT,
    nota           DECIMAL(4,2)   NOT NULL,
    frequencia_pct DECIMAL(5,2)   NOT NULL,
    situacao       VARCHAR(20)    NOT NULL,
    id_aluno       INT            NOT NULL,
    id_turma       INT            NOT NULL,
    FOREIGN KEY (id_aluno) REFERENCES aluno(id_aluno),
    FOREIGN KEY (id_turma) REFERENCES turma(id_turma)
);

-- ============================================================
--  STORED PROCEDURES – USUÁRIOS
-- ============================================================
DELIMITER $$

DROP PROCEDURE IF EXISTS sp_usuario_criar $$
CREATE PROCEDURE sp_usuario_criar (
    IN p_nome VARCHAR(100),
    IN p_email VARCHAR(100),
    IN p_senha_hash VARCHAR(255),
    IN p_role VARCHAR(20)
)
BEGIN
    INSERT INTO Usuarios (nome, email, senha_hash, role, ativo, criado_em)
    VALUES (p_nome, p_email, p_senha_hash, p_role, 1, NOW());
END $$

DROP PROCEDURE IF EXISTS sp_usuario_listar $$
CREATE PROCEDURE sp_usuario_listar()
BEGIN
    SELECT id, nome, email, role, ativo, criado_em FROM Usuarios ORDER BY nome;
END $$

DROP PROCEDURE IF EXISTS sp_usuario_obter $$
CREATE PROCEDURE sp_usuario_obter(IN p_id INT)
BEGIN
    SELECT id, nome, email, senha_hash, role, ativo, criado_em FROM Usuarios WHERE id = p_id LIMIT 1;
END $$

DROP PROCEDURE IF EXISTS sp_usuario_obter_por_email $$
CREATE PROCEDURE sp_usuario_obter_por_email(IN p_email VARCHAR(100))
BEGIN
    SELECT id, nome, email, senha_hash, role, ativo FROM Usuarios WHERE email = p_email LIMIT 1;
END $$

DROP PROCEDURE IF EXISTS sp_usuario_editar $$
CREATE PROCEDURE sp_usuario_editar (
    IN p_id INT,
    IN p_nome VARCHAR(100),
    IN p_email VARCHAR(100),
    IN p_role VARCHAR(20),
    IN p_ativo TINYINT(1)
)
BEGIN
    UPDATE Usuarios SET nome = p_nome, email = p_email, role = p_role, ativo = p_ativo
    WHERE id = p_id;
END $$

DROP PROCEDURE IF EXISTS sp_usuario_excluir $$
CREATE PROCEDURE sp_usuario_excluir(IN p_id INT)
BEGIN
    DELETE FROM Usuarios WHERE id = p_id;
END $$

-- ============================================================
--  STORED PROCEDURES – DEPARTAMENTO
-- ============================================================
DROP PROCEDURE IF EXISTS sp_departamento_listar $$
CREATE PROCEDURE sp_departamento_listar()
BEGIN
    SELECT id_depto, nome, sigla FROM departamento ORDER BY nome;
END $$

DROP PROCEDURE IF EXISTS sp_departamento_obter $$
CREATE PROCEDURE sp_departamento_obter(IN p_id INT)
BEGIN
    SELECT id_depto, nome, sigla FROM departamento WHERE id_depto = p_id LIMIT 1;
END $$

DROP PROCEDURE IF EXISTS sp_departamento_criar $$
CREATE PROCEDURE sp_departamento_criar(
    IN p_nome  VARCHAR(80),
    IN p_sigla VARCHAR(10)
)
BEGIN
    INSERT INTO departamento (nome, sigla) VALUES (p_nome, p_sigla);
END $$

DROP PROCEDURE IF EXISTS sp_departamento_editar $$
CREATE PROCEDURE sp_departamento_editar(
    IN p_id    INT,
    IN p_nome  VARCHAR(80),
    IN p_sigla VARCHAR(10)
)
BEGIN
    UPDATE departamento SET nome = p_nome, sigla = p_sigla WHERE id_depto = p_id;
END $$

DROP PROCEDURE IF EXISTS sp_departamento_excluir $$
CREATE PROCEDURE sp_departamento_excluir(IN p_id INT)
BEGIN
    DELETE FROM departamento WHERE id_depto = p_id;
END $$

-- ============================================================
--  STORED PROCEDURES – PROFESSOR
-- ============================================================
DROP PROCEDURE IF EXISTS sp_professor_listar $$
CREATE PROCEDURE sp_professor_listar()
BEGIN
    SELECT p.id_professor, p.nome, p.cpf, p.titulacao,
           p.id_depto, d.nome AS depto_nome
    FROM professor p
    LEFT JOIN departamento d ON d.id_depto = p.id_depto
    ORDER BY p.nome;
END $$

DROP PROCEDURE IF EXISTS sp_professor_obter $$
CREATE PROCEDURE sp_professor_obter(IN p_id INT)
BEGIN
    SELECT p.id_professor, p.nome, p.cpf, p.titulacao,
           p.id_depto, d.nome AS depto_nome
    FROM professor p
    LEFT JOIN departamento d ON d.id_depto = p.id_depto
    WHERE p.id_professor = p_id LIMIT 1;
END $$

DROP PROCEDURE IF EXISTS sp_professor_criar $$
CREATE PROCEDURE sp_professor_criar(
    IN p_nome      VARCHAR(100),
    IN p_cpf       CHAR(11),
    IN p_titulacao VARCHAR(30),
    IN p_id_depto  INT
)
BEGIN
    INSERT INTO professor (nome, cpf, titulacao, id_depto)
    VALUES (p_nome, p_cpf, p_titulacao, p_id_depto);
END $$

DROP PROCEDURE IF EXISTS sp_professor_editar $$
CREATE PROCEDURE sp_professor_editar(
    IN p_id        INT,
    IN p_nome      VARCHAR(100),
    IN p_cpf       CHAR(11),
    IN p_titulacao VARCHAR(30),
    IN p_id_depto  INT
)
BEGIN
    UPDATE professor SET nome = p_nome, cpf = p_cpf, titulacao = p_titulacao,
                         id_depto = p_id_depto
    WHERE id_professor = p_id;
END $$

DROP PROCEDURE IF EXISTS sp_professor_excluir $$
CREATE PROCEDURE sp_professor_excluir(IN p_id INT)
BEGIN
    DELETE FROM professor WHERE id_professor = p_id;
END $$

-- ============================================================
--  STORED PROCEDURES – ALUNO
-- ============================================================
DROP PROCEDURE IF EXISTS sp_aluno_listar $$
CREATE PROCEDURE sp_aluno_listar()
BEGIN
    SELECT id_aluno, ra, nome, cpf, data_ingresso FROM aluno ORDER BY nome;
END $$

DROP PROCEDURE IF EXISTS sp_aluno_obter $$
CREATE PROCEDURE sp_aluno_obter(IN p_id INT)
BEGIN
    SELECT id_aluno, ra, nome, cpf, data_ingresso FROM aluno WHERE id_aluno = p_id LIMIT 1;
END $$

DROP PROCEDURE IF EXISTS sp_aluno_criar $$
CREATE PROCEDURE sp_aluno_criar(
    IN p_ra            VARCHAR(20),
    IN p_nome          VARCHAR(100),
    IN p_cpf           CHAR(11),
    IN p_data_ingresso DATE
)
BEGIN
    INSERT INTO aluno (ra, nome, cpf, data_ingresso)
    VALUES (p_ra, p_nome, p_cpf, p_data_ingresso);
END $$

DROP PROCEDURE IF EXISTS sp_aluno_editar $$
CREATE PROCEDURE sp_aluno_editar(
    IN p_id            INT,
    IN p_ra            VARCHAR(20),
    IN p_nome          VARCHAR(100),
    IN p_cpf           CHAR(11),
    IN p_data_ingresso DATE
)
BEGIN
    UPDATE aluno SET ra = p_ra, nome = p_nome, cpf = p_cpf,
                     data_ingresso = p_data_ingresso
    WHERE id_aluno = p_id;
END $$

DROP PROCEDURE IF EXISTS sp_aluno_excluir $$
CREATE PROCEDURE sp_aluno_excluir(IN p_id INT)
BEGIN
    DELETE FROM aluno WHERE id_aluno = p_id;
END $$

-- ============================================================
--  STORED PROCEDURES – DISCIPLINA
-- ============================================================
DROP PROCEDURE IF EXISTS sp_disciplina_listar $$
CREATE PROCEDURE sp_disciplina_listar()
BEGIN
    SELECT d.id_disciplina, d.codigo, d.nome, d.carga_horaria,
           d.id_depto, dep.nome AS depto_nome
    FROM disciplina d
    LEFT JOIN departamento dep ON dep.id_depto = d.id_depto
    ORDER BY d.nome;
END $$

DROP PROCEDURE IF EXISTS sp_disciplina_obter $$
CREATE PROCEDURE sp_disciplina_obter(IN p_id INT)
BEGIN
    SELECT d.id_disciplina, d.codigo, d.nome, d.carga_horaria,
           d.id_depto, dep.nome AS depto_nome
    FROM disciplina d
    LEFT JOIN departamento dep ON dep.id_depto = d.id_depto
    WHERE d.id_disciplina = p_id LIMIT 1;
END $$

DROP PROCEDURE IF EXISTS sp_disciplina_criar $$
CREATE PROCEDURE sp_disciplina_criar(
    IN p_codigo        VARCHAR(10),
    IN p_nome          VARCHAR(100),
    IN p_carga_horaria INT,
    IN p_id_depto      INT
)
BEGIN
    INSERT INTO disciplina (codigo, nome, carga_horaria, id_depto)
    VALUES (p_codigo, p_nome, p_carga_horaria, p_id_depto);
END $$

DROP PROCEDURE IF EXISTS sp_disciplina_editar $$
CREATE PROCEDURE sp_disciplina_editar(
    IN p_id            INT,
    IN p_codigo        VARCHAR(10),
    IN p_nome          VARCHAR(100),
    IN p_carga_horaria INT,
    IN p_id_depto      INT
)
BEGIN
    UPDATE disciplina SET codigo = p_codigo, nome = p_nome,
                          carga_horaria = p_carga_horaria, id_depto = p_id_depto
    WHERE id_disciplina = p_id;
END $$

DROP PROCEDURE IF EXISTS sp_disciplina_excluir $$
CREATE PROCEDURE sp_disciplina_excluir(IN p_id INT)
BEGIN
    DELETE FROM disciplina WHERE id_disciplina = p_id;
END $$

-- ============================================================
--  STORED PROCEDURES – TURMA
-- ============================================================
DROP PROCEDURE IF EXISTS sp_turma_listar $$
CREATE PROCEDURE sp_turma_listar()
BEGIN
    SELECT t.id_turma, t.semestre,
           t.id_disciplina, d.nome AS disciplina_nome, d.codigo AS disciplina_codigo,
           t.id_professor, p.nome AS professor_nome
    FROM turma t
    LEFT JOIN disciplina d ON d.id_disciplina = t.id_disciplina
    LEFT JOIN professor  p ON p.id_professor  = t.id_professor
    ORDER BY t.semestre DESC, d.nome;
END $$

DROP PROCEDURE IF EXISTS sp_turma_obter $$
CREATE PROCEDURE sp_turma_obter(IN p_id INT)
BEGIN
    SELECT t.id_turma, t.semestre,
           t.id_disciplina, d.nome AS disciplina_nome, d.codigo AS disciplina_codigo,
           t.id_professor, p.nome AS professor_nome
    FROM turma t
    LEFT JOIN disciplina d ON d.id_disciplina = t.id_disciplina
    LEFT JOIN professor  p ON p.id_professor  = t.id_professor
    WHERE t.id_turma = p_id LIMIT 1;
END $$

DROP PROCEDURE IF EXISTS sp_turma_criar $$
CREATE PROCEDURE sp_turma_criar(
    IN p_semestre      VARCHAR(10),
    IN p_id_disciplina INT,
    IN p_id_professor  INT
)
BEGIN
    INSERT INTO turma (semestre, id_disciplina, id_professor)
    VALUES (p_semestre, p_id_disciplina, p_id_professor);
END $$

DROP PROCEDURE IF EXISTS sp_turma_editar $$
CREATE PROCEDURE sp_turma_editar(
    IN p_id            INT,
    IN p_semestre      VARCHAR(10),
    IN p_id_disciplina INT,
    IN p_id_professor  INT
)
BEGIN
    UPDATE turma SET semestre = p_semestre, id_disciplina = p_id_disciplina,
                     id_professor = p_id_professor
    WHERE id_turma = p_id;
END $$

DROP PROCEDURE IF EXISTS sp_turma_excluir $$
CREATE PROCEDURE sp_turma_excluir(IN p_id INT)
BEGIN
    DELETE FROM turma WHERE id_turma = p_id;
END $$

-- ============================================================
--  STORED PROCEDURES – HISTÓRICO
-- ============================================================
DROP PROCEDURE IF EXISTS sp_historico_listar $$
CREATE PROCEDURE sp_historico_listar()
BEGIN
    SELECT h.id_historico, h.nota, h.frequencia_pct, h.situacao,
           h.id_aluno, a.nome AS aluno_nome, a.ra,
           h.id_turma, t.semestre,
           d.nome AS disciplina_nome
    FROM historico h
    LEFT JOIN aluno     a ON a.id_aluno       = h.id_aluno
    LEFT JOIN turma     t ON t.id_turma        = h.id_turma
    LEFT JOIN disciplina d ON d.id_disciplina  = t.id_disciplina
    ORDER BY a.nome, t.semestre;
END $$

DROP PROCEDURE IF EXISTS sp_historico_obter $$
CREATE PROCEDURE sp_historico_obter(IN p_id INT)
BEGIN
    SELECT h.id_historico, h.nota, h.frequencia_pct, h.situacao,
           h.id_aluno, a.nome AS aluno_nome, a.ra,
           h.id_turma, t.semestre,
           d.nome AS disciplina_nome
    FROM historico h
    LEFT JOIN aluno      a ON a.id_aluno      = h.id_aluno
    LEFT JOIN turma      t ON t.id_turma      = h.id_turma
    LEFT JOIN disciplina d ON d.id_disciplina = t.id_disciplina
    WHERE h.id_historico = p_id LIMIT 1;
END $$

DROP PROCEDURE IF EXISTS sp_historico_criar $$
CREATE PROCEDURE sp_historico_criar(
    IN p_nota DECIMAL(4,2),
    IN p_frequencia_pct DECIMAL(5,2),
    IN p_situacao VARCHAR(20),
    IN p_id_aluno INT,
    IN p_id_turma INT
)
BEGIN
    INSERT INTO historico (nota, frequencia_pct, situacao, id_aluno, id_turma)
    VALUES (p_nota, p_frequencia_pct, p_situacao, p_id_aluno, p_id_turma);
END $$

DROP PROCEDURE IF EXISTS sp_historico_editar $$
CREATE PROCEDURE sp_historico_editar(
    IN p_id INT,
    IN p_nota DECIMAL(4,2),
    IN p_frequencia_pct DECIMAL(5,2),
    IN p_situacao VARCHAR(20),
    IN p_id_aluno INT,
    IN p_id_turma INT
)
BEGIN
	UPDATE historico SET nota = p_nota, frequencia_pct = p_frequencia_pct,
                         situacao = p_situacao, id_aluno = p_id_aluno, id_turma = p_id_turma
    WHERE id_historico = p_id;
END $$

DROP PROCEDURE IF EXISTS sp_historico_excluir $$
CREATE PROCEDURE sp_historico_excluir(IN p_id INT)
BEGIN
    DELETE FROM historico WHERE id_historico = p_id;
END $$

DELIMITER ;

-- ============================================================
--  USUÁRIO ADMINISTRADOR PADRÃO  (senha: admin123)
--  Hash BCrypt gerado com work factor 11
-- ============================================================
CALL sp_usuario_criar(
    'Reitor',
    'reitor@universidade.com',
    '$2a$11$GjhjthUjYDusvIr01QLr6e/4hsIKJHWNmOcNgxKUoKHf0u0kTr4te',
    'Reitor'
);
 
-- Verificar se atualizou
SELECT id, nome, email, role, ativo, criado_em FROM Usuarios;

-- ============================================================
--  DADOS DE EXEMPLO
-- ============================================================
INSERT INTO departamento (nome, sigla) VALUES
    ('Engenharia de Computação', 'ENG'),
    ('Medicina', 'MED'),
    ('Direito', 'DIR'),
    ('Administração', 'ADM');

INSERT INTO professor (nome, cpf, titulacao, id_depto) VALUES
    ('Carlos Mendes',   '12345678901', 'Doutor',  1),
    ('Ana Paula Lima',  '98765432100', 'Mestre',  1),
    ('Roberto Souza',   '11122233344', 'Doutor',  2),
    ('Maria Fernandes', '55566677788', 'Mestre',  4);

INSERT INTO aluno (ra, nome, cpf, data_ingresso) VALUES
    ('RA20230001', 'João Silva',       '00011122233', '2023-02-01'),
    ('RA20230002', 'Fernanda Oliveira','00044455566', '2023-02-01'),
    ('RA20240001', 'Lucas Pereira',    '00077788899', '2024-02-01');

INSERT INTO disciplina (codigo, nome, carga_horaria, id_depto) VALUES
    ('CC001', 'Algoritmos',              60, 1),
    ('CC002', 'Banco de Dados',          80, 1),
    ('AD001', 'Administração Geral',     60, 4),
    ('ME001', 'Anatomia Humana',         80, 2);

INSERT INTO turma (semestre, id_disciplina, id_professor) VALUES
    ('2024.1', 1, 1),
    ('2024.1', 2, 2),
    ('2024.2', 3, 4);

INSERT INTO historico (nota, frequencia_pct, situacao, id_aluno, id_turma) VALUES
    (8.50, 90.00, 'Aprovado',   1, 1),
    (4.00, 65.00, 'Reprovado',  2, 1),
    (7.00, 85.00, 'Aprovado',   1, 2),
    (9.50, 95.00, 'Aprovado',   3, 3);

-- verificação final
SELECT * FROM Usuarios;