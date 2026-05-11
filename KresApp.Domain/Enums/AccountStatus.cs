namespace KresApp.Domain.Enums;

public enum AccountStatus
{
    PendingDocuments   = 0,  // Kesinleşmiş listede, evrak bekliyor
    DocumentsSubmitted = 1,  // Evraklar yüklendi, admin onayı bekliyor
    Active             = 2,  // Tam erişim
    Suspended          = 3   // Askıya alındı
}
