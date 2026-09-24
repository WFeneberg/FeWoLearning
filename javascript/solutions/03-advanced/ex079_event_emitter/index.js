// Reference solution — exercise 079.

export class Emitter {
  #listeners = new Map(); // event -> [{ fn, original }]

  #entries(event) {
    let list = this.#listeners.get(event);
    if (list === undefined) {
      list = [];
      this.#listeners.set(event, list);
    }
    return list;
  }

  on(event, listener) {
    this.#entries(event).push({ fn: listener, original: listener });
    return this;
  }

  once(event, listener) {
    const wrapper = (...args) => {
      this.off(event, listener);
      listener(...args);
    };
    // `original` is what off() matches on, so a once() listener can be
    // removed by the function the caller actually passed in.
    this.#entries(event).push({ fn: wrapper, original: listener });
    return this;
  }

  off(event, listener) {
    const list = this.#listeners.get(event);
    if (list === undefined) return this;
    const index = list.findIndex((entry) => entry.original === listener);
    if (index !== -1) list.splice(index, 1);
    return this;
  }

  emit(event, ...args) {
    // Copy first: a listener that subscribes or unsubscribes during the
    // emit must not change this round.
    const list = [...(this.#listeners.get(event) ?? [])];
    const errors = [];
    for (const entry of list) {
      try {
        entry.fn(...args);
      } catch (error) {
        errors.push(error);
      }
    }
    if (errors.length > 0) throw new AggregateError(errors, `listener error in "${event}"`);
    return list.length;
  }

  listenerCount(event) {
    return this.#listeners.get(event)?.length ?? 0;
  }
}
