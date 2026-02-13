# Backend Implementation Plan: Device Analytics (JSON Storage)

This plan outlines the specific changes required in the **ASP.NET Core Backend** to implement the Device Analytics feature using JSON files.

## 1. New Files

We will create the following files to handle logging and storage logic.

### 1.1. Application Layer (Interfaces & DTOs)

*   `API/Application/Interfaces/IDeviceLogRepository.cs`: Interface defining methods `LogDeviceAsync` and `GetDeviceStatsAsync`.
    *   *Status*: **Already Created**.

### 1.2. Infrastructure Layer (Implementation)

*   `API/Infrastructure/Repositories/FileDeviceLogRepository.cs`:
    *   **Responsibility**: Implements `IDeviceLogRepository`.
    *   **Logic**:
        *   Uses `ConcurrentQueue` to buffer logs in memory.
        *   Uses a `Timer` (Background Task) to flush logs to `API/App_Data/logs/device_logs_YYYYMMDD.json` every 30 seconds.
        *   Uses **NDJSON** format (one JSON object per line) for fast appending.

### 1.3. API Layer (Middleware & Controller)

*   `API/API/Middlewares/DeviceLogMiddleware.cs`:
    *   **Responsibility**: Intercepts every HTTP request.
    *   **Logic**: Parses `User-Agent` header -> Detects "Mobile" vs "Desktop" -> Calls Repository to queue the log.
*   `API/API/Controllers/DashboardController.cs`:
    *   **Responsibility**: Exposes API for Frontend.
    *   **Endpoint**: `GET /api/dashboard/device-stats` -> Returns aggregated stats for charts.

## 2. Modifications to Existing Files

### 2.1. Dependency Injection (`Program.cs`)

We need to register the new service and middleware.

*   **Register Repository**:
    ```csharp
    builder.Services.AddSingleton<IDeviceLogRepository, FileDeviceLogRepository>();
    ```
    *(Note: Must be Singleton to maintain the memory queue state)*

*   **Register Middleware**:
    ```csharp
    app.UseMiddleware<DeviceLogMiddleware>();
    ```
    *(Note: Place this early in the pipeline, e.g., before Authentication, to log all traffic)*

## 3. Verification Steps

1.  **Code Review**: Check if the logic properly buffers writes (to avoid disk I/O issues).
2.  **Build**: Ensure solution builds without errors.
3.  **Test**:
    *   Run API.
    *   Send requests using Postman (simulating different User-Agents).
    *   Check `API/App_Data/logs/` folder to see if JSON files are created.
    *   Call `GET /api/dashboard/device-stats` to verify data aggregation.
