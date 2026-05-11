# Implementation Plan - Personnel List Navigation

The goal is to enable navigation from the "Personel" statistic card on the Admin Dashboard to a list screen (User Management screen filtered by teachers/staff) and ensure the User Management screen correctly filters users based on the provided initial role.

## User Review Required

> [!IMPORTANT]
> I am assuming that "Personel" (Personnel) should navigate to the `UserManagementScreen` pre-filtered for "Teachers". Is this correct, or should it show both "Admins" and "Teachers"?

## Proposed Changes

### [ashb_kres (Flutter App)](file:///e:/mobileapp/ashb_kres)

#### [MODIFY] [admin_dashboard_screen.dart](file:///e:/mobileapp/ashb_kres/lib/features/dashboard/screens/admin_dashboard_screen.dart)
- Add `onTap` handler to the "Personel" `StatCard` to navigate to `AppRoutes.userManagement` with `extra: 'teacher'`.

#### [MODIFY] [user_management_screen.dart](file:///e:/mobileapp/ashb_kres/lib/features/users/screens/user_management_screen.dart)
- Update `initState` to trigger a filtered fetch if `initialRole` is provided.

## Verification Plan

### Manual Verification
- Run the Flutter app (or simulate navigation).
- Click on the "Personel" card on the Admin Dashboard.
- Verify that it navigates to the "Kullanıcı Yönetimi" (User Management) screen.
- Verify that the "Öğretmenler" tab is selected and only teachers are listed.
