// Exercise 098 — WebSocket-backed live store (expert).
// Goal:   a reactive store fed by a socket. Items arriving from the server land
//         in reactive state; local additions appear immediately as *pending* and
//         are confirmed when the server echoes them back; a dropped connection
//         reconnects up to a limit.
// Drills: injecting a socket factory so the store is testable without a network,
//         optimistic updates and their reconciliation, connection state machines,
//         bounded retry, cleaning up on teardown.
import { type ComputedRef, type Ref } from "vue";

/** The slice of WebSocket this store depends on. Tests supply a fake. */
export interface SocketLike {
  send: (data: string) => void;
  close: () => void;
  onopen: (() => void) | null;
  onclose: (() => void) | null;
  onmessage: ((event: { data: string }) => void) | null;
}

export interface LiveItem {
  id: string;
  text: string;
  /** True while the server has not yet confirmed a locally added item. */
  pending: boolean;
}

export type LiveStatus = "connecting" | "open" | "closed";

export interface LiveStoreOptions {
  /** Called once per connection attempt. Must return a fresh socket. */
  connect: () => SocketLike;
  /** How many times to reconnect after an unexpected close. Default 3. */
  maxRetries?: number;
}

export interface LiveStore {
  items: Ref<LiveItem[]>;
  status: Ref<LiveStatus>;
  /** How many reconnects have been attempted so far. */
  retries: Ref<number>;
  /** Items still awaiting server confirmation. */
  pendingCount: ComputedRef<number>;
  /**
   * Optimistically appends `{ id, text, pending: true }`, and — only while the
   * status is "open" — sends `JSON.stringify({ type: "add", id, text })`.
   *
   * While the socket is not open the item is still added locally and simply stays
   * pending; nothing is written to the socket.
   */
  add: (id: string, text: string) => void;
  /** Closes the socket for good: no reconnect after an explicit close. */
  dispose: () => void;
}

/**
 * Creates the store and connects immediately.
 *
 * Incoming messages are JSON. Two shapes matter:
 *  - `{ type: "add", id, text }` — if an item with that id is already present it
 *    is *confirmed* (`pending` becomes false); otherwise it is appended as a
 *    non-pending item. This is what makes an echo reconcile an optimistic add.
 *  - `{ type: "remove", id }` — drops the item with that id, if present.
 *
 * Malformed JSON must be ignored rather than throwing.
 *
 * Lifecycle: `status` is "connecting" until `onopen`, then "open". On `onclose`,
 * reconnect while `retries < maxRetries`, incrementing `retries` and going back to
 * "connecting"; once the budget is exhausted settle on "closed". `dispose()` closes
 * and suppresses any further reconnect.
 */
export function useLiveStore(_options: LiveStoreOptions): LiveStore {
  throw new Error("TODO: implement useLiveStore");
}
