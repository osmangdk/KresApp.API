-- SystemSettings tablosu oluşturma
CREATE TABLE IF NOT EXISTS "SystemSettings" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "IsPreEnrollmentActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "PreEnrollmentStartDate" TIMESTAMP WITH TIME ZONE NULL,
    "PreEnrollmentEndDate" TIMESTAMP WITH TIME ZONE NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- İlk ayar satırını ekle (eğer yoksa)
INSERT INTO "SystemSettings" ("Id", "IsPreEnrollmentActive", "UpdatedAt")
SELECT uuid_generate_v4(), TRUE, NOW()
WHERE NOT EXISTS (SELECT 1 FROM "SystemSettings");
