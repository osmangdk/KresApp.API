-- child_allergies: Çocuğa ait alerji bilgileri (1:N ilişki)
CREATE TABLE IF NOT EXISTS child_allergies (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    child_id UUID NOT NULL REFERENCES children(id) ON DELETE CASCADE,
    allergy_name TEXT NOT NULL,
    severity TEXT,           -- Hafif, Orta, Ağır
    notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Performans için index
CREATE INDEX IF NOT EXISTS idx_child_allergies_child_id ON child_allergies(child_id);

-- Eski allergies sütununu children tablosundan kaldır (opsiyonel - veri taşıma sonrası)
-- ALTER TABLE children DROP COLUMN IF EXISTS allergies;
