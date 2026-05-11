# 🔒 KresApp Güvenlik Tarama Raporu

> **Tarih:** 2026-05-08 | **Kapsam:** .NET Backend (KresApp.API) + Flutter Mobil Uygulama (ashb_kres)

---

## 🔴 Kritik Güvenlik Açıkları

### 1. Parola Düz Metin Depolama (EnrollmentRequest.ParentPassword)
- **Dosya:** [EnrollmentRequest.cs](file:///e:/mobileapp/KresApp.API/KresApp.Domain/Entities/EnrollmentRequest.cs#L12)
- **Risk:** Başvuruda veli şifresi düz metin olarak veritabanına yazılıyor
- **Durum:** ✅ Düzeltildi — submit sırasında hash'leniyor

### 2. CORS Politikası: Herşeye Açık
- **Dosya:** [Program.cs:113](file:///e:/mobileapp/KresApp.API/KresApp.API/Program.cs#L113)
- **Mevcut:** `AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()`
- **Risk:** Herhangi bir origin tüm API'ye erişebilir
- **Durum:** ⚠️ Düzeltilecek

### 3. Register Endpoint Açık (AuthController)
- **Dosya:** [AuthController.cs:18](file:///e:/mobileapp/KresApp.API/KresApp.API/Controllers/AuthController.cs#L18)
- **Risk:** `/api/auth/register` herhangi biri tarafından çağrılabilir, yetkisiz kullanıcı kaydı yapılabilir
- **Durum:** ⚠️ Düzeltilecek

### 4. JWT Token Expiry — 1 Gün
- **Dosya:** [JwtService.cs:62](file:///e:/mobileapp/KresApp.API/KresApp.Infrastructure/Services/JwtService.cs#L62)
- **Mevcut:** `DateTime.UtcNow.AddDays(1)`
- **Risk:** Token çalınırsa 24 saat boyunca geçerli. Mobil için kabul edilebilir ama refresh token mekanizması yok
- **Durum:** ℹ️ Bilgi — şimdilik kabul edilebilir

## 🟡 Orta Seviye Açıklar

### 5. Enrollment File Upload — Rate Limiting Yok
- **Dosya:** [EnrollmentFileController.cs](file:///e:/mobileapp/KresApp.API/KresApp.API/Controllers/EnrollmentFileController.cs)
- **Risk:** DDoS saldırısı ile disk/MinIO doldurulabilir
- **Durum:** ⚠️ Düzeltilecek

### 6. EnrollmentController Role Case Mismatch
- **Dosya:** [EnrollmentController.cs:39](file:///e:/mobileapp/KresApp.API/KresApp.API/Controllers/EnrollmentController.cs#L39)
- **Mevcut:** `Roles = "admin,superadmin"` ama diğer controller'larda `Roles = "Admin"`
- **Risk:** Büyük/küçük harf uyuşmazlığı nedeniyle yetkilendirme bypass'ı olabilir
- **Durum:** ⚠️ Düzeltilecek

### 7. Exception Mesajları İstemciye Sızıyor
- **Risk:** `catch (Exception ex) → ex.Message` doğrudan istemciye dönüyor. Stack trace yok ama iç hata mesajları sızabilir
- **Durum:** ℹ️ Mevcut durumda kontrollü mesajlar kullanılıyor, ama genel bir exception filter eklenmeli

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

## Düzeltme Planı

| # | Açık | Çözüm | Öncelik |
|---|---|---|---|
| 1 | Parola düz metin | Submit sırasında hash'le | 🔴 Kritik |
| 2 | CORS herşeye açık | Belirli origin'lere kısıtla | 🔴 Kritik |
| 3 | Register endpoint açık | Admin-only yap | 🔴 Kritik |
| 5 | File upload rate limit yok | Rate limiting middleware | 🟡 Orta |
| 6 | Role case mismatch | "Admin" standardize et | 🟡 Orta |
