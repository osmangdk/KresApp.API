## Kurum Zili (Institution Bell) Module

I have implemented the "Kurum Zili" module, which allows parents to notify the school when they are coming to drop off or pick up their child, and allows teachers to manage these requests.

### Backend Implementation
- **Domain**: Added `SchoolBellRequest` entity with `Type` (DropOff/PickUp) and `Status` (Pending/Completed/Cancelled).
- **Persistence**: Created `SchoolBellRepository` and updated `AppDbContext` and `init_db.sql`.
- **Application**: Created `SchoolBellService` and `SchoolBellDtos`.
- **API**: Created `SchoolBellController` with endpoints for creating, listing, and updating status.

### Frontend Implementation
- **Models & Providers**: Added `SchoolBellRequestModel` and `SchoolBellNotifier`.
- **Screens**:
  - **Teacher View**: `SchoolBellScreen` lists active requests with a design matching the provided screenshot. Teachers can mark requests as "Completed" or "Cancelled".
  - **Parent View**: `SchoolBellRequestFormScreen` provides a simple form for parents to select a child, action type, estimated time, and optional notes.
- **Integration**: Added "Kurum Zili" buttons to all dashboard types (Admin, Teacher, Parent) for quick access.

## Verification

- Verified backend service registration and controller endpoints.
- Verified frontend model mapping and routing configuration.
- The UI design in `SchoolBellScreen` follows the aesthetics requested (teal header, orange status labels, card-based layout).

## Announcement Module Fix

I have fixed the issue where new announcements were not being saved and the list was showing mock data.

### Changes Made
- **Frontend Model**: Created `AnnouncementModel` to handle data from the backend.
- **Frontend Provider**: Created `AnnouncementNotifier` using Riverpod to fetch, create, and delete announcements via the API.
- **Duyurular Screen**: 
  - Converted `AnnouncementsScreen` from mock data to real data.
  - Implemented the save logic in the "Yeni Duyuru" form, correctly sending the title, body, category, and emoji to the backend.
  - Added category filtering that communicates with the backend query parameters.
  - Implemented role-based access; only Admin/SuperAdmin users can see the "+" button.
- **Dashboards**: 
  - Updated Admin Dashboard to show the real unread announcement count badge.
  - Updated Parent Dashboard to show the most recent 2 announcements from the database.
