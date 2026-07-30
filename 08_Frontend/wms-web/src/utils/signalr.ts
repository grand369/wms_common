import { ref, onUnmounted } from 'vue';
import * as signalR from '@microsoft/signalr';

export interface SignalRConnection {
  on(method: string, callback: (...args: any[]) => void): void;
  off(method: string, callback?: (...args: any[]) => void): void;
  invoke(method: string, ...args: any[]): Promise<any>;
  start(): Promise<void>;
  stop(): Promise<void>;
}

interface SignalRHook {
  connected: import('vue').Ref<boolean>;
  connection: SignalRConnection | null;
  on: (method: string, callback: (...args: any[]) => void) => void;
  off: (method: string, callback?: (...args: any[]) => void) => void;
  invoke: (method: string, ...args: any[]) => Promise<any>;
}

class SignalRHubConnection implements SignalRConnection {
  private connection: signalR.HubConnection | null = null;
  private url: string;
  private started = false;

  constructor(url: string) {
    this.url = url;
  }

  async start(): Promise<void> {
    if (this.started) return;
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.url, {
        accessTokenFactory: () => localStorage.getItem('wms_token') || '',
      })
      .withAutomaticReconnect([0, 1000, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    await this.connection.start();
    this.started = true;
  }

  async stop(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
    this.started = false;
  }

  on(method: string, callback: (...args: any[]) => void): void {
    if (this.connection) {
      this.connection.on(method, callback);
    }
  }

  off(method: string, callback?: (...args: any[]) => void): void {
    if (this.connection) {
      if (callback) {
        this.connection.off(method, callback);
      } else {
        this.connection.off(method);
      }
    }
  }

  async invoke(method: string, ...args: any[]): Promise<any> {
    if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
      return await this.connection.invoke(method, ...args);
    }
  }
}

export function useSignalR(hubUrl: string): SignalRHook {
  const connected = ref(false);
  const connection = ref<SignalRConnection | null>(null) as import('vue').Ref<SignalRConnection | null>;
  const baseUrl = import.meta.env.VITE_API_BASE_URL || window.location.origin;
  const fullUrl = `${baseUrl.replace(/\/$/, '')}${hubUrl}`;

  const conn = new SignalRHubConnection(fullUrl);
  connection.value = conn;

  conn.start().then(() => {
    connected.value = true;
  }).catch(() => {
    connected.value = false;
  });

  onUnmounted(() => {
    conn.stop().catch(() => {});
  });

  return {
    connected,
    connection: connection.value,
    on: (method: string, callback: (...args: any[]) => void) => conn.on(method, callback),
    off: (method: string, callback?: (...args: any[]) => void) => conn.off(method, callback),
    invoke: (method: string, ...args: any[]) => conn.invoke(method, ...args),
  };
}
