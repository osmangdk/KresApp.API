CREATE TABLE IF NOT EXISTS "UserAccessRequests" (
    "Id"          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Email"       TEXT NOT NULL,
    "Name"        TEXT NOT NULL,
    "Phone"       TEXT,
    "RequestedRole" INTEGER NOT NULL DEFAULT 0,
    "Status"      INTEGER NOT NULL DEFAULT 0,
    "AdminNote"   TEXT,
    "CreatedAt"   TIMESTAMPTZ NOT NULL DEFAULT now(),
    "ProcessedAt" TIMESTAMPTZ
);
