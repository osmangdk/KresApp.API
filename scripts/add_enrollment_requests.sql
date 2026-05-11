-- ============================================================
-- KresApp Veritabanı Güncelleme Scripti
-- Çalıştırma: pgAdmin Query Tool veya psql üzerinden
-- ============================================================

-- 1. EnrollmentRequests tablosunu oluştur (yoksa)
CREATE TABLE IF NOT EXISTS "EnrollmentRequests" (
    "Id"                 uuid        NOT NULL DEFAULT uuid_generate_v4(),
    "ParentName"         text        NOT NULL,
    "ParentEmail"        text        NOT NULL,
    "ParentPhone"        text        NOT NULL,
    "ParentTcKimlikNo"   text,
    "ParentPassword"     text        NOT NULL,
    "ChildFullName"      text        NOT NULL,
    "ChildBirthDate"     date,
    "ChildGender"        text,
    "ChildBloodType"     text,
    "ChildAllergies"     text,
    "ChildMedicalNotes"  text,
    "Notes"              text,
    "Status"             integer     NOT NULL DEFAULT 0,
    "AdminNote"          text,
    "CreatedAt"          timestamptz NOT NULL DEFAULT now(),
    "ReviewedAt"         timestamptz,
    "ReviewedByUserId"   uuid,
    CONSTRAINT "PK_EnrollmentRequests" PRIMARY KEY ("Id")
);

-- İndeksler
CREATE INDEX IF NOT EXISTS "IX_EnrollmentRequests_ParentEmail"
    ON "EnrollmentRequests" ("ParentEmail");

CREATE INDEX IF NOT EXISTS "IX_EnrollmentRequests_Status"
    ON "EnrollmentRequests" ("Status");

-- ============================================================
-- 2. Users tablosuna TcKimlikNo sütununu ekle (varsa hata vermez)
-- ============================================================
ALTER TABLE "Users"
    ADD COLUMN IF NOT EXISTS "TcKimlikNo" text;

-- ============================================================
-- Kontrol: Yapılan değişiklikleri doğrula
-- ============================================================
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'Users' AND column_name = 'TcKimlikNo';

SELECT table_name
FROM information_schema.tables
WHERE table_name = 'EnrollmentRequests';
