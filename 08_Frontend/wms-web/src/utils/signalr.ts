import { ref, onUnmounted } from 'vue';

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

class WebSocketSignalRConnection implements SignalRConnection {
  private ws: WebSocket | null = null;
  private listeners: Record<string, ((...args: any[]) => void)[]> = {};
  private url: string;
  private started = false;

  constructor(url: string) {
    this.url = url;
  }

  async start(): Promise<void> {
    if (this.started || typeof WebSocket === 'undefined') return;
    return new Promise((resolve, reject) => {
      try {
        this.ws = new WebSocket(this.url);
        this.ws.onopen = () => {
          this.started = true;
          resolve();
        };
        this.ws.onerror = (err) => {
          console.warn('SignalR WebSocket error:', err);
          reject(err);
        };
        this.ws.onmessage = (event) => {
          try {
            const message = JSON.parse(event.data);
            if (message.method && this.listeners[message.method]) {
              this.listeners[message.method].forEach((cb) => cb(...(message.args || [])));
            }
          } catch {
            // Ignore non-JSON messages
          }
        };
        this.ws.onclose = () => {
          this.started = false;
        };
      } catch (err) {
        reject(err);
      }
    });
  }

  async stop(): Promise<void> {
    this.ws?.close();
    this.ws = null;
    this.started = false;
  }

  on(method: string, callback: (...args: any[]) => void): void {
    if (!this.listeners[method]) this.listeners[method] = [];
    this.listeners[method].push(callback);
  }

  off(method: string, callback?: (...args: any[]) => void): void {
    if (!this.listeners[method]) return;
    if (callback) {
      this.listeners[method] = this.listeners[method].filter((cb) => cb !== callback);
    } else {
      this.listeners[method] = [];
    }
  }

  async invoke(method: string, ...args: any[]): Promise<any> {
    if (this.ws?.readyState === WebSocket.OPEN) {
      this.ws.send(JSON.stringify({ method, args }));
    }
  }
}

export function useSignalR(hubUrl: string): SignalRHook {
  const connected = ref(false);
  const connection = ref<SignalRConnection | null>(null) as import('vue').Ref<SignalRConnection | null>;
  const baseUrl = import.meta.env.VITE_API_BASE_URL || window.location.origin;
  const fullUrl = `${baseUrl.replace(/\/$/, '')}${hubUrl}`;

  const conn = new WebSocketSignalRConnection(fullUrl);
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
