import { describe, expect, it, vi } from "vitest";
import { useLiveStore, type SocketLike } from "./useLiveStore";

/** A hand-driven stand-in for WebSocket: the test decides when things happen. */
class FakeSocket implements SocketLike {
  onopen: (() => void) | null = null;
  onclose: (() => void) | null = null;
  onmessage: ((event: { data: string }) => void) | null = null;
  readonly sent: string[] = [];
  closed = false;

  send(data: string): void {
    this.sent.push(data);
  }

  close(): void {
    this.closed = true;
  }

  /* --- test-side helpers --- */
  open(): void {
    this.onopen?.();
  }

  emit(payload: unknown): void {
    this.onmessage?.({ data: JSON.stringify(payload) });
  }

  emitRaw(data: string): void {
    this.onmessage?.({ data });
  }

  drop(): void {
    this.onclose?.();
  }
}

/** Returns the store plus every socket the factory handed out, in order. */
function setup(maxRetries?: number) {
  const sockets: FakeSocket[] = [];
  const connect = vi.fn(() => {
    const s = new FakeSocket();
    sockets.push(s);
    return s;
  });
  const store = useLiveStore(maxRetries === undefined ? { connect } : { connect, maxRetries });
  return { store, sockets, connect };
}

describe("useLiveStore", () => {
  it("connects immediately and reports connecting until the socket opens", () => {
    const { store, connect, sockets } = setup();

    expect(connect).toHaveBeenCalledTimes(1);
    expect(store.status.value).toBe("connecting");

    sockets[0].open();
    expect(store.status.value).toBe("open");
  });

  it("appends items the server sends as already confirmed", () => {
    const { store, sockets } = setup();
    sockets[0].open();

    sockets[0].emit({ type: "add", id: "a", text: "from server" });

    expect(store.items.value).toEqual([{ id: "a", text: "from server", pending: false }]);
    expect(store.pendingCount.value).toBe(0);
  });

  it("removes items on a remove message and ignores unknown ids", () => {
    const { store, sockets } = setup();
    sockets[0].open();
    sockets[0].emit({ type: "add", id: "a", text: "one" });
    sockets[0].emit({ type: "add", id: "b", text: "two" });

    sockets[0].emit({ type: "remove", id: "a" });
    sockets[0].emit({ type: "remove", id: "does-not-exist" });

    expect(store.items.value.map((i) => i.id)).toEqual(["b"]);
  });

  it("adds locally as pending and sends the add over the socket", () => {
    const { store, sockets } = setup();
    sockets[0].open();

    store.add("x", "typed locally");

    expect(store.items.value).toEqual([{ id: "x", text: "typed locally", pending: true }]);
    expect(store.pendingCount.value).toBe(1);
    expect(sockets[0].sent).toEqual([JSON.stringify({ type: "add", id: "x", text: "typed locally" })]);
  });

  it("confirms an optimistic add when the server echoes it, without duplicating", () => {
    const { store, sockets } = setup();
    sockets[0].open();
    store.add("x", "typed locally");

    sockets[0].emit({ type: "add", id: "x", text: "typed locally" });

    expect(store.items.value).toHaveLength(1);
    expect(store.items.value[0].pending).toBe(false);
    expect(store.pendingCount.value).toBe(0);
  });

  it("ignores malformed JSON instead of throwing", () => {
    const { store, sockets } = setup();
    sockets[0].open();
    sockets[0].emit({ type: "add", id: "a", text: "one" });

    expect(() => sockets[0].emitRaw("{not json")).not.toThrow();
    expect(store.items.value).toHaveLength(1);
  });

  it("reconnects after an unexpected close, up to maxRetries", () => {
    const { store, sockets, connect } = setup(2);
    sockets[0].open();

    sockets[0].drop();
    expect(connect).toHaveBeenCalledTimes(2);
    expect(store.retries.value).toBe(1);
    expect(store.status.value).toBe("connecting");

    sockets[1].open();
    sockets[1].drop();
    expect(connect).toHaveBeenCalledTimes(3);
    expect(store.retries.value).toBe(2);

    // Budget exhausted: no further attempt, and the store settles as closed.
    sockets[2].open();
    sockets[2].drop();
    expect(connect).toHaveBeenCalledTimes(3);
    expect(store.status.value).toBe("closed");
  });

  it("keeps items across a reconnect and can confirm them afterwards", () => {
    const { store, sockets } = setup(1);
    sockets[0].open();
    store.add("x", "survives");

    sockets[0].drop();
    sockets[1].open();
    sockets[1].emit({ type: "add", id: "x", text: "survives" });

    expect(store.items.value).toHaveLength(1);
    expect(store.items.value[0].pending).toBe(false);
  });

  it("dispose closes the socket and suppresses reconnecting", () => {
    const { store, sockets, connect } = setup(5);
    sockets[0].open();

    store.dispose();

    expect(sockets[0].closed).toBe(true);
    expect(store.status.value).toBe("closed");

    // Even if the transport reports the close afterwards, no new socket appears.
    sockets[0].drop();
    expect(connect).toHaveBeenCalledTimes(1);
    expect(store.status.value).toBe("closed");
  });

  it("still records a local add while the socket is not open", () => {
    const { store, sockets } = setup();

    store.add("y", "queued");

    expect(store.items.value[0]).toEqual({ id: "y", text: "queued", pending: true });
    expect(sockets[0].sent).toHaveLength(0);
  });
});
