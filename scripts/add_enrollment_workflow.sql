-- ============================================================================
-- KresApp: Enrollment Workflow + AccountStatus Migration
-- Tarih: 2026-05-08
-- Açıklama: 3 aşamalı kayıt iş akışı için gerekli veritabanı değişiklikleri
-- ============================================================================

-- 1. EnrollmentRequests tablosuna puanlama ve kullanıcı bağlantısı alanları
ALTER TABLE "EnrollmentRequests"
    ADD COLUMN IF NOT EXISTS "Score"           integer,
    ADD COLUMN IF NOT EXISTS "ScoringNotes"    text,
    ADD COLUMN IF NOT EXISTS "CreatedUserId"   uuid;

-- 2. Users tablosuna hesap durumu alanı
-- DEFAULT 2 = Active (mevcut tüm kullanıcılar otomatik Active olur)
ALTER TABLE "Users"
    ADD COLUMN IF NOT EXISTS "AccountStatus"   integer NOT NULL DEFAULT 2;

-- ============================================================================
-- AccountStatus Değerleri:
--   0 = PendingDocuments   (Kesinleşmiş listede, evrak yüklenmesi bekleniyor)
--   1 = DocumentsSubmitted (Evraklar yüklendi, admin onayı bekleniyor)
--   2 = Active             (Tam erişim sağlandı)
--   3 = Suspended          (Hesap askıya alındı)
--
-- EnrollmentStatus Değerleri:
--   0 = Pending             (Ön başvuru yapıldı)
--   1 = Scored              (Admin puanladı)
--   2 = Finalized           (Kesinleşmiş listede, User oluşturuldu)
--   3 = DocumentsSubmitted  (Kesin kayıt evrakları yüklendi)
--   4 = Approved            (Admin onayladı, tam erişim sağlandı)
--   5 = Rejected            (Başvuru reddedildi)
-- ============================================================================
