import { Injectable, InjectionToken, inject, signal } from "@angular/core";

// Exercise 098 — a live-data service: reconnecting WebSocket wrapper with signal projection
// (reference solution).

export interface PriceTick {
  readonly symbol: string;
  readonly price: number;
}

export interface LiveSocketHandle {
  onopen: ((event: Event) => void) | null;
  onmessage: ((event: MessageEvent<string>) => void) | null;
  onerror: ((event: Event) => void) | null;
  onclose: ((event: CloseEvent) => void) | null;
  close(): void;
}

export type LiveSocketFactory = () => LiveSocketHandle;

export const LIVE_SOCKET_FACTORY = new InjectionToken<LiveSocketFactory>("LIVE_SOCKET_FACTORY");

export interface ReconnectConfig {
  readonly maxAttempts: number;
  readonly baseDelayMs: number;
}

export const RECONNECT_CONFIG = new InjectionToken<ReconnectConfig>("RECONNECT_CONFIG", {
  factory: (): ReconnectConfig => ({ maxAttempts: 5, baseDelayMs: 1000 }),
});

export type ConnectionStatus = "connecting" | "open" | "reconnecting" | "closed";

@Injectable()
export class LivePriceService {
  private readonly createSocket = inject(LIVE_SOCKET_FACTORY);
  private readonly reconnectConfig = inject(RECONNECT_CONFIG);

  private socket: LiveSocketHandle | null = null;
  private manuallyClosed = false;

  readonly status = signal<ConnectionStatus>("closed");
  readonly latestTick = signal<PriceTick | null>(null);
  readonly reconnectAttempts = signal(0);

  connect(): void {
    this.manuallyClosed = false;
    this.status.set("connecting");
    const socket = this.createSocket();
    this.socket = socket;
    this.wireSocket(socket);
  }

  disconnect(): void {
    // Set before close() — the (real or faked) close event always arrives after this call returns.
    this.manuallyClosed = true;
    this.socket?.close();
  }

  private wireSocket(socket: LiveSocketHandle): void {
    socket.onopen = () => {
      this.status.set("open");
      this.reconnectAttempts.set(0);
    };
    socket.onmessage = (event) => {
      this.latestTick.set(JSON.parse(event.data) as PriceTick);
    };
    socket.onerror = () => {
      // A real WebSocket always follows an error with a close event — reconnect decisions live
      // in handleClose, not here, so both paths funnel through one place.
    };
    socket.onclose = () => {
      this.handleClose();
    };
  }

  private handleClose(): void {
    if (this.manuallyClosed) {
      this.status.set("closed");
      return;
    }
    if (this.reconnectAttempts() >= this.reconnectConfig.maxAttempts) {
      this.status.set("closed");
      return;
    }
    this.status.set("reconnecting");
    const attempt = this.reconnectAttempts() + 1;
    this.reconnectAttempts.set(attempt);
    const delay = this.reconnectConfig.baseDelayMs * 2 ** (attempt - 1);
    setTimeout(() => this.connect(), delay);
  }
}
