CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 1. Attendances
CREATE TABLE IF NOT EXISTS attendances (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    child_id UUID NOT NULL,
    date DATE NOT NULL,
    status INTEGER NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 2. Daily Reports
CREATE TABLE IF NOT EXISTS daily_reports (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    child_id UUID NOT NULL,
    date DATE NOT NULL,
    mood INTEGER NOT NULL,
    morning_meal INTEGER NOT NULL,
    lunch_meal INTEGER NOT NULL,
    afternoon_meal INTEGER NOT NULL,
    did_sleep BOOLEAN NOT NULL,
    sleep_hours INTEGER NOT NULL,
    sleep_mins INTEGER NOT NULL,
    toilet_count INTEGER NOT NULL,
    activities TEXT[] NOT NULL DEFAULT '{}',
    teacher_note TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 3. Announcements
CREATE TABLE IF NOT EXISTS announcements (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    title TEXT NOT NULL,
    body TEXT NOT NULL,
    category INTEGER NOT NULL,
    emoji TEXT NOT NULL,
    date TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 4. Payments
CREATE TABLE IF NOT EXISTS payments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    child_id UUID NOT NULL,
    amount NUMERIC NOT NULL,
    month INTEGER NOT NULL,
    year INTEGER NOT NULL,
    status INTEGER NOT NULL,
    due_date TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 5. Conversations
CREATE TABLE IF NOT EXISTS conversations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    title TEXT,
    is_group BOOLEAN NOT NULL,
    participant_ids UUID[] NOT NULL DEFAULT '{}',
    last_message TEXT,
    last_message_time TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 6. Messages
CREATE TABLE IF NOT EXISTS messages (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    conversation_id UUID NOT NULL,
    sender_id UUID NOT NULL,
    sender_name TEXT NOT NULL,
    content TEXT NOT NULL,
    is_read BOOLEAN NOT NULL DEFAULT FALSE,
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 7. Meal Menus
CREATE TABLE IF NOT EXISTS meal_menus (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    day TEXT NOT NULL,
    breakfast TEXT NOT NULL,
    lunch TEXT NOT NULL,
    snack TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- 8. Schedules
CREATE TABLE IF NOT EXISTS schedules (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    day TEXT NOT NULL,
    subject TEXT NOT NULL,
    start_time TEXT NOT NULL,
    end_time TEXT NOT NULL,
    teacher TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

-- Ek Stra: Entitylere eklediğimiz yeni sütunlar (Users ve Children için)
ALTER TABLE children 
ADD COLUMN IF NOT EXISTS blood_type TEXT DEFAULT 'Tamamlanmadı',
ADD COLUMN IF NOT EXISTS allergies TEXT[] DEFAULT '{}',
ADD COLUMN IF NOT EXISTS medical_notes TEXT,
ADD COLUMN IF NOT EXISTS parent_name TEXT DEFAULT '',
ADD COLUMN IF NOT EXISTS parent_phone TEXT DEFAULT '',
ADD COLUMN IF NOT EXISTS secondary_phone TEXT,
ADD COLUMN IF NOT EXISTS enrollment_date DATE DEFAULT CURRENT_DATE;

ALTER TABLE users
ADD COLUMN IF NOT EXISTS name TEXT DEFAULT '',
ADD COLUMN IF NOT EXISTS phone TEXT;
