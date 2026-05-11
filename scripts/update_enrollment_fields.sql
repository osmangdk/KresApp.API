-- EnrollmentRequests tablosuna kurum ve eş bilgilerini ekleyen script
ALTER TABLE "EnrollmentRequests" 
    ADD COLUMN IF NOT EXISTS "ParentSicilNo"      text,
    ADD COLUMN IF NOT EXISTS "ParentUnit"         text,
    ADD COLUMN IF NOT EXISTS "ParentTitle"        text,
    ADD COLUMN IF NOT EXISTS "ParentServiceYears" integer,
    ADD COLUMN IF NOT EXISTS "SpouseIsAlive"      boolean DEFAULT true,
    ADD COLUMN IF NOT EXISTS "SpouseIsWorking"    boolean,
    ADD COLUMN IF NOT EXISTS "SpouseWorkplace"    text,
    ADD COLUMN IF NOT EXISTS "SpouseWorkplaceHasDaycare" boolean;

-- Opsiyonel: Mevcut kayıtlara varsayılan değer atamak isterseniz
-- UPDATE "EnrollmentRequests" SET "SpouseIsAlive" = true WHERE "SpouseIsAlive" IS NULL;
