# KresApp.API - Eksik Endpoint Analizi

> **Tarih:** 2026-05-04  
> **Kaynak:** Flutter mobil uygulama (`ashb_kres`) analizi  
> **Durum:** Mobil uygulamadaki tüm özellikler şu anda `MockData` (sahte/hardcoded veri) kullanıyor. API'ye bağlanması için aşağıdaki endpoint'lerin oluşturulması gerekiyor.

---

## 📊 Mevcut Durum

### ✅ API'de Var Olan Endpoint'ler

| Controller | Endpoint | Method | Açıklama |
|---|---|---|---|
| `AuthController` | `api/auth/register` | POST | Kullanıcı kaydı |
| `AuthController` | `api/auth/login` | POST | Kullanıcı girişi (JWT token döner) |
| `ChildrenController` | `api/children` | GET | Çocuk listesi (role göre filtrelenir) |
| `ChildrenController` | `api/children` | POST | Yeni çocuk kaydı (Admin/Teacher) |

### ❌ Eksik Olan Endpoint'ler (Toplam: 35+)

---

## 1. 👤 Profil Yönetimi (`ProfileController`)

Mobil uygulama: `profile_screen.dart` — Kullanıcı profil bilgilerini görüntüleme, şifre/telefon güncelleme.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 1 | `api/profile` | GET | Oturumdaki kullanıcının profil bilgilerini getir | Authorize |
| 2 | `api/profile` | PUT | Profil bilgilerini güncelle (ad, telefon vb.) | Authorize |
| 3 | `api/profile/change-password` | POST | Şifre değiştir | Authorize |
| 4 | `api/profile/upload-avatar` | POST | Profil fotoğrafı yükle | Authorize |

**Request/Response modelleri:**
```
GET /api/profile → { id, name, email, phone, role, avatarUrl }
PUT /api/profile → Body: { name, phone }
POST /api/profile/change-password → Body: { currentPassword, newPassword }
```

---

## 2. 👧 Çocuk Yönetimi (`ChildrenController` - Genişletme)

Mobil uygulama: `children_list_screen.dart`, `child_detail_screen.dart`, `add_child_screen.dart` — Çocuk detayı, güncelleme, silme, alerji düzenleme.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 5 | `api/children/{id}` | GET | Tek çocuğun detay bilgileri | Authorize |
| 6 | `api/children/{id}` | PUT | Çocuk bilgilerini güncelle | Admin, Teacher |
| 7 | `api/children/{id}` | DELETE | Çocuk kaydını sil | Admin |
| 8 | `api/children/{id}/allergies` | PUT | Çocuğun alerji/beslenme bilgilerini güncelle | Authorize (Veli kendi çocuğu) |
| 9 | `api/children/search` | GET | Çocuk arama (isim, sınıf filtresi) | Authorize |

**Request/Response modelleri:**
```
GET /api/children/{id} → { id, firstName, lastName, birthDate, className, bloodType, allergies[], medicalNotes, parentName, parentPhone, secondaryPhone, enrollmentDate }
PUT /api/children/{id}/allergies → Body: { allergies: ["Fıstık", "Süt"], medicalNotes: "..." }
GET /api/children/search?q=can&class=Papatya → [ChildDto]
```

---

## 3. ✅ Yoklama Yönetimi (`AttendanceController`)

Mobil uygulama: `attendance_screen.dart` — Günlük yoklama alma, sınıf filtresi, özet istatistik.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 10 | `api/attendance` | GET | Belirli tarihteki yoklama listesi | Authorize |
| 11 | `api/attendance` | POST | Toplu yoklama kaydet (tüm sınıf) | Admin, Teacher |
| 12 | `api/attendance/{childId}` | GET | Bir çocuğun yoklama geçmişi | Authorize |
| 13 | `api/attendance/summary` | GET | Yoklama özet istatistikleri (geldi/gelmedi/geç) | Admin, Teacher |

**Request/Response modelleri:**
```
GET /api/attendance?date=2026-05-04&class=Papatya → [{ childId, childName, status, time }]
POST /api/attendance → Body: { date, records: [{ childId, status: "present|absent|late" }] }
GET /api/attendance/{childId}?month=2026-05 → [{ date, status }]
GET /api/attendance/summary?date=2026-05-04 → { present: 12, absent: 3, late: 1, total: 16, rate: 75.0 }
```

---

## 4. 📋 Günlük Rapor Yönetimi (`DailyReportController`)

Mobil uygulama: `daily_report_list_screen.dart`, `daily_report_form_screen.dart` — Çocuğun günlük ruh hali, yemek, uyku, tuvalet, etkinlik ve öğretmen notu.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 14 | `api/daily-reports` | GET | Belirli tarihteki tüm raporları listele | Authorize |
| 15 | `api/daily-reports/{childId}` | GET | Bir çocuğun günlük raporunu getir | Authorize |
| 16 | `api/daily-reports` | POST | Yeni günlük rapor oluştur | Admin, Teacher |
| 17 | `api/daily-reports/{id}` | PUT | Mevcut raporu güncelle | Admin, Teacher |

**Request/Response modelleri:**
```
POST /api/daily-reports → Body: {
  childId, date,
  mood: "excellent|good|normal|sad",
  meals: { morning: "ateAll|ateHalf|didNotEat", lunch: "...", afternoon: "..." },
  sleep: { didSleep: true, hours: 1, minutes: 30 },
  toiletCount: 2,
  activities: ["Resim", "Müzik"],
  teacherNote: "Bugün çok neşeliydi."
}
```

---

## 5. 📢 Duyuru Yönetimi (`AnnouncementsController`)

Mobil uygulama: `announcements_screen.dart` — Duyuru listesi, kategori filtresi, yeni duyuru oluşturma.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 18 | `api/announcements` | GET | Duyuru listesi (filtrelenebilir) | Authorize |
| 19 | `api/announcements/{id}` | GET | Tek duyuru detayı | Authorize |
| 20 | `api/announcements` | POST | Yeni duyuru oluştur | Admin |
| 21 | `api/announcements/{id}` | PUT | Duyuruyu güncelle | Admin |
| 22 | `api/announcements/{id}` | DELETE | Duyuruyu sil | Admin |
| 23 | `api/announcements/{id}/read` | POST | Duyuruyu okundu işaretle | Authorize |

**Request/Response modelleri:**
```
GET /api/announcements?category=Etkinlik → [{ id, title, body, category, date, isRead, emoji }]
POST /api/announcements → Body: { title, body, category: "Genel|Etkinlik|Tatil|Acil" }
```

---

## 6. 💰 Ödeme Yönetimi (`PaymentsController`)

Mobil uygulama: `payments_screen.dart` — Ödeme listesi, durum filtresi, aylık özet, ödeme detayı.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 24 | `api/payments` | GET | Ödeme listesi (veli: kendi çocukları) | Authorize |
| 25 | `api/payments/{id}` | GET | Ödeme detayı | Authorize |
| 26 | `api/payments` | POST | Yeni ödeme kaydı oluştur | Admin |
| 27 | `api/payments/{id}` | PUT | Ödeme durumunu güncelle | Admin |
| 28 | `api/payments/summary` | GET | Ödeme özet istatistikleri | Admin |

**Request/Response modelleri:**
```
GET /api/payments?status=pending&month=2026-05 → [{ id, childId, childName, amount, month, year, status, dueDate }]
GET /api/payments/summary → { total: 45000, collected: 30000, pending: 15000, paidCount: 6, totalCount: 10 }
POST /api/payments → Body: { childId, amount, month, year, dueDate }
PUT /api/payments/{id} → Body: { status: "paid|pending|overdue" }
```

---

## 7. 💬 Mesajlaşma Yönetimi (`MessagesController`)

Mobil uygulama: `messages_screen.dart`, `chat_screen.dart` — Konuşma listesi, mesaj gönderme/alma, grup mesajları.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 29 | `api/conversations` | GET | Kullanıcının konuşma listesi | Authorize |
| 30 | `api/conversations/{id}/messages` | GET | Bir konuşmanın mesajlarını getir | Authorize |
| 31 | `api/conversations/{id}/messages` | POST | Mesaj gönder | Authorize |
| 32 | `api/conversations` | POST | Yeni konuşma başlat | Authorize |
| 33 | `api/conversations/{id}/read` | POST | Mesajları okundu işaretle | Authorize |

**Request/Response modelleri:**
```
GET /api/conversations → [{ id, title, isGroup, lastMessage, lastMessageTime, unreadCount, participantIds[] }]
GET /api/conversations/{id}/messages?page=1 → [{ id, senderId, senderName, content, timestamp, isRead }]
POST /api/conversations/{id}/messages → Body: { content }
```

> **Not:** Gerçek zamanlı mesajlaşma için ileride **SignalR Hub** eklenebilir.

---

## 8. 🍽️ Yemek Menüsü Yönetimi (`MealMenuController`)

Mobil uygulama: `meal_menu_screen.dart`, `meal_menu_form_screen.dart` — Haftalık yemek menüsü CRUD.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 34 | `api/meals` | GET | Haftalık yemek menüsünü getir | Authorize |
| 35 | `api/meals` | POST | Yeni menü ekle | Admin |
| 36 | `api/meals/{id}` | PUT | Menüyü güncelle | Admin |
| 37 | `api/meals/{id}` | DELETE | Menüyü sil | Admin |

**Request/Response modelleri:**
```
GET /api/meals?week=2026-W19 → [{ id, day, breakfast, lunch, snack }]
POST /api/meals → Body: { day: "Pazartesi", breakfast: "...", lunch: "...", snack: "..." }
```

---

## 9. 📅 Ders Programı Yönetimi (`ScheduleController`)

Mobil uygulama: `schedule_screen.dart`, `schedule_form_screen.dart` — Günlere göre ders programı CRUD.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 38 | `api/schedules` | GET | Ders programını getir | Authorize |
| 39 | `api/schedules` | POST | Yeni ders ekle | Admin |
| 40 | `api/schedules/{id}` | PUT | Ders bilgisini güncelle | Admin |
| 41 | `api/schedules/{id}` | DELETE | Dersi sil | Admin |

**Request/Response modelleri:**
```
GET /api/schedules → [{ id, day, subject, startTime, endTime, teacher }]
POST /api/schedules → Body: { day: "Pazartesi", subject: "Müzik", startTime: "09:00", endTime: "09:45", teacher: "Fatma Demir" }
```

---

## 10. 📊 Dashboard / İstatistik (`DashboardController`)

Mobil uygulama: `admin_dashboard_screen.dart`, `teacher_dashboard_screen.dart`, `parent_dashboard_screen.dart` — Rol bazlı istatistikler ve son aktiviteler.

| # | Endpoint | Method | Açıklama | Yetki |
|---|---|---|---|---|
| 42 | `api/dashboard/stats` | GET | Dashboard istatistikleri (rol bazlı) | Authorize |
| 43 | `api/dashboard/notifications` | GET | Son aktiviteler/bildirimler | Authorize |
| 44 | `api/dashboard/weekly-attendance` | GET | Haftalık yoklama oranları (grafik verisi) | Admin, Teacher |

**Request/Response modelleri:**
```
GET /api/dashboard/stats → {
  totalStudents: 16, presentToday: 12, attendanceRate: 75.0,
  pendingPayments: 4, overduePayments: 1, totalStaff: 5
}
GET /api/dashboard/notifications → [{ id, title, body, type, isRead, createdAt }]
GET /api/dashboard/weekly-attendance → { rates: [87.5, 93.7, 81.2, 87.5, 100, 0, 0] }
```

---

## 📋 Özet Tablo

| Modül | Eksik Endpoint Sayısı | Öncelik |
|---|---|---|
| Profil Yönetimi | 4 | 🔴 Yüksek |
| Çocuk Yönetimi (genişletme) | 5 | 🔴 Yüksek |
| Yoklama | 4 | 🔴 Yüksek |
| Günlük Rapor | 4 | 🔴 Yüksek |
| Duyurular | 6 | 🟡 Orta |
| Ödemeler | 5 | 🟡 Orta |
| Mesajlaşma | 5 | 🟡 Orta |
| Yemek Menüsü | 4 | 🟢 Düşük |
| Ders Programı | 4 | 🟢 Düşük |
| Dashboard/İstatistik | 3 | 🟡 Orta |
| **TOPLAM** | **44** | |

---

## 🏗️ Önerilen Uygulama Sırası

1. **Profil** ve **Çocuk genişletme** → Temel kullanıcı/çocuk işlemleri
2. **Yoklama** ve **Günlük Rapor** → Günlük operasyonlar
3. **Duyurular** ve **Dashboard** → Bilgilendirme
4. **Ödemeler** → Mali takip
5. **Mesajlaşma** → İletişim (SignalR ile gerçek zamanlı)
6. **Yemek Menüsü** ve **Ders Programı** → İçerik yönetimi

---

## 📁 Oluşturulması Gereken Dosyalar

### API Katmanı (Controllers)
- `Controllers/ProfileController.cs`
- `Controllers/AttendanceController.cs`
- `Controllers/DailyReportController.cs`
- `Controllers/AnnouncementsController.cs`
- `Controllers/PaymentsController.cs`
- `Controllers/MessagesController.cs`
- `Controllers/MealMenuController.cs`
- `Controllers/ScheduleController.cs`
- `Controllers/DashboardController.cs`

### Application Katmanı (Services + DTOs)
- Her controller için karşılık gelen Service ve DTO sınıfları

### Domain Katmanı (Entities)
- `Attendance.cs`, `DailyReport.cs`, `Announcement.cs`, `Payment.cs`
- `Conversation.cs`, `Message.cs`, `MealMenu.cs`, `Schedule.cs`

### Persistence Katmanı (Repositories + Migrations)
- Her entity için Repository sınıfı
- EF Core DbContext güncelleme ve migration'lar
