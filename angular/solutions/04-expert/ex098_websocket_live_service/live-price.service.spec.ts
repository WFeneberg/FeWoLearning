import { TestBed } from "@angular/core/testing";
import {
  LIVE_SOCKET_FACTORY,
  LiveSocketHandle,
  LivePriceService,
  RECONNECT_CONFIG,
} from "./live-price.service";

class FakeSocket implements LiveSocketHandle {
  onopen: ((event: Event) => void) | null = null;
  onmessage: ((event: MessageEvent<string>) => void) | null = null;
  onerror: ((event: Event) => void) | null = null;
  onclose: ((event: CloseEvent) => void) | null = null;
  closeCalls = 0;

  close(): void {
    this.closeCalls++;
  }

  open(): void {
    this.onopen?.(new Event("open"));
  }

  message(tick: { symbol: string; price: number }): void {
    this.onmessage?.(new MessageEvent("message", { data: JSON.stringify(tick) }));
  }

  fireClose(): void {
    this.onclose?.(new CloseEvent("close"));
  }
}

/** Let a real (tiny) backoff timer elapse before asserting on the next reconnect attempt. */
const settle = () => new Promise<void>((resolve) => setTimeout(resolve, 20));

describe("LivePriceService (fake-socket reconnect + signal projection)", () => {
  let sockets: FakeSocket[];
  let service: LivePriceService;

  beforeEach(() => {
    sockets = [];
    TestBed.configureTestingModule({
      providers: [
        LivePriceService,
        {
          provide: LIVE_SOCKET_FACTORY,
          useValue: () => {
            const socket = new FakeSocket();
            sockets.push(socket);
            return socket;
          },
        },
        { provide: RECONNECT_CONFIG, useValue: { maxAttempts: 2, baseDelayMs: 1 } },
      ],
    });
    service = TestBed.inject(LivePriceService);
  });

  it("starts closed and moves to connecting, then open, on connect()", () => {
    expect(service.status()).toBe("closed");

    service.connect();
    expect(service.status()).toBe("connecting");
    expect(sockets).toHaveLength(1);

    sockets[0].open();
    expect(service.status()).toBe("open");
  });

  it("projects an inbound message onto latestTick", () => {
    service.connect();
    sockets[0].open();

    sockets[0].message({ symbol: "ACME", price: 12.5 });

    expect(service.latestTick()).toEqual({ symbol: "ACME", price: 12.5 });
  });

  it("reconnects with backoff after an unexpected close, opening a fresh socket", async () => {
    service.connect();
    sockets[0].open();

    sockets[0].fireClose();
    expect(service.status()).toBe("reconnecting");
    expect(service.reconnectAttempts()).toBe(1);

    await settle();

    expect(sockets).toHaveLength(2);
    sockets[1].open();
    expect(service.status()).toBe("open");
    expect(service.reconnectAttempts()).toBe(0); // a successful open clears the failure count
  });

  it("gives up after maxAttempts consecutive failures and settles at closed", async () => {
    service.connect();
    sockets[0].fireClose(); // attempt 1
    await settle();
    expect(sockets).toHaveLength(2);

    sockets[1].fireClose(); // attempt 2 (maxAttempts)
    await settle();
    expect(sockets).toHaveLength(3);

    sockets[2].fireClose(); // exceeds maxAttempts — give up
    expect(service.status()).toBe("closed");

    await settle();
    expect(sockets).toHaveLength(3); // no further reconnect was scheduled
  });

  it("disconnect() closes the socket and suppresses reconnection", async () => {
    service.connect();
    sockets[0].open();

    service.disconnect();
    expect(sockets[0].closeCalls).toBe(1);

    sockets[0].fireClose();
    expect(service.status()).toBe("closed");

    await settle();
    expect(sockets).toHaveLength(1); // disconnect must not trigger a reconnect
  });

  it("does not project a message that arrives after the socket is considered closed by the app", () => {
    service.connect();
    sockets[0].open();
    sockets[0].message({ symbol: "FIRST", price: 1 });

    service.disconnect();
    sockets[0].fireClose();

    expect(service.latestTick()).toEqual({ symbol: "FIRST", price: 1 });
    expect(service.status()).toBe("closed");
  });
});
