// Exercise 098 — WebSocket-backed live store (reference solution).
import { computed, ref, type ComputedRef, type Ref } from "vue";

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
  pending: boolean;
}

export type LiveStatus = "connecting" | "open" | "closed";

export interface LiveStoreOptions {
  connect: () => SocketLike;
  maxRetries?: number;
}

export interface LiveStore {
  items: Ref<LiveItem[]>;
  status: Ref<LiveStatus>;
  retries: Ref<number>;
  pendingCount: ComputedRef<number>;
  add: (id: string, text: string) => void;
  dispose: () => void;
}

interface AddMessage {
  type: "add";
  id: string;
  text: string;
}

interface RemoveMessage {
  type: "remove";
  id: string;
}

type ServerMessage = AddMessage | RemoveMessage;

/** Returns null for anything that is not a message shape we understand. */
function parseMessage(raw: string): ServerMessage | null {
  let value: unknown;
  try {
    value = JSON.parse(raw);
  } catch {
    return null;
  }
  if (typeof value !== "object" || value === null) return null;

  // Not `Partial<AddMessage & RemoveMessage>`: intersecting the two literal
  // `type` fields collapses them to `never`, which poisons every access below.
  const msg = value as { type?: unknown; id?: unknown; text?: unknown };

  if (msg.type === "add" && typeof msg.id === "string" && typeof msg.text === "string") {
    return { type: "add", id: msg.id, text: msg.text };
  }
  if (msg.type === "remove" && typeof msg.id === "string") {
    return { type: "remove", id: msg.id };
  }
  return null;
}

export function useLiveStore(options: LiveStoreOptions): LiveStore {
  const maxRetries = options.maxRetries ?? 3;

  const items = ref<LiveItem[]>([]) as Ref<LiveItem[]>;
  const status = ref<LiveStatus>("connecting");
  const retries = ref(0);

  let socket: SocketLike | null = null;
  // Set by dispose() so a close we caused never triggers a reconnect.
  let disposed = false;

  const pendingCount = computed(() => items.value.filter((i) => i.pending).length);

  const handle = (message: ServerMessage): void => {
    if (message.type === "add") {
      const existing = items.value.find((i) => i.id === message.id);
      if (existing) {
        // The echo of an optimistic add: reconcile instead of duplicating.
        existing.pending = false;
        existing.text = message.text;
      } else {
        items.value.push({ id: message.id, text: message.text, pending: false });
      }
      return;
    }
    items.value = items.value.filter((i) => i.id !== message.id);
  };

  const open = (): void => {
    status.value = "connecting";
    const s = options.connect();
    socket = s;

    s.onopen = () => {
      status.value = "open";
    };

    s.onmessage = (event) => {
      const message = parseMessage(event.data);
      if (message) handle(message);
    };

    s.onclose = () => {
      if (disposed) return;
      if (retries.value < maxRetries) {
        retries.value += 1;
        open();
        return;
      }
      status.value = "closed";
    };
  };

  const add = (id: string, text: string): void => {
    items.value.push({ id, text, pending: true });
    if (status.value === "open") {
      socket?.send(JSON.stringify({ type: "add", id, text }));
    }
  };

  const dispose = (): void => {
    disposed = true;
    status.value = "closed";
    socket?.close();
  };

  open();

  return { items, status, retries, pendingCount, add, dispose };
}
