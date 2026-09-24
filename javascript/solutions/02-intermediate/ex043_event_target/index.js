// Reference solution — exercise 043.

export class Bus extends EventTarget {
  emit(name, detail) {
    return this.dispatchEvent(new CustomEvent(name, { detail }));
  }

  on(name, fn) {
    const handler = (event) => fn(event.detail);
    this.addEventListener(name, handler);
    return () => this.removeEventListener(name, handler);
  }

  once(name, fn) {
    const handler = (event) => fn(event.detail);
    this.addEventListener(name, handler, { once: true });
    return () => this.removeEventListener(name, handler);
  }
}

export function addTwiceCallCount() {
  const bus = new Bus();
  let calls = 0;
  const handler = () => {
    calls += 1;
  };
  bus.addEventListener("ping", handler);
  bus.addEventListener("ping", handler); // ignored: same function, same options
  bus.emit("ping", null);
  return calls;
}
