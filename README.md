# 🔐 Privacy Lens

> **Your device. Your data. Your privacy.**

Privacy Lens is an open-source **Android privacy and permission
dashboard** built with **.NET MAUI**. It provides a clear, centralized
view of permission-related information for applications on an Android
device, helping users understand which apps have access to sensitive
capabilities such as the **camera, microphone, location, contacts,
photos and videos, storage, phone, SMS, calendar, nearby devices,
notifications, and physical activity**.

Privacy Lens follows a **privacy-first, local-processing architecture**.
It is designed to inspect application and permission metadata made
available by Android---not the personal content protected by those
permissions.

![](Screenshots/Poster.png?raw=true)

------------------------------------------------------------------------

## ✨ Key Features

-   View applications available to Privacy Lens on the Android device.
-   Search and browse applications.
-   Review sensitive permissions granted to individual applications.
-   Browse applications by permission category.
-   View **Sensitive Permissions** in one place.
-   View **High-Risk Permissions** that may deserve additional review.
-   Open detailed information for an application.
-   Open detailed information for a permission.
-   Display reliable permission activity when Android makes it
    available.
-   Open Android system settings for permission management.
-   Cache application and permission metadata locally with SQLite.
-   Use a **cache-first, refresh-second** architecture for responsive
    screens.
-   Automatically refresh stale data after approximately three hours
    when the app is able to refresh.
-   Allow manual refresh whenever the user wants updated information.
-   Support light and dark themes.
-   No advertising.
-   No account required for core functionality.
-   No selling or sharing of app/permission metadata with third parties.

------------------------------------------------------------------------

## 🛡️ Privacy by Design

Privacy Lens is designed to analyze **permission-related metadata**.

It does not need to access the personal content protected by those
permissions simply to provide its permission dashboard.

For example:

``` text
Camera
Allowed
```

means Android reports that an application has relevant Camera
permission. It does **not** mean Privacy Lens accesses or activates the
camera.

Privacy Lens is not designed to read or capture the contents of your:

-   Photos or videos
-   Gallery
-   Personal files or documents
-   Contacts
-   SMS messages
-   Call history
-   Calendar events
-   Camera
-   Microphone
-   Physical location

Privacy Lens may show that another application has permission related to
one of these categories, but the dashboard is based on permission
metadata rather than the personal content itself.

Application and permission metadata used by the core application is
processed and cached locally on the device. Privacy Lens does not sell
or share this information with third parties as part of the
functionality described here.

------------------------------------------------------------------------

# 🧭 Application Navigation

Privacy Lens contains four primary navigation destinations:

``` text
┌────────────┬────────────┬───────────────┬────────────┐
│    Apps    │    Home    │  Permissions  │   About    │
└────────────┴────────────┴───────────────┴────────────┘
```

**Home** is the default landing page.

The complete navigation model is:

``` text
Privacy Lens
│
├── 📱 Apps
│   └── App Details
│       └── Permission Details
│
├── 🏠 Home
│   ├── Sensitive Permissions
│   │   └── Permission Details
│   │       └── App Details
│   │
│   └── High-Risk Permissions
│       └── Permission Details
│           └── App Details
│
├── 🔐 Permissions
│   └── Permission Details
│       └── App Details
│
└── ℹ️ About
    ├── About Privacy Lens
    ├── Privacy Policy
    ├── Terms of Use
    ├── Open-Source Licenses
    └── Application Information
```

------------------------------------------------------------------------

## 🏠 Home

Home is the primary Privacy Lens dashboard.

It provides a quick overview of the device's permission configuration
and can include:

-   User greeting
-   Privacy overview
-   Sensitive permission summary
-   High-risk permission summary
-   Reliable recent permission activity, when available
-   Last synchronization time
-   Quick navigation to important privacy sections

If Android does not expose reliable permission-usage information,
Privacy Lens does not fabricate activity records or timestamps. The Home
page can instead display its welcome state and reliable permission
summaries.

Example activity, only when reliably available:

``` text
Maps
Location
10:32 AM

Example Camera App
Camera
9:15 AM
```

------------------------------------------------------------------------

## 📱 Apps

The Apps page provides an **application-centric** view of privacy.

It reads cached application information through the repository and
SQLite database instead of repeatedly scanning Android whenever the
screen opens.

An application row can display:

``` text
[App Icon]

Application Name
Package Name

4 permissions allowed
```

The page can provide:

-   Search
-   Alphabetical ordering
-   User/system app filtering where supported
-   Pull-to-refresh
-   Loading state
-   Empty state
-   Error state

Selecting an application opens **App Details**.

------------------------------------------------------------------------

## 📄 App Details

App Details provides a focused view of one application.

Information can include:

-   Application icon
-   Application name
-   Package name
-   Application version
-   Sensitive permissions
-   Permission status
-   Reliable last-access information where available

Example:

``` text
Example App
com.example.app
Version 2.1

Permissions

Camera
Allowed
Last access unavailable

Microphone
Allowed

Location
Allowed while using app
```

Where appropriate, Privacy Lens provides a **Manage Permissions** action
that opens the application's Android system settings.

Android remains responsible for actually granting or revoking another
application's permissions.

------------------------------------------------------------------------

## 🔐 Permissions

The Permissions page provides a **permission-centric** view.

It can include categories such as:

``` text
Camera
8 apps allowed

Microphone
5 apps allowed

Location
12 apps allowed

Contacts
4 apps allowed

Photos & Videos
6 apps allowed
```

Permission categories are based on the Android version and capabilities
of the device.

Selecting a permission opens **Permission Details**.

------------------------------------------------------------------------

## 🔎 Permission Details

Permission Details shows applications that have relevant access to a
selected permission category.

Example:

``` text
Camera

8 apps have access

Example App
Allowed
Last access unavailable

Another App
Allowed
Last used: Today, 9:15 AM
```

Reliable recent usage can be sorted first when Android provides the
necessary information. Applications without reliable usage timestamps
can be displayed alphabetically afterward.

Selecting an application opens **App Details**.

------------------------------------------------------------------------

## 🛡️ Sensitive Permissions

The Sensitive Permissions page focuses on Android permissions associated
with sensitive device capabilities or user information.

Depending on the Android version, categories may include:

-   Camera
-   Microphone
-   Location
-   Contacts
-   Photos & Videos
-   Music & Audio
-   Storage / Files
-   Phone
-   SMS
-   Calendar
-   Nearby Devices
-   Physical Activity

A permission can display a summary such as:

``` text
Camera
8 apps allowed
```

Selecting it opens **Permission Details**.

A permission being sensitive does **not** mean an application using it
is dangerous. Many legitimate applications require sensitive permissions
to provide expected functionality.

------------------------------------------------------------------------

## ⚠️ High-Risk Permissions

The High-Risk Permissions page highlights permission categories or
permission states that may deserve additional user review.

Examples may include:

-   Camera
-   Microphone
-   Precise Location
-   Background Location
-   Contacts
-   SMS
-   Phone
-   Files / Media

High-risk classification should be based on explicit, documented Privacy
Lens rules.

A high-risk classification means:

> **Review recommended**

It does **not** mean:

> **Malware**, **Spyware**, or **Dangerous application**

Privacy Lens should never classify an application as malicious solely
because it has a sensitive permission.

------------------------------------------------------------------------

## ℹ️ About

The About section provides information about Privacy Lens itself.

It can contain:

-   Application name and version
-   Product description
-   Privacy principles
-   Privacy Policy
-   Terms of Use
-   Open-source license information
-   Third-party notices
-   Source repository information
-   Developer/contact information

Privacy Lens is distributed under the **MIT License**.

------------------------------------------------------------------------

# ⚠️ Permission Granted vs. Permission Used

This distinction is fundamental to Privacy Lens:

``` text
Permission Granted ≠ Permission Used
```

If an application has Camera permission, that does not prove the
application recently used the camera.

Privacy Lens must not infer specific permission usage solely from:

-   A granted permission
-   Generic application usage
-   Application foreground activity

When Android does not provide reliable permission-specific usage
information, Privacy Lens displays:

``` text
Last access unavailable
```

rather than generating or guessing a timestamp.

------------------------------------------------------------------------

# 🏗️ Architecture

Privacy Lens uses **MVVM**, dependency injection, repository
abstraction, Android platform services, and SQLite-backed caching.

``` text
┌──────────────────────────────────┐
│             XAML UI              │
│                                  │
│ Home / Apps / Permissions/About  │
│ Detail & Analysis Pages          │
└────────────────┬─────────────────┘
                 │
                 ▼
┌──────────────────────────────────┐
│            ViewModels            │
│      CommunityToolkit.Mvvm       │
└────────────────┬─────────────────┘
                 │
                 ▼
┌──────────────────────────────────┐
│        PrivacyRepository         │
│       SQLite-first queries       │
└────────────────┬─────────────────┘
                 │
                 ▼
┌──────────────────────────────────┐
│       AccessLensDatabase         │
│             SQLite               │
└────────────────▲─────────────────┘
                 │
              Updates
                 │
┌────────────────┴─────────────────┐
│         DataSyncService          │
│   Android → SQLite Sync Layer    │
└────────────────┬─────────────────┘
                 │
                 ▼
┌──────────────────────────────────┐
│     Android Platform Services    │
│                                  │
│ PackageManager                   │
│ Permission APIs                  │
│ Capability APIs                  │
│ Supported usage-related APIs     │
│ Android Settings integration     │
└──────────────────────────────────┘
```

The intended direction for ordinary UI reads is:

``` text
View
→ ViewModel
→ PrivacyRepository
→ SQLite
```

The synchronization direction is:

``` text
RefreshService
→ DataSyncService
→ Android Services
→ SQLite
→ Repository
→ ViewModel
→ View
```

Views and ViewModels should not directly query Android `PackageManager`,
`AppOpsManager`, or similar native APIs.

------------------------------------------------------------------------

# 💾 SQLite & Local Caching

Privacy Lens uses SQLite as the primary read source for application
screens.

The local database can contain:

-   Installed/visible application metadata
-   Package names
-   Application versions
-   Permission metadata
-   Permission grant status
-   Permission categories
-   Permission capabilities
-   Reliable permission activity records
-   Synchronization metadata

Application icons should be cached as files rather than stored as large
Base64 values or blobs in SQLite. SQLite stores the corresponding cache
path.

The local database exists to reduce repeated Android package and
permission scans and improve UI responsiveness.

------------------------------------------------------------------------

# 🔄 Refresh Strategy

Privacy Lens follows:

> **CACHE FIRST, REFRESH SECOND**

On a normal application launch:

``` text
Start
  │
  ▼
Initialize SQLite
  │
  ▼
Read Cached Data
  │
  ▼
Display Immediately
  │
  ▼
Check Last Successful Sync
  │
  ├── Cache < 3 hours ──→ Continue Using Cache
  │
  └── Cache ≥ 3 hours
          │
          ▼
     Refresh Data
          │
          ▼
      Update SQLite
          │
          ▼
       Refresh UI
```

The refresh interval is approximately:

``` text
3 hours
```

The interval should be represented by a shared configuration/constant
rather than duplicated throughout the codebase.

All synchronization timestamps should be stored in UTC and converted to
local time for presentation.

------------------------------------------------------------------------

## Manual Refresh

Users can manually request fresh information.

Primary pages can support pull-to-refresh.

``` text
Manual Refresh
      │
      ▼
ForceRefreshAsync()
      │
      ▼
DataSyncService
      │
      ▼
Android APIs
      │
      ▼
SQLite
      │
      ▼
Reload Visible Data
```

Manual refresh bypasses the normal three-hour cache interval.

If refresh fails, Privacy Lens keeps valid cached information rather
than clearing the database.

The UI can display:

``` text
Refresh failed. Showing previously saved data.
```

------------------------------------------------------------------------

## Android Background Execution

Privacy Lens does not assume a simple timer can run exactly every three
hours while the application is terminated.

Android controls background execution and may delay or prevent
background work.

Privacy Lens therefore checks cache freshness when appropriate, such as:

-   Application startup
-   Application resume
-   Home activation
-   Manual refresh

Android WorkManager or another supported mechanism may be evaluated for
optional background synchronization, but the application must remain
correct even when Android does not execute background work.

------------------------------------------------------------------------

# 📁 Project Structure

The following represents the source-oriented Privacy Lens project
structure.

Generated build artifacts and temporary IDE/cache directories are
intentionally excluded.

``` text
PrivacyLens/
│
├── PrivacyLens.csproj
├── App.xaml
├── App.xaml.cs
├── AppShell.xaml
├── AppShell.xaml.cs
├── MauiProgram.cs
│
├── Controls/
│   └── [Reusable MAUI controls]
│
├── Converters/
│   ├── PermissionIconConverter.cs
│   ├── PermissionStatusConverter.cs
│   ├── DateTimeToRelativeTimeConverter.cs
│   └── [Other UI converters]
│
├── Data/
│   ├── AccessLensDatabase.cs
│   ├── DatabaseConstants.cs
│   │
│   ├── Entities/
│   │   ├── InstalledAppEntity.cs
│   │   ├── AppPermissionEntity.cs
│   │   ├── DevicePermissionEntity.cs
│   │   ├── PermissionUsageEntity.cs
│   │   ├── PermissionCapabilityEntity.cs
│   │   └── SyncMetadataEntity.cs
│   │
│   └── Mappers/
│       ├── InstalledAppEntityMapper.cs
│       ├── AppPermissionEntityMapper.cs
│       ├── DevicePermissionEntityMapper.cs
│       ├── PermissionUsageEntityMapper.cs
│       └── PermissionCapabilityEntityMapper.cs
│
├── Enums/
│   ├── PermissionCategory.cs
│   ├── PermissionAccessStatus.cs
│   ├── DataAvailability.cs
│   ├── PrivacyAlertSeverity.cs
│   ├── SyncStatus.cs
│   └── SyncType.cs
│
├── Helpers/
│   ├── PermissionMapper.cs
│   ├── DateTimeHelper.cs
│   ├── GreetingHelper.cs
│   └── [Other helpers]
│
├── Models/
│   ├── InstalledApp.cs
│   ├── AppPermission.cs
│   ├── DevicePermission.cs
│   ├── PermissionUsageRecord.cs
│   ├── PermissionCapability.cs
│   ├── PermissionAccessItem.cs
│   ├── PrivacyAlert.cs
│   └── SyncResult.cs
│
├── Platforms/
│   └── Android/
│       ├── AndroidManifest.xml
│       ├── MainActivity.cs
│       ├── MainApplication.cs
│       │
│       ├── Resources/
│       │   ├── values/
│       │   └── values-night/
│       │
│       └── Services/
│           ├── AndroidInstalledAppsService.cs
│           ├── AndroidPermissionService.cs
│           ├── AndroidPermissionUsageService.cs
│           ├── AndroidDeviceCapabilityService.cs
│           └── AndroidAppSettingsService.cs
│
├── Properties/
│   └── [Project properties]
│
├── Repositories/
│   ├── Interfaces/
│   │   └── IPrivacyRepository.cs
│   │
│   └── PrivacyRepository.cs
│
├── Resources/
│   ├── AppIcon/
│   ├── Fonts/
│   ├── Images/
│   ├── Raw/
│   ├── Splash/
│   │
│   └── Styles/
│       ├── Colors.xaml
│       └── Styles.xaml
│
├── Services/
│   ├── Interfaces/
│   │   ├── IInstalledAppsService.cs
│   │   ├── IPermissionService.cs
│   │   ├── IPermissionUsageService.cs
│   │   ├── IDeviceCapabilityService.cs
│   │   ├── IAppSettingsService.cs
│   │   ├── IDataSyncService.cs
│   │   └── IRefreshService.cs
│   │
│   ├── DataSyncService.cs
│   └── RefreshService.cs
│
├── ViewModels/
│   ├── BaseViewModel.cs
│   ├── HomeViewModel.cs
│   ├── AppsViewModel.cs
│   ├── AppDetailsViewModel.cs
│   ├── PermissionsViewModel.cs
│   ├── PermissionDetailsViewModel.cs
│   ├── SensitivePermissionsViewModel.cs
│   ├── HighRiskPermissionsViewModel.cs
│   └── AboutViewModel.cs
│
└── Views/
    ├── HomePage.xaml
    ├── HomePage.xaml.cs
    ├── AppsPage.xaml
    ├── AppsPage.xaml.cs
    ├── AppDetailsPage.xaml
    ├── AppDetailsPage.xaml.cs
    ├── PermissionsPage.xaml
    ├── PermissionsPage.xaml.cs
    ├── PermissionDetailsPage.xaml
    ├── PermissionDetailsPage.xaml.cs
    ├── SensitivePermissionsPage.xaml
    ├── SensitivePermissionsPage.xaml.cs
    ├── HighRiskPermissionsPage.xaml
    ├── HighRiskPermissionsPage.xaml.cs
    ├── AboutPage.xaml
    └── AboutPage.xaml.cs
```

> The exact file list can evolve with the implementation. The important
> architectural boundaries are more important than forcing every feature
> into a particular file.


------------------------------------------------------------------------

# 🧩 Technology Stack

Privacy Lens is built with:

  Technology                       Purpose
  -------------------------------- ----------------------------------
  .NET MAUI                        Android application framework
  C#                               Application language
  XAML                             User interface
  CommunityToolkit.Mvvm            MVVM infrastructure
  SQLite / sqlite-net-pcl          Local persistent cache
  Microsoft Dependency Injection   Dependency management
  Android Package APIs             Application metadata
  Android Permission APIs          Permission metadata
  Android Settings Intents         Permission-management navigation

The current project targets Android using a framework such as:

``` text
net10.0-android
```

The exact target should match the repository's `.csproj` and installed
.NET MAUI SDK/workloads.

------------------------------------------------------------------------

# 🔌 Service Boundaries

Android-specific functionality is isolated behind interfaces.

Examples:

``` text
IInstalledAppsService
        │
        └── AndroidInstalledAppsService

IPermissionService
        │
        └── AndroidPermissionService

IPermissionUsageService
        │
        └── AndroidPermissionUsageService

IDeviceCapabilityService
        │
        └── AndroidDeviceCapabilityService

IAppSettingsService
        │
        └── AndroidAppSettingsService
```

This keeps Android code out of ViewModels and makes core application
logic easier to test.

------------------------------------------------------------------------

# 📦 Data Model

The SQLite cache is organized around several logical entities.

### Installed Apps

Stores application metadata such as:

``` text
PackageName
Name
VersionName
VersionCode
IsSystemApp
IconCachePath
GrantedSensitivePermissionCount
LastUpdatedAt
```

### App Permissions

Represents application-to-permission relationships:

``` text
PackageName
PermissionName
PermissionCategory
PermissionAccessStatus
IsGranted
LastAccessTime
UsageDataAvailability
```

### Device Permissions

Stores permission-level summaries:

``` text
PermissionName
DisplayName
PermissionCategory
GrantedAppCount
IsSupported
```

### Permission Usage

Stores permission-specific activity only when reliable evidence is
available:

``` text
PackageName
PermissionCategory
PermissionName
AccessTime
IsBackgroundAccess
DataAvailability
```

### Sync Metadata

Tracks synchronization state:

``` text
SyncType
LastSuccessfulSyncAt
LastAttemptedSyncAt
SyncStatus
ErrorMessage
```

------------------------------------------------------------------------

# 🚀 Getting Started

## Prerequisites

To build Privacy Lens, install a .NET development environment with
Android/.NET MAUI support.

Verify the installed SDK:

``` bash
dotnet --version
```

List installed SDKs:

``` bash
dotnet --list-sdks
```

Check workloads:

``` bash
dotnet workload list
```

If required, install the MAUI workload using the appropriate command for
your development environment.

------------------------------------------------------------------------

## Clone the Repository

``` bash
git clone <YOUR-REPOSITORY-URL>
cd PrivacyLens
```

------------------------------------------------------------------------

## Restore Dependencies

``` bash
dotnet restore
```

------------------------------------------------------------------------

## Build Android

From the project directory:

``` bash
dotnet build -f net10.0-android
```

If the repository targets a different Android target framework, use the
framework declared in `PrivacyLens.csproj`.

You can also build and deploy using Visual Studio with an Android
emulator or physical Android device.

------------------------------------------------------------------------

# 🧪 Testing

Core application logic should be designed so it can be tested without
requiring Android APIs directly.

Good unit-test targets include:

-   Permission mapping
-   Permission categorization
-   High-risk classification rules
-   Sensitive permission classification
-   App search/filtering
-   Permission sorting
-   Repository queries
-   SQLite cache behavior
-   Three-hour expiration logic
-   Manual force refresh
-   Synchronization failure behavior
-   Relative time formatting
-   Greeting generation
-   Data availability handling

Android-specific behavior should be validated separately on Android
devices/emulators.

------------------------------------------------------------------------

# 📱 Android Compatibility

Privacy Lens must account for differences between Android versions.

Areas that can vary include:

-   Package visibility
-   Runtime permissions
-   Media permissions
-   Scoped storage
-   Background location
-   Approximate vs. precise location
-   Notification permissions
-   Nearby-device permissions
-   Permission auto-reset
-   Permission usage visibility
-   Background execution

Platform-specific code should use runtime API-level checks where
necessary.

------------------------------------------------------------------------

# 🔍 Package Visibility

Modern Android versions restrict which installed applications one
application can discover.

Privacy Lens should use the minimum package visibility necessary for its
core functionality.

Broad package visibility must not be added silently.

If `QUERY_ALL_PACKAGES` is considered necessary, its technical need and
application-distribution policy implications must be evaluated before
use.

------------------------------------------------------------------------

# 🔐 Least-Privilege Principle

Privacy Lens should not request sensitive permissions merely to
determine whether another application has those permissions.

For example, Privacy Lens should not request Camera permission simply to
show that another app has Camera permission.

The application should request only access genuinely necessary for its
own functionality.

------------------------------------------------------------------------

# 🌐 Network & Third-Party Data Sharing

The core Privacy Lens architecture is designed for local processing.

Privacy Lens does not need to upload the user's installed-app list or
permission metadata to a Privacy Lens server to provide its core
dashboard.

The project should avoid adding analytics, advertising, cloud
synchronization, or other third-party SDKs that transmit data without
reviewing and updating:

-   Privacy Policy
-   Terms of Use
-   Store disclosures
-   Data Safety declarations

Any future change in data behavior must be documented accurately.

------------------------------------------------------------------------

# 📜 Privacy Policy

Privacy Lens includes a dedicated Privacy Policy explaining its
information-handling practices.

The policy should clearly state that Privacy Lens:

-   Processes permission-related metadata required for the dashboard.
-   Does not access personal content merely to inspect another
    application's permissions.
-   Does not browse the user's gallery or personal files.
-   Does not read contacts, messages, or calendar content for the
    permission dashboard.
-   Does not activate the camera or microphone for permission
    inspection.
-   Does not sell app/permission metadata.
-   Does not share app/permission metadata with third parties as part of
    the described core functionality.
-   Stores cache information locally on the device.

See:

``` text
PRIVACY_POLICY.md
```

------------------------------------------------------------------------

# 📑 Terms of Use

The Terms of Use explain:

-   The informational purpose of Privacy Lens.
-   Android platform limitations.
-   The difference between granted and used permissions.
-   That Privacy Lens is not a definitive malware/spyware detection
    product.
-   User responsibility for permission decisions.
-   Local caching behavior.
-   Open-source licensing.
-   Warranty and liability limitations.

See:

``` text
TERMS_OF_USE.md
```

------------------------------------------------------------------------

# 📖 Open Source

Privacy Lens is an open-source project.

Contributions that improve:

-   Android compatibility
-   Accessibility
-   Performance
-   Permission mapping
-   UI/UX
-   SQLite performance
-   Tests
-   Documentation
-   Privacy transparency

are welcome, subject to the project's contribution process.

------------------------------------------------------------------------

# 🤝 Contributing

A typical contribution workflow is:

``` bash
git checkout -b feature/my-feature
```

Make the change, add relevant tests, verify the Android build, and
commit with a descriptive message.

Example:

``` bash
git commit -m "feat: improve permission category mapping"
```

Before submitting a contribution, ensure:

-   The project builds.
-   Existing tests pass.
-   New logic has appropriate tests.
-   Android-specific APIs remain behind abstractions.
-   No unnecessary sensitive permission is introduced.
-   No user/app metadata is transmitted externally without explicit
    architectural and privacy review.

------------------------------------------------------------------------

# 📄 License

Privacy Lens is distributed under the **MIT License**.

The MIT License permits broad use, modification, distribution, and
redistribution subject to the terms of the license.

See:

``` text
LICENSE
```

for the complete license text.

Third-party dependencies may be governed by their own licenses and
notices.

------------------------------------------------------------------------

# ⚖️ Disclaimer

Privacy Lens is an informational privacy utility.

It does not guarantee that every installed application, permission, or
permission-use event will be visible.

Android determines what information is exposed to normal third-party
applications.

A sensitive permission being granted does not by itself indicate
malicious behavior.

Users should evaluate permissions in the context of an application's
intended functionality and use Android system controls to manage
permissions where appropriate.

------------------------------------------------------------------------

# 🎯 Project Goal

Privacy Lens has one central goal:

> **Make Android permission information easier to understand without
> compromising the user's privacy in the process.**

The application should help answer:

**Apps**

> What sensitive permissions can this application access?

**Permissions**

> Which applications have access to this permission?

**Sensitive Permissions**

> Which sensitive capabilities are currently available to applications?

**High-Risk Permissions**

> Which permission categories or access patterns deserve additional
> review?

**Home**

> What is the current privacy overview of my device, based on
> information Android reliably makes available?

Privacy Lens should answer these questions using reliable platform
information, local processing, clear language, and a privacy-first
architecture.

------------------------------------------------------------------------

## 🔐 Privacy Lens

**Your device. Your data. Your privacy.**
