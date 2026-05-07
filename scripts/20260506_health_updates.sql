-- 1. Children tablosuna yeni sütunlar ekle
ALTER TABLE "Children" 
ADD COLUMN IF NOT EXISTS "Gender" TEXT,
ADD COLUMN IF NOT EXISTS "Weight" DOUBLE PRECISION,
ADD COLUMN IF NOT EXISTS "Height" DOUBLE PRECISION;

-- 2. Vaccinations (Aşılar) tablosu
CREATE TABLE IF NOT EXISTS "Vaccinations" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "ChildId" UUID NOT NULL REFERENCES "Children"("Id") ON DELETE CASCADE,
    "VaccineName" TEXT NOT NULL,
    "PlannedDate" DATE,
    "AppliedDate" DATE,
    "Status" TEXT NOT NULL,
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 3. ChildHealthRecords (Sağlık Kayıtları) tablosu
CREATE TABLE IF NOT EXISTS "ChildHealthRecords" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "ChildId" UUID NOT NULL REFERENCES "Children"("Id") ON DELETE CASCADE,
    "DiseaseName" TEXT NOT NULL,
    "OccurrenceDate" DATE,
    "Description" TEXT,
    "Category" TEXT NOT NULL, -- Contagious veya Chronic
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Indexler
CREATE INDEX IF NOT EXISTS "IX_Vaccinations_ChildId" ON "Vaccinations"("ChildId");
CREATE INDEX IF NOT EXISTS "IX_ChildHealthRecords_ChildId" ON "ChildHealthRecords"("ChildId");
