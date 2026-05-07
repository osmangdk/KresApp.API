-- ============================================================
-- KresApp PostgreSQL Veritabanı Oluşturma Scripti
-- KresApp.Domain.Entities kaynak alınarak üretilmiştir.
--
-- NOT: Enum kolonlar INTEGER olarak tutulmaktadır.
--      EF Core enum'ları varsayılan olarak integer (0,1,2...)
--      şeklinde kaydeder. Değer karşılıkları aşağıdaki
--      yorum satırlarında belirtilmiştir.
-- ============================================================

-- Uzantılar
CREATE EXTENSION IF NOT EXISTS "pgcrypto";  -- gen_random_uuid() için


-- ============================================================
-- ENUM DEĞER REFERANSLARı (yorum — DB'de tür tanımı yok)
-- ============================================================
-- UserRole          : 0=Admin,    1=Teacher,   2=Parent
-- AttendanceStatus  : 0=Present,  1=Absent,    2=Late
-- MealStatus        : 0=AteAll,   1=AteHalf,   2=DidNotEat
-- MoodStatus        : 0=Excellent,1=Good,      2=Normal,    3=Sad
-- PaymentStatus     : 0=Paid,     1=Pending,   2=Overdue
-- AnnouncementCategory: 0=Genel, 1=Etkinlik,  2=Tatil,     3=Acil


-- ============================================================
-- TABLOLAR
-- ============================================================

-- ----------------------------------------------------------
-- "Users"  (User entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Users" (
    "Id"           UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Email"        VARCHAR(256) NOT NULL UNIQUE,
    "PasswordHash" TEXT         NOT NULL,
    "Role"         INTEGER      NOT NULL,  -- UserRole: 0=Admin, 1=Teacher, 2=Parent
    "Name"         VARCHAR(256) NOT NULL DEFAULT '',
    "Phone"        VARCHAR(50)
);

-- ----------------------------------------------------------
-- "Classes"  (Class entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Classes" (
    "Id"        UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Name"      VARCHAR(256) NOT NULL,
    "TeacherId" UUID         NOT NULL REFERENCES "Users"("Id")
);

-- ----------------------------------------------------------
-- "Children"  (Child entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Children" (
    "Id"             UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Name"           VARCHAR(256) NOT NULL,
    "BirthDate"      DATE,
    "ParentId"       UUID         NOT NULL REFERENCES "Users"("Id"),
    "ClassId"        UUID         NOT NULL REFERENCES "Classes"("Id"),
    "CreatedAt"      TIMESTAMPTZ  NOT NULL DEFAULT now(),
    "BloodType"      VARCHAR(20)  NOT NULL DEFAULT 'Tamamlanmadı',
    "MedicalNotes"   TEXT,
    "ParentName"     VARCHAR(256) NOT NULL DEFAULT '',
    "ParentPhone"    VARCHAR(50)  NOT NULL DEFAULT '',
    "SecondaryPhone" VARCHAR(50),
    "EnrollmentDate" DATE         NOT NULL DEFAULT CURRENT_DATE
);

-- ----------------------------------------------------------
-- "ChildAllergies"  (ChildAllergy entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "ChildAllergies" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "ChildId"     UUID         NOT NULL REFERENCES "Children"("Id") ON DELETE CASCADE,
    "AllergyName" VARCHAR(256) NOT NULL,
    "Severity"    VARCHAR(50),         -- Hafif, Orta, Ağır
    "Notes"       TEXT,
    "CreatedAt"   TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "Attendances"  (Attendance entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Attendances" (
    "Id"        UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "ChildId"   UUID        NOT NULL REFERENCES "Children"("Id") ON DELETE CASCADE,
    "Date"      DATE        NOT NULL,
    "Status"    INTEGER     NOT NULL,  -- AttendanceStatus: 0=Present, 1=Absent, 2=Late
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT now(),
    UNIQUE ("ChildId", "Date")
);

-- ----------------------------------------------------------
-- "DailyReports"  (DailyReport entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "DailyReports" (
    "Id"            UUID        NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "ChildId"       UUID        NOT NULL REFERENCES "Children"("Id") ON DELETE CASCADE,
    "Date"          DATE        NOT NULL,
    "Mood"          INTEGER     NOT NULL,  -- MoodStatus: 0=Excellent,1=Good,2=Normal,3=Sad
    "MorningMeal"   INTEGER     NOT NULL,  -- MealStatus: 0=AteAll,1=AteHalf,2=DidNotEat
    "LunchMeal"     INTEGER     NOT NULL,  -- MealStatus: 0=AteAll,1=AteHalf,2=DidNotEat
    "AfternoonMeal" INTEGER     NOT NULL,  -- MealStatus: 0=AteAll,1=AteHalf,2=DidNotEat
    "DidSleep"      BOOLEAN     NOT NULL DEFAULT FALSE,
    "SleepHours"    SMALLINT    NOT NULL DEFAULT 0,
    "SleepMins"     SMALLINT    NOT NULL DEFAULT 0,
    "ToiletCount"   SMALLINT    NOT NULL DEFAULT 0,
    "Activities"    TEXT[]      NOT NULL DEFAULT '{}',
    "TeacherNote"   TEXT,
    "CreatedAt"     TIMESTAMPTZ NOT NULL DEFAULT now(),
    UNIQUE ("ChildId", "Date")
);

-- ----------------------------------------------------------
-- "Payments"  (Payment entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Payments" (
    "Id"        UUID           NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "ChildId"   UUID           NOT NULL REFERENCES "Children"("Id") ON DELETE CASCADE,
    "Amount"    NUMERIC(18, 2) NOT NULL,
    "Month"     SMALLINT       NOT NULL CHECK ("Month" BETWEEN 1 AND 12),
    "Year"      SMALLINT       NOT NULL CHECK ("Year" >= 2000),
    "Status"    INTEGER        NOT NULL,  -- PaymentStatus: 0=Paid, 1=Pending, 2=Overdue
    "DueDate"   TIMESTAMPTZ    NOT NULL,
    "CreatedAt" TIMESTAMPTZ    NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "Announcements"  (Announcement entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Announcements" (
    "Id"        UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Title"     VARCHAR(512) NOT NULL,
    "Body"      TEXT         NOT NULL,
    "Category"  INTEGER      NOT NULL,  -- AnnouncementCategory: 0=Genel,1=Etkinlik,2=Tatil,3=Acil
    "Emoji"     VARCHAR(20)  NOT NULL,
    "Date"      TIMESTAMPTZ  NOT NULL,
    "CreatedAt" TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "MealMenus"  (MealMenu entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "MealMenus" (
    "Id"        UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Day"       VARCHAR(20)  NOT NULL,   -- Pazartesi, Salı vs.
    "Breakfast" TEXT         NOT NULL,
    "Lunch"     TEXT         NOT NULL,
    "Snack"     TEXT         NOT NULL,
    "CreatedAt" TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "Schedules"  (Schedule entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Schedules" (
    "Id"        UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Day"       VARCHAR(20)  NOT NULL,   -- Pazartesi, Salı vs.
    "Subject"   VARCHAR(256) NOT NULL,
    "StartTime" VARCHAR(10)  NOT NULL,   -- 09:00 vs.
    "EndTime"   VARCHAR(10)  NOT NULL,
    "Teacher"   VARCHAR(256) NOT NULL,
    "CreatedAt" TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "Conversations"  (Conversation entity)
-- ParticipantIds dizisi UUID[] olarak tutulur.
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Conversations" (
    "Id"              UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Title"           VARCHAR(256),                 -- NULL ise birebir konuşma
    "IsGroup"         BOOLEAN      NOT NULL DEFAULT FALSE,
    "ParticipantIds"  UUID[]       NOT NULL DEFAULT '{}',
    "LastMessage"     TEXT,
    "LastMessageTime" TIMESTAMPTZ,
    "CreatedAt"       TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "Messages"  (Message entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Messages" (
    "Id"             UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "ConversationId" UUID         NOT NULL REFERENCES "Conversations"("Id") ON DELETE CASCADE,
    "SenderId"       UUID         NOT NULL REFERENCES "Users"("Id"),
    "SenderName"     VARCHAR(256) NOT NULL,
    "Content"        TEXT         NOT NULL,
    "IsRead"         BOOLEAN      NOT NULL DEFAULT FALSE,
    "CreatedAt"      TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "SchoolBellRequests"  (SchoolBellRequest entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "SchoolBellRequests" (
    "Id"            UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "ChildId"       UUID         NOT NULL REFERENCES "Children"("Id") ON DELETE CASCADE,
    "ParentId"      UUID         NOT NULL REFERENCES "Users"("Id"),
    "Type"          INTEGER      NOT NULL,  -- SchoolBellType: 0=DropOff, 1=PickUp
    "Status"        INTEGER      NOT NULL,  -- SchoolBellStatus: 0=Pending, 1=Completed, 2=Cancelled
    "Note"          TEXT,
    "EstimatedTime" TIMESTAMPTZ  NOT NULL,
    "CreatedAt"     TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "LearningOutcomes"  (Kazanımlar entity)
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "LearningOutcomes" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Month"       SMALLINT     NOT NULL CHECK ("Month" BETWEEN 1 AND 12),
    "Year"        SMALLINT     NOT NULL CHECK ("Year" >= 2000),
    "Theme"       VARCHAR(512) NOT NULL,
    "Outcomes"    JSONB        NOT NULL DEFAULT '[]',
    "Description" TEXT,
    "CreatedAt"   TIMESTAMPTZ  NOT NULL DEFAULT now(),
    UNIQUE ("Month", "Year")
);

-- ----------------------------------------------------------
-- "Medications"
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "Medications" (
    "Id"          UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "ChildId"     UUID         NOT NULL REFERENCES "Children"("Id") ON DELETE CASCADE,
    "Name"        VARCHAR(256) NOT NULL,
    "Dosage"      VARCHAR(128) NOT NULL,
    "Times"       JSONB        NOT NULL DEFAULT '[]',
    "Note"        TEXT,
    "StartDate"   DATE         NOT NULL,
    "EndDate"     DATE,
    "IsActive"    BOOLEAN      NOT NULL DEFAULT TRUE,
    "CreatedAt"   TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ----------------------------------------------------------
-- "MedicationLogs"
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "MedicationLogs" (
    "Id"           UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "MedicationId" UUID         NOT NULL REFERENCES "Medications"("Id") ON DELETE CASCADE,
    "Date"         DATE         NOT NULL,
    "Time"         VARCHAR(16)  NOT NULL,
    "IsGiven"      BOOLEAN      NOT NULL DEFAULT FALSE,
    "GivenByUserId" UUID        REFERENCES "Users"("Id"),
    "GivenAt"      TIMESTAMPTZ
);

-- ----------------------------------------------------------
-- "GalleryItems"
-- ----------------------------------------------------------
CREATE TABLE IF NOT EXISTS "GalleryItems" (
    "Id"              UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "Title"           VARCHAR(256) NOT NULL,
    "Description"     TEXT,
    "Url"             TEXT         NOT NULL,
    "ThumbnailUrl"    TEXT,
    "Type"            INTEGER      NOT NULL, -- 0: Photo, 1: Video
    "ClassId"         UUID         REFERENCES "Classes"("Id") ON DELETE CASCADE,
    "ChildId"         UUID         REFERENCES "Children"("Id") ON DELETE CASCADE,
    "CreatedByUserId" UUID         NOT NULL REFERENCES "Users"("Id"),
    "CreatedAt"       TIMESTAMPTZ  NOT NULL DEFAULT now()
);


-- ============================================================
-- YARDIMCI İNDEKSLER
-- ============================================================

-- Children
CREATE INDEX IF NOT EXISTS ix_children_parentid ON "Children"("ParentId");
CREATE INDEX IF NOT EXISTS ix_children_classid  ON "Children"("ClassId");

-- Attendances
CREATE INDEX IF NOT EXISTS ix_attendances_childid_date ON "Attendances"("ChildId", "Date");

-- DailyReports
CREATE INDEX IF NOT EXISTS ix_dailyreports_childid_date ON "DailyReports"("ChildId", "Date");

-- Payments
CREATE INDEX IF NOT EXISTS ix_payments_childid ON "Payments"("ChildId");

-- Messages
CREATE INDEX IF NOT EXISTS ix_messages_conversationid ON "Messages"("ConversationId");
CREATE INDEX IF NOT EXISTS ix_messages_senderid       ON "Messages"("SenderId");

-- ChildAllergies
CREATE INDEX IF NOT EXISTS ix_childallergies_childid ON "ChildAllergies"("ChildId");

-- Conversations (GIN ile UUID[] araması)
CREATE INDEX IF NOT EXISTS ix_conversations_participantids ON "Conversations" USING GIN ("ParticipantIds");

-- SchoolBellRequests
CREATE INDEX IF NOT EXISTS ix_schoolbellrequests_parentid ON "SchoolBellRequests"("ParentId");
CREATE INDEX IF NOT EXISTS ix_schoolbellrequests_childid  ON "SchoolBellRequests"("ChildId");

-- LearningOutcomes
CREATE INDEX IF NOT EXISTS ix_learningoutcomes_year_month ON "LearningOutcomes"("Year", "Month");

-- Medications
CREATE INDEX IF NOT EXISTS ix_medications_childid ON "Medications"("ChildId");
CREATE INDEX IF NOT EXISTS ix_medicationlogs_medicationid ON "MedicationLogs"("MedicationId");
CREATE INDEX IF NOT EXISTS ix_medicationlogs_date ON "MedicationLogs"("Date");

-- GalleryItems
CREATE INDEX IF NOT EXISTS ix_galleryitems_classid ON "GalleryItems"("ClassId");
CREATE INDEX IF NOT EXISTS ix_galleryitems_childid ON "GalleryItems"("ChildId");

-- ============================================================
-- ÖRNEK VERİ (MOCK DATA) EKLENMESİ
-- ============================================================

-- 1. Users (Kullanıcılar)
-- Role: 0=Admin, 1=Teacher, 2=Parent
INSERT INTO "Users" ("Id", "Email", "PasswordHash", "Role", "Name", "Phone") VALUES
('11111111-1111-1111-1111-111111111111', 'admin@aile.gov.tr', '$2a$11$qJHgNN6EXe.7X8vbJgh4w.9lj6BSISWp8nHsZ31lPw5XsreS9JEz6', 0, 'Admin Kullanıcısı', '05550000000'),
('22222222-2222-2222-2222-222222222222', 'teacher1@aile.gov.tr', '$2a$11$qJHgNN6EXe.7X8vbJgh4w.9lj6BSISWp8nHsZ31lPw5XsreS9JEz6', 1, 'Ayşe Öğretmen', '05551111111'),
('33333333-3333-3333-3333-333333333333', 'teacher2@aile.gov.tr', '$2a$11$qJHgNN6EXe.7X8vbJgh4w.9lj6BSISWp8nHsZ31lPw5XsreS9JEz6', 1, 'Fatma Öğretmen', '05552222222'),
('44444444-4444-4444-4444-444444444444', 'parent1@aile.gov.tr', '$2a$11$qJHgNN6EXe.7X8vbJgh4w.9lj6BSISWp8nHsZ31lPw5XsreS9JEz6', 2, 'Ahmet Veli', '05553333333'),
('55555555-5555-5555-5555-555555555555', 'parent2@aile.gov.tr', '$2a$11$qJHgNN6EXe.7X8vbJgh4w.9lj6BSISWp8nHsZ31lPw5XsreS9JEz6', 2, 'Zeynep Veli', '05554444444')
ON CONFLICT ("Id") DO UPDATE SET "PasswordHash" = EXCLUDED."PasswordHash";

-- 2. Classes (Sınıflar)
INSERT INTO "Classes" ("Id", "Name", "TeacherId") VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Papatyalar Sınıfı', '22222222-2222-2222-2222-222222222222'),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Yıldızlar Sınıfı', '33333333-3333-3333-3333-333333333333')
ON CONFLICT ("Id") DO NOTHING;

-- 3. Children (Çocuklar)
INSERT INTO "Children" ("Id", "Name", "BirthDate", "ParentId", "ClassId", "BloodType", "MedicalNotes", "ParentName", "ParentPhone", "SecondaryPhone", "EnrollmentDate") VALUES
('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Ali Veli', '2019-05-10', '44444444-4444-4444-4444-444444444444', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'A+', 'Astım', 'Ahmet Veli', '05553333333', '05559999999', '2023-09-01'),
('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Ayşe Veli', '2020-02-15', '55555555-5555-5555-5555-555555555555', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '0-', 'Yok', 'Zeynep Veli', '05554444444', NULL, '2023-09-01')
ON CONFLICT ("Id") DO NOTHING;

-- 4. ChildAllergies (Alerjiler)
INSERT INTO "ChildAllergies" ("Id", "ChildId", "AllergyName", "Severity", "Notes") VALUES
('f1111111-1111-1111-1111-111111111111', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'Fıstık', 'Ağır', 'Kesinlikle fıstık içeren gıdalar verilmemeli.'),
('f2222222-2222-2222-2222-222222222222', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Toz', 'Hafif', 'Sınıf temizliğine dikkat edilmeli.')
ON CONFLICT ("Id") DO NOTHING;

-- 5. Attendances (Yoklamalar)
-- Status: 0=Present, 1=Absent, 2=Late
INSERT INTO "Attendances" ("Id", "ChildId", "Date", "Status") VALUES
('a1111111-1111-1111-1111-111111111111', 'cccccccc-cccc-cccc-cccc-cccccccccccc', CURRENT_DATE, 0),
('a2222222-2222-2222-2222-222222222222', 'dddddddd-dddd-dddd-dddd-dddddddddddd', CURRENT_DATE, 1)
ON CONFLICT ("Id") DO NOTHING;

-- 6. DailyReports (Günlük Raporlar)
-- Mood: 0=Excellent,1=Good,2=Normal,3=Sad
-- MealStatus: 0=AteAll,1=AteHalf,2=DidNotEat
INSERT INTO "DailyReports" ("Id", "ChildId", "Date", "Mood", "MorningMeal", "LunchMeal", "AfternoonMeal", "DidSleep", "SleepHours", "SleepMins", "ToiletCount", "Activities", "TeacherNote") VALUES
('d1111111-1111-1111-1111-111111111111', 'cccccccc-cccc-cccc-cccc-cccccccccccc', CURRENT_DATE, 0, 0, 1, 0, TRUE, 1, 30, 2, ARRAY['Boyama', 'Şarkı Söyleme'], 'Bugün çok enerjikti.'),
('d2222222-2222-2222-2222-222222222222', 'dddddddd-dddd-dddd-dddd-dddddddddddd', CURRENT_DATE - INTERVAL '1 day', 1, 1, 2, 0, FALSE, 0, 0, 1, ARRAY['Oyun Hamuru'], 'Biraz halsiz görünüyordu.')
ON CONFLICT ("Id") DO NOTHING;

-- 7. Payments (Ödemeler)
-- Status: 0=Paid, 1=Pending, 2=Overdue
INSERT INTO "Payments" ("Id", "ChildId", "Amount", "Month", "Year", "Status", "DueDate") VALUES
('61111111-1111-1111-1111-111111111111', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 5000.00, 5, 2024, 0, '2024-05-05'),
('62222222-2222-2222-2222-222222222222', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 5000.00, 5, 2024, 1, '2024-05-05')
ON CONFLICT ("Id") DO NOTHING;

-- 8. Announcements (Duyurular)
-- Category: 0=Genel, 1=Etkinlik, 2=Tatil, 3=Acil
INSERT INTO "Announcements" ("Id", "Title", "Body", "Category", "Emoji", "Date") VALUES
('71111111-1111-1111-1111-111111111111', '23 Nisan Kutlaması', '23 Nisan gösterimiz okul bahçesinde yapılacaktır.', 1, '🎉', CURRENT_TIMESTAMP),
('72222222-2222-2222-2222-222222222222', 'Yaz Tatili', 'Okulumuz temmuz ayında tatile girecektir.', 2, '🏖️', CURRENT_TIMESTAMP)
ON CONFLICT ("Id") DO NOTHING;

-- 9. MealMenus (Yemek Menüleri)
INSERT INTO "MealMenus" ("Id", "Day", "Breakfast", "Lunch", "Snack") VALUES
('81111111-1111-1111-1111-111111111111', 'Pazartesi', 'Haşlanmış yumurta, peynir, zeytin, süt', 'Mercimek çorbası, köfte, makarna', 'Meyve salatası'),
('82222222-2222-2222-2222-222222222222', 'Salı', 'Omlet, domates, salatalık, bitki çayı', 'Tavuk sote, pilav, yoğurt', 'Kek, süt')
ON CONFLICT ("Id") DO NOTHING;

-- 10. Schedules (Ders Programları)
INSERT INTO "Schedules" ("Id", "Day", "Subject", "StartTime", "EndTime", "Teacher") VALUES
('91111111-1111-1111-1111-111111111111', 'Pazartesi', 'Resim', '09:00', '09:45', 'Ayşe Öğretmen'),
('92222222-2222-2222-2222-222222222222', 'Pazartesi', 'Müzik', '10:00', '10:45', 'Fatma Öğretmen')
ON CONFLICT ("Id") DO NOTHING;

-- 11. Conversations (Mesajlaşmalar)
-- Group: ParticipantIds (Ayşe Öğretmen ve Ahmet Veli)
INSERT INTO "Conversations" ("Id", "Title", "IsGroup", "ParticipantIds", "LastMessage", "LastMessageTime") VALUES
('c1111111-1111-1111-1111-111111111111', NULL, FALSE, ARRAY['22222222-2222-2222-2222-222222222222'::uuid, '44444444-4444-4444-4444-444444444444'::uuid], 'Teşekkürler hocam.', CURRENT_TIMESTAMP)
ON CONFLICT ("Id") DO NOTHING;

-- 12. Messages (Mesajlar)
INSERT INTO "Messages" ("Id", "ConversationId", "SenderId", "SenderName", "Content", "IsRead", "CreatedAt") VALUES
('e1111111-1111-1111-1111-111111111111', 'c1111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', 'Ayşe Öğretmen', 'Ali bugün harikaydı!', TRUE, CURRENT_TIMESTAMP - INTERVAL '1 hour'),
('e2222222-2222-2222-2222-222222222222', 'c1111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444444', 'Ahmet Veli', 'Teşekkürler hocam.', FALSE, CURRENT_TIMESTAMP)
ON CONFLICT ("Id") DO NOTHING;

-- 13. LearningOutcomes (Kazanımlar)
INSERT INTO "LearningOutcomes" ("Id", "Month", "Year", "Theme", "Outcomes", "Description") VALUES
('l1111111-1111-1111-1111-111111111111', 4, 2026, 'Bahara Merhaba ve Tohumun Yolculuğu',
  '["Bitkilerin yaşam döngüsünü öğrenme", "Doğanın uyanışını gözlemleme", "Bahçe çalışmaları ve tohum ekimi", "Renklerin karışımı ve canlılık"]'::jsonb,
  'Nisan ayında çocuklarımız baharın gelişini kutlayarak doğa ile iç içe etkinlikler gerçekleştirecek.'),
('l2222222-2222-2222-2222-222222222222', 5, 2026, 'Meslekler ve Gökyüzü',
  '["Gelecekteki hayallerimiz", "Yıldızlar, ay ve güneş sistemi", "Astronotların dünyası", "Farklı meslekleri tanıma"]'::jsonb,
  'Mayıs ayında çocuklarımız meslek kavramlarını ve gökyüzünü keşfedecek.')
ON CONFLICT ("Month", "Year") DO NOTHING;

-- 14. Medications (İlaçlar)
-- Ali Yılmaz için ilaçlar
INSERT INTO "Medications" ("Id", "ChildId", "Name", "Dosage", "Times", "Note", "StartDate", "IsActive") VALUES
('m1111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'BALIK YAĞI (ŞURUP)', '1 Tatlı Kaşığı', '["09:30"]'::jsonb, 'Kahvaltı sonrası', CURRENT_DATE, TRUE),
('m2222222-2222-2222-2222-222222222222', '11111111-1111-1111-1111-111111111111', 'DEMİR TAKVİYESİ', '5 Damla', '["15:30"]'::jsonb, 'İkindiden hemen sonra', CURRENT_DATE, TRUE)
ON CONFLICT ("Id") DO NOTHING;

-- MedicationLogs (Örnek olarak bugün verilmiş bir ilaç)
INSERT INTO "MedicationLogs" ("Id", "MedicationId", "Date", "Time", "IsGiven", "GivenByUserId", "GivenAt") VALUES
('ml111111-1111-1111-1111-111111111111', 'm1111111-1111-1111-1111-111111111111', CURRENT_DATE, '09:30', TRUE, '22222222-2222-2222-2222-222222222222', CURRENT_TIMESTAMP)
ON CONFLICT ("Id") DO NOTHING;

-- 15. GalleryItems (Fotoğraf ve Videolar)
INSERT INTO "GalleryItems" ("Id", "Title", "Description", "Url", "Type", "ClassId", "CreatedByUserId") VALUES
('g1111111-1111-1111-1111-111111111111', 'Bugünkü Resim Atölyemiz', 'Çocuklar bugün sulu boya ile harikalar yarattı.', 'https://picsum.photos/800/600?random=1', 0, '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222'),
('g2222222-2222-2222-2222-222222222222', 'Müzik Dersimizden Kareler', 'Şarkılar söylerken çok eğlendik.', 'https://picsum.photos/800/600?random=2', 0, '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222'),
('g3333333-3333-3333-3333-333333333333', 'Bahçe Oyunları', 'Güneşin tadını çıkarıyoruz.', 'https://picsum.photos/800/600?random=3', 0, '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222')
ON CONFLICT ("Id") DO NOTHING;
