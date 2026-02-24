ALTER TABLE gacha_masters
ADD soft_pity_threshold INT NOT NULL DEFAULT 0 COMMENT '점진적 확률 증가 시작 회차',
ADD hard_pity_threshold INT NOT NULL DEFAULT 0 COMMENT '100% 확정 지급 회차',
ADD pity_bonus_rate DOUBLE NOT NULL DEFAULT 0.0 COMMENT '1스택 당 증가 확률',
ADD pity_target_rarity INT NOT NULL DEFAULT 4 COMMENT '천장 대상 희귀도';

-- 2. 스택 관리를 위한 신규 테이블 생성
CREATE TABLE user_gacha_pities(
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  gacha_id INT NOT NULL,
  pity_stack INT NOT NULL DEFAULT 0,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

  CONSTRAINT uq_user_gacha UNIQUE (user_id, gacha_id),

  CONSTRAINT fk_user_gacha_pities_user 
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
        
  CONSTRAINT fk_user_gacha_pities_gacha 
    FOREIGN KEY (gacha_id) REFERENCES gacha_masters(id) ON DELETE CASCADE
);