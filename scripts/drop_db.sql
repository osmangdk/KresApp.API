-- ============================================================
-- KresApp PostgreSQL Tablo Silme Scripti
-- FK bağımlılık sırasına göre (bağımlı tablolar önce silinir)
-- ============================================================

DROP TABLE IF EXISTS "Messages"        CASCADE;
DROP TABLE IF EXISTS "Conversations"   CASCADE;
DROP TABLE IF EXISTS "Announcements"   CASCADE;
DROP TABLE IF EXISTS "Schedules"       CASCADE;
DROP TABLE IF EXISTS "MealMenus"       CASCADE;
DROP TABLE IF EXISTS "Payments"        CASCADE;
DROP TABLE IF EXISTS "DailyReports"    CASCADE;
DROP TABLE IF EXISTS "Attendances"     CASCADE;
DROP TABLE IF EXISTS "ChildAllergies"  CASCADE;
DROP TABLE IF EXISTS "Children"        CASCADE;
DROP TABLE IF EXISTS "Classes"         CASCADE;
DROP TABLE IF EXISTS "Users"           CASCADE;
