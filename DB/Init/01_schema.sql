-- =========================
-- Database
-- =========================
CREATE DATABASE IF NOT EXISTS gamedb
  DEFAULT CHARACTER SET utf8mb4
  COLLATE utf8mb4_general_ci;

USE gamedb;

-- =========================
-- users
-- =========================
CREATE TABLE users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(50) NOT NULL,
  email VARCHAR(100) NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  salt VARCHAR(50) NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  last_login_at DATETIME NULL,
  status TINYINT NOT NULL DEFAULT 1 COMMENT '1-active, 2-suspended, 3-banned',

  UNIQUE KEY idx_username (username),
  UNIQUE KEY idx_email (email)
);

-- =========================
-- user_stats
-- =========================
CREATE TABLE user_stats (
  user_id INT PRIMARY KEY,
  level INT NOT NULL DEFAULT 1,
  exp INT NOT NULL DEFAULT 0,

  CONSTRAINT fk_user_stats_user
    FOREIGN KEY (user_id) REFERENCES users(id)
    ON DELETE CASCADE
);

-- =========================
-- user_profile
-- =========================
CREATE TABLE user_profiles (
  user_id INT PRIMARY KEY,
  nickname VARCHAR(50) NOT NULL,
  introduction TEXT,

  CONSTRAINT fk_user_profile_user
    FOREIGN KEY (user_id) REFERENCES users(id)
    ON DELETE CASCADE
);

-- =========================
-- user_resources
-- =========================
CREATE TABLE user_resources (
  user_id INT PRIMARY KEY,
  gold INT NOT NULL DEFAULT 0,
  diamond INT NOT NULL DEFAULT 0,

  CONSTRAINT fk_user_resources_user
    FOREIGN KEY (user_id) REFERENCES users(id)
    ON DELETE CASCADE
);

-- =========================
-- character_templates
-- =========================
CREATE TABLE character_templates (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(50) NOT NULL,
  description TEXT,
  rarity TINYINT NOT NULL DEFAULT 1 COMMENT '1-Common, 2-Rare, 3-Epic, 4-Legendary',
  base_hp INT NOT NULL,
  base_attack INT NOT NULL,
  base_defense INT NOT NULL
);

-- =========================
-- user_characters
-- =========================
CREATE TABLE user_characters (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  template_id INT NOT NULL,
  level INT NOT NULL DEFAULT 1,
  experience INT NOT NULL DEFAULT 0,
  stars INT NOT NULL DEFAULT 1,
  obtained_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

  CONSTRAINT fk_user_characters_user
    FOREIGN KEY (user_id) REFERENCES users(id)
    ON DELETE CASCADE,

  CONSTRAINT fk_user_characters_template
    FOREIGN KEY (template_id) REFERENCES character_templates(id)
);

-- =========================
-- gacha_masters
-- =========================
CREATE TABLE gacha_masters (
  id INT AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(50) NOT NULL,
  name VARCHAR(50) NOT NULL,
  description VARCHAR(255),
  cost_type INT NOT NULL,
  cost_amount INT NOT NULL,
  is_limited BOOLEAN NOT NULL,
  start_time DATETIME NOT NULL,
  end_time DATETIME NOT NULL
);

-- =========================
-- gacha_pools
-- =========================
CREATE TABLE gacha_pools (
  id INT AUTO_INCREMENT PRIMARY KEY,
  gacha_id INT NOT NULL,
  item_type INT NOT NULL,
  item_id INT NOT NULL,
  rarity INT NOT NULL,

  CONSTRAINT fk_gacha_pools_gacha
    FOREIGN KEY (gacha_id) REFERENCES gacha_masters(id)
    ON DELETE CASCADE
);

-- =========================
-- gacha_rarity_rates
-- =========================
CREATE TABLE gacha_rarity_rates (
  id INT AUTO_INCREMENT PRIMARY KEY,
  gacha_id INT NOT NULL,
  rarity INT NOT NULL,
  rate DOUBLE NOT NULL,

  CONSTRAINT fk_gacha_rarity_rates_gacha
    FOREIGN KEY (gacha_id) REFERENCES gacha_masters(id)
    ON DELETE CASCADE
);

-- =========================
-- decks
-- =========================
CREATE TABLE decks (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  deck_index INT NOT NULL,
  name VARCHAR(50) NOT NULL DEFAULT 'Default Deck',
  is_active BOOLEAN NOT NULL DEFAULT FALSE,

  CONSTRAINT fk_decks_user
    FOREIGN KEY (user_id) REFERENCES users(id)
    ON DELETE CASCADE,

  UNIQUE KEY idx_user_deck (user_id, deck_index)
);

-- =========================
-- deck_slots
-- =========================
CREATE TABLE deck_slots (
  id INT AUTO_INCREMENT PRIMARY KEY,
  deck_id INT NOT NULL,
  user_character_id INT,
  slot_order TINYINT NOT NULL,

  CONSTRAINT fk_deck_slots_deck
    FOREIGN KEY (deck_id) REFERENCES decks(id)
    ON DELETE CASCADE,

  CONSTRAINT fk_deck_slots_user_character
    FOREIGN KEY (user_character_id) REFERENCES user_characters(id),

  UNIQUE KEY idx_deck_slot_order (deck_id, slot_order)
);

-- =========================
-- enemy_templates
-- =========================
CREATE TABLE enemy_templates (
  id INT AUTO_INCREMENT PRIMARY KEY COMMENT '몬스터 고유 ID',
  name VARCHAR(50) NOT NULL COMMENT '몬스터 이름',
  description TEXT COMMENT '설명',
  level INT NOT NULL DEFAULT 1 COMMENT '몬스터 레벨',
  hp INT NOT NULL COMMENT '체력',
  attack INT NOT NULL COMMENT '공격력',
  defense INT NOT NULL COMMENT '방어력'
);

-- =========================
-- stages
-- =========================
CREATE TABLE stages (
  id INT AUTO_INCREMENT PRIMARY KEY,
  chapter INT NOT NULL COMMENT '챕터 번호',
  stage_number INT NOT NULL COMMENT '스테이지 번호',
  reward_gold INT NOT NULL,
  reward_exp INT NOT NULL,

  UNIQUE KEY idx_chapter_stage (chapter, stage_number)
);

-- =========================
-- stage_enemies
-- =========================
CREATE TABLE stage_enemies (
  id INT AUTO_INCREMENT PRIMARY KEY,
  stage_id INT NOT NULL,
  enemy_id INT NOT NULL,
  count INT NOT NULL DEFAULT 1 COMMENT '해당 적이 몇 마리 등장?',

  CONSTRAINT fk_stage_enemies_stage
    FOREIGN KEY (stage_id) REFERENCES stages(id)
    ON DELETE CASCADE,

  CONSTRAINT fk_stage_enemies_enemy
    FOREIGN KEY (enemy_id) REFERENCES enemy_templates(id)
);
