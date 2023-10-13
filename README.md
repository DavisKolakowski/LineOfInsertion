# Line Of Insertion (LOI)

The Line Of Insertion (LOI) project is a comprehensive remote file watcher system designed to monitor file system events across multiple machines. The system comprises an API that manages watched folders and the state of each worker, and individual workers installed on remote machines that report back to the API.

## Components

### NFXFileSystemEventWatcherManager API

**Purpose**: Central control hub for managing watched folders and monitoring the state of each worker on remote machines.

**Key Features**:
- **Controllers**: The `NFXFileEventSystemController` provides endpoints to manage and monitor file system watchers.
- **Data Context**: The `LineOfInsertionDbContext` facilitates database operations and interaction with the underlying tables.
- **Real-time Communication**: The `WorkerHub` manages SignalR connections for real-time updates from workers.

### NFXFileSystemEventWatcher Service (Worker)

**Purpose**: Installed on remote machines, these workers monitor specified folders and communicate file system events back to the API.

**Identification**: Each worker is uniquely identified by its machine name.

**Key Features**:
- **Event Monitoring**: The `Worker` class monitors file system events, capturing changes in the watched directories.
- **State Management**: The `ApplicationStateManager` class manages the state of the worker, controlling the folders it watches and relaying events back to the API.

## System Flow

### Setup:

1. Configure the database connection string in the API's `secrets.json`.
2. Configure the SignalR hub URL in the Service's `appsettings.json`.

### Operation:

1. Run the API project.
2. Install and run the Worker service on each remote machine you want to monitor.
3. Use the API to specify folders to watch on each remote machine.
4. The API provides real-time updates on file system events from each worker.

### Management:

1. The API allows users to start, pause, or stop watching specific folders on any connected remote machine.
2. Each worker's state (e.g., active, paused) is also managed and monitored via the API. This is facilitated by the `ApplicationStateManager` in conjunction with the API's controllers.

---

Please note that to fully utilize this system, ensure that the network permissions and firewall settings on the remote machines are configured to allow communication with the central API.