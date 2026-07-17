import * as signalR from '@microsoft/signalr';

export type HubName = 'InventoryHub' | 'AlertHub' | 'TaskHub' | 'NotificationHub';

export type ConnectionState = 'connected' | 'connecting' | 'disconnected' | 'reconnecting';

interface HubEntry {
  url: string;
  connection: signalR.HubConnection | null;
}

const HUB_PATHS: Record<HubName, string> = {
  InventoryHub: '/signalr/inventory',
  AlertHub: '/signalr/alert',
  TaskHub: '/signalr/task',
  NotificationHub: '/signalr/notification',
};

const RECONNECT_DELAYS = [0, 1000, 2000, 5000, 10000, 30000];

class SignalRService {
  private readonly hubs: Record<HubName, HubEntry>;

  constructor() {
    const baseUrl = import.meta.env.VITE_SIGNALR_BASE_URL || '';
    this.hubs = {
      InventoryHub: { url: `${baseUrl}${HUB_PATHS.InventoryHub}`, connection: null },
      AlertHub: { url: `${baseUrl}${HUB_PATHS.AlertHub}`, connection: null },
      TaskHub: { url: `${baseUrl}${HUB_PATHS.TaskHub}`, connection: null },
      NotificationHub: { url: `${baseUrl}${HUB_PATHS.NotificationHub}`, connection: null },
    };
  }

  private buildConnection(url: string): signalR.HubConnection {
    return new signalR.HubConnectionBuilder()
      .withUrl(url, {
        accessTokenFactory: () => localStorage.getItem('wms_token') || '',
      })
      .withAutomaticReconnect(RECONNECT_DELAYS)
      .configureLogging(signalR.LogLevel.Warning)
      .build();
  }

  async connect(hubName: HubName): Promise<signalR.HubConnection> {
    const entry = this.hubs[hubName];
    if (entry.connection && entry.connection.state === signalR.HubConnectionState.Connected) {
      return entry.connection;
    }

    const connection = this.buildConnection(entry.url);
    entry.connection = connection;

    await connection.start();
    return connection;
  }

  async connectAll(): Promise<void> {
    const names = Object.keys(this.hubs) as HubName[];
    await Promise.all(names.map((name) => this.connect(name)));
  }

  async disconnect(hubName: HubName): Promise<void> {
    const entry = this.hubs[hubName];
    if (entry.connection) {
      await entry.connection.stop();
      entry.connection = null;
    }
  }

  async disconnectAll(): Promise<void> {
    const names = Object.keys(this.hubs) as HubName[];
    await Promise.all(names.map((name) => this.disconnect(name)));
  }

  on<T = unknown>(hubName: HubName, methodName: string, callback: (data: T) => void): void {
    const entry = this.hubs[hubName];
    if (!entry.connection) {
      throw new Error(`Hub ${hubName} is not connected`);
    }
    entry.connection.on(methodName, callback as (...args: any[]) => void);
  }

  off(hubName: HubName, methodName: string, callback?: (...args: any[]) => void): void {
    const entry = this.hubs[hubName];
    if (!entry.connection) return;
    if (callback) {
      entry.connection.off(methodName, callback);
    } else {
      entry.connection.off(methodName);
    }
  }

  async invoke<T = unknown>(hubName: HubName, methodName: string, ...args: any[]): Promise<T> {
    const entry = this.hubs[hubName];
    if (!entry.connection) {
      throw new Error(`Hub ${hubName} is not connected`);
    }
    return entry.connection.invoke(methodName, ...args) as Promise<T>;
  }

  getState(hubName: HubName): ConnectionState {
    const entry = this.hubs[hubName];
    if (!entry.connection) return 'disconnected';
    switch (entry.connection.state) {
      case signalR.HubConnectionState.Connected:
        return 'connected';
      case signalR.HubConnectionState.Connecting:
        return 'connecting';
      case signalR.HubConnectionState.Reconnecting:
        return 'reconnecting';
      default:
        return 'disconnected';
    }
  }

  isConnected(hubName: HubName): boolean {
    return this.getState(hubName) === 'connected';
  }
}

export const signalRService = new SignalRService();

export default signalRService;
