-- ============================================================
--  SCRIPT DE CORREÇÃO
--  1. Atualiza roles: Reitor, Gerente, Aluno, Professor
--  2. Corrige sp_historico_obter (faltava a.ra → erro no Editar/Excluir)
-- ============================================================
USE bduniversidade;

-- ============================================================
--  1. ALTERA A COLUNA ROLE (novos papéis)
-- ============================================================
ALTER TABLE Usuarios
    MODIFY COLUMN role ENUM('Reitor','Gerente','Aluno','Professor')
    NOT NULL DEFAULT 'Aluno';

-- Atualiza o usuário admin para Reitor
UPDATE Usuarios
SET role = 'Reitor'
WHERE email = 'admin@universidade.com';

-- (Opcional) Atualiza hash da senha para admin123 caso ainda esteja errado
UPDATE Usuarios
SET senha_hash = '$2a$11$GjhjthUjYDusvIr01QLr6e/4hsIKJHWNmOcNgxKUoKHf0u0kTr4te'
WHERE email = 'admin@universidade.com';

-- ============================================================
--  2. CORRIGE sp_historico_obter (adiciona a.ra que faltava)
-- ============================================================
DELIMITER $$

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

DELIMITER ;

-- ============================================================
--  VERIFICAÇÃO
-- ============================================================
SELECT id, nome, email, role, ativo FROM Usuarios;
