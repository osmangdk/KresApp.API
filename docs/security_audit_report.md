# 🔒 KresApp Güvenlik Tarama Raporu

> **Tarih:** 2026-05-08 | **Kapsam:** .NET Backend (KresApp.API) + Flutter Mobil Uygulama (ashb_kres)

---

## 🔴 Kritik Güvenlik Açıkları

### 1. Parola Düz Metin Depolama (EnrollmentRequest.ParentPassword)
- **Dosya:** `KresApp.Domain/Entities/EnrollmentRequest.cs`
- **Risk:** Başvuruda veli şifresi düz metin olarak veritabanına yazılıyor
- **Durum:** ✅ **Düzeltildi** — submit sırasında BCrypt ile hash'leniyor

### 2. CORS Politikası: Herşeye Açık
- **Dosya:** `KresApp.API/Program.cs`
- **Önceki:** `AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()`
- **Risk:** Herhangi bir origin tüm API'ye erişebilir
- **Durum:** ✅ **Düzeltildi** — Sadece localhost:5238, 3000, 8080 ve 10.0.2.2 izinli

### 3. Register Endpoint Açık (AuthController)
- **Dosya:** `KresApp.API/Controllers/AuthController.cs`
- **Risk:** `/api/auth/register` herhangi biri tarafından çağrılabilir
- **Durum:** ✅ **Düzeltildi** — `[Authorize(Roles = "Admin,SuperAdmin")]` eklendi

### 4. JWT Token Expiry — 1 Gün
- **Dosya:** `KresApp.Infrastructure/Services/JwtService.cs`
- **Mevcut:** `DateTime.UtcNow.AddDays(1)`
- **Risk:** Token çalınırsa 24 saat boyunca geçerli. Mobil için kabul edilebilir ama refresh token mekanizması yok
- **Durum:** ℹ️ Bilgi — şimdilik kabul edilebilir

## 🟡 Orta Seviye Açıklar

### 5. Enrollment File Upload — Rate Limiting Yok
- **Dosya:** `KresApp.API/Controllers/FinalRegistrationController.cs`
- **Risk:** DDoS saldırısı ile disk/MinIO doldurulabilir
- **Durum:** ⚠️ İlerleyen aşamada rate limiting middleware eklenecek

### 6. EnrollmentController Role Case Mismatch
- **Dosya:** `KresApp.API/Controllers/EnrollmentController.cs`
- **Önceki:** `Roles = "admin,superadmin"` ama diğer controller'larda `Roles = "Admin"`
- **Durum:** ✅ **Düzeltildi** — `"Admin,SuperAdmin"` olarak standardize edildi

### 7. Exception Mesajları İstemciye Sızıyor
- **Risk:** `catch (Exception ex) → ex.Message` doğrudan istemciye dönüyor
- **Durum:** ℹ️ Kontrollü mesajlar kullanılıyor, ama genel bir exception filter eklenmeli

## 🟢 İyi Uygulamalar (Sorun Yok)

| Alan | Durum |
|---|---|
| **Parola Hash:** BCrypt | ✅ Güvenli |
| **JWT Key Kontrolü:** ≥32 byte | ✅ Güvenli |
| **Flutter Token Storage:** FlutterSecureStorage | ✅ Güvenli |
| **API Auth:** Controller seviyesinde `[Authorize]` | ✅ Güvenli |
| **Dosya Yükleme:** 10MB limit + MIME type kontrolü | ✅ Güvenli |
| **SQL Injection:** EF Core parameterized queries | ✅ Güvenli |

---

## Düzeltme Özeti

| # | Açık | Çözüm | Durum |
|---|---|---|---|
| 1 | Parola düz metin | Submit sırasında BCrypt hash | ✅ Tamamlandı |
| 2 | CORS herşeye açık | Belirli origin'lere kısıtlandı | ✅ Tamamlandı |
| 3 | Register endpoint açık | Admin-only yapıldı | ✅ Tamamlandı |
| 5 | File upload rate limit yok | Rate limiting middleware | ⚠️ Bekliyor |
| 6 | Role case mismatch | "Admin,SuperAdmin" standardize | ✅ Tamamlandı |
