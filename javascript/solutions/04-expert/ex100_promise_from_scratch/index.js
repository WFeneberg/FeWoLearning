// Reference solution — exercise 100.

const PENDING = "pending";
const FULFILLED = "fulfilled";
const REJECTED = "rejected";

export class Thenable {
  #state = PENDING;
  #value;
  #callbacks = [];

  constructor(executor) {
    const settle = (state, value) => {
      if (this.#state !== PENDING) return; // settle once, silently
      this.#state = state;
      this.#value = value;
      for (const callback of this.#callbacks) queueMicrotask(callback);
      this.#callbacks = [];
    };

    const resolve = (value) => {
      // Adoption: resolving with a thenable waits for it.
      if (value === this) {
        settle(REJECTED, new TypeError("cannot resolve a promise with itself"));
        return;
      }
      const then = typeof value?.then === "function" ? value.then : null;
      if (then === null) {
        settle(FULFILLED, value);
        return;
      }
      let handled = false;
      try {
        then.call(
          value,
          (inner) => {
            if (handled) return;
            handled = true;
            resolve(inner);
          },
          (reason) => {
            if (handled) return;
            handled = true;
            settle(REJECTED, reason);
          },
        );
      } catch (error) {
        if (!handled) settle(REJECTED, error);
      }
    };

    try {
      executor(resolve, (reason) => settle(REJECTED, reason));
    } catch (error) {
      settle(REJECTED, error);
    }
  }

  then(onFulfilled, onRejected) {
    return new Thenable((resolve, reject) => {
      const run = () => {
        const handler = this.#state === FULFILLED ? onFulfilled : onRejected;
        if (typeof handler !== "function") {
          // Pass-through: the missing handler forwards the outcome.
          if (this.#state === FULFILLED) resolve(this.#value);
          else reject(this.#value);
          return;
        }
        try {
          resolve(handler(this.#value));
        } catch (error) {
          reject(error);
        }
      };
      // Always a microtask, settled or not.
      if (this.#state === PENDING) this.#callbacks.push(run);
      else queueMicrotask(run);
    });
  }

  catch(onRejected) {
    return this.then(undefined, onRejected);
  }

  finally(onFinally) {
    return this.then(
      (value) => {
        onFinally();
        return value;
      },
      (reason) => {
        onFinally();
        throw reason;
      },
    );
  }

  static resolve(value) {
    if (value instanceof Thenable) return value;
    return new Thenable((resolve) => resolve(value));
  }

  static reject(reason) {
    return new Thenable((_resolve, reject) => reject(reason));
  }

  static all(values) {
    return new Thenable((resolve, reject) => {
      const list = [...values];
      const results = new Array(list.length);
      let remaining = list.length;
      if (remaining === 0) {
        resolve(results);
        return;
      }
      list.forEach((value, index) => {
        Thenable.resolve(value).then((settled) => {
          results[index] = settled;
          if (--remaining === 0) resolve(results);
        }, reject);
      });
    });
  }
}
