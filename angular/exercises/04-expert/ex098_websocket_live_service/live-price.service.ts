import { Injectable, InjectionToken, inject, signal } from "@angular/core";

// Exercise 098 — a live-data service: reconnecting WebSocket wrapper with signal projection (expert).
// Goal:   turn a raw, stateful, callback-based WebSocket into a signal a template can just read —
//         and keep that signal honest across the network blips a live socket actually has: drops,
//         errors, and a backend that takes a few seconds to come back.
// Drills: wrapping a callback-based API behind an injectable abstraction so it is fake-able in
//         tests, exponential reconnect backoff (the same shape as exercise 078's retry, applied to
//         a long-lived connection instead of a single request), and projecting inbound messages
//         onto a signal without ever exposing the raw socket to a consumer.
// Passes: when `npx jest exercises/04-expert/ex098_websocket_live_service` is green.
//
// There is no real WebSocket server in this test environment, and there should not need to be one —
// this service never talks to `window.WebSocket` directly. It depends on `LIVE_SOCKET_FACTORY`, an
// injected function that produces something shaped like a socket (`LiveSocketHandle`). In the app,
// that factory really does `new WebSocket(url)`; in tests, it hands back a plain object the test
// controls by hand, firing `onopen`/`onmessage`/`onerror`/`onclose` itself, exactly the way exercise
// 078 avoided a real HTTP backend by faking `HttpTestingController` requests instead of a server.
// This is what makes reconnect/backoff timing deterministic under Jest rather than a real multi-
// second flake.
//
// A live socket that goes down is not necessarily gone for good, so a single dropped connection
// must not leave `status` stuck at "closed" forever — `onclose` (unless it followed a deliberate
// `disconnect()`) schedules a fresh `connect()` after a backoff delay that DOUBLES on every
// consecutive failure (`RECONNECT_CONFIG.baseDelayMs`, then `* 2`, then `* 4`, ...) for the same
// reason exercise 078's retry backoff grows: reconnecting at a fixed rate keeps hammering a backend
// that is still recovering. Once `RECONNECT_CONFIG.maxAttempts` consecutive reconnects have all
// failed, the service gives up and settles at "closed" — it does not retry forever.
//
// `disconnect()` is the other branch `onclose` must recognize: a socket the CALLER intentionally
// closed must settle at "closed" and must NOT trigger a reconnect — the flag that distinguishes
// "the network dropped us" from "we hung up on purpose" has to be set before `close()` is even
// called, because the close event (real or faked) arrives asynchronously afterward.

export interface PriceTick {
  readonly symbol: string;
  readonly price: number;
}

/** The subset of the real WebSocket surface this service depends on — small and fake-able. */
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
  /** Consecutive reconnect attempts allowed after the first connection, not counting it. */
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

  /**
   * TODO: implement connect.
   *   - Clear the manual-close flag (a fresh `connect()` is always a deliberate (re)connection).
   *   - Set status to "connecting".
   *   - Create a socket via `createSocket()`, store it, and wire its callbacks (see `wireSocket`).
   */
  connect(): void {
    throw new Error("TODO: implement connect");
  }

  /**
   * TODO: implement disconnect.
   *   - Mark this as a manual close BEFORE calling `close()` — the close event (real or faked)
   *     always arrives after `close()` returns, so `handleClose` needs the flag set in advance to
   *     tell "we hung up" apart from "the network dropped us".
   *   - Call `close()` on the current socket, if there is one.
   */
  disconnect(): void {
    throw new Error("TODO: implement disconnect");
  }

  /**
   * TODO: implement wireSocket.
   *   - onopen: set status to "open" and reset reconnectAttempts to 0 (a successful open means
   *     whatever trouble caused a previous reconnect is over).
   *   - onmessage: JSON.parse `event.data` as a PriceTick and set it on latestTick.
   *   - onerror: nothing to do here directly — a real WebSocket always follows an error with a
   *     close event, and `handleClose` is where reconnect decisions belong.
   *   - onclose: call `handleClose()`.
   */
  private wireSocket(socket: LiveSocketHandle): void {
    throw new Error("TODO: implement wireSocket");
  }

  /**
   * TODO: implement handleClose.
   *   - If this was a manual close (disconnect()), set status to "closed" and stop — no reconnect.
   *   - If reconnectAttempts has already reached reconnectConfig.maxAttempts, give up: set status
   *     to "closed" and stop.
   *   - Otherwise: set status to "reconnecting", increment reconnectAttempts, and schedule a fresh
   *     `connect()` after `reconnectConfig.baseDelayMs * 2 ** (reconnectAttempts() - 1)` — using the
   *     just-incremented attempt count, so the first reconnect waits baseDelayMs, the second waits
   *     baseDelayMs * 2, and so on.
   */
  private handleClose(): void {
    throw new Error("TODO: implement handleClose");
  }
}
