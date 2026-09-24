// Reference solution — exercise 080.

export function createStream(producer) {
  return {
    subscribe(observer) {
      let active = true;
      let teardown;

      const stop = () => {
        if (!active) return;
        active = false;
        teardown?.();
        teardown = undefined;
      };

      teardown = producer({
        next: (value) => {
          if (active) observer.next?.(value);
        },
        complete: () => {
          if (!active) return;
          const done = observer.complete;
          stop();
          done?.();
        },
        error: (reason) => {
          if (!active) return;
          const fail = observer.error;
          stop();
          fail?.(reason);
        },
      });

      // The producer may have completed synchronously, in which case stop()
      // already ran and there is nothing left to tear down.
      if (!active) {
        teardown?.();
        teardown = undefined;
      }
      return stop;
    },
  };
}

export function createSubject() {
  const observers = new Set();
  let done = false;
  return {
    subscribe(observer) {
      if (done) return () => undefined;
      observers.add(observer);
      return () => observers.delete(observer);
    },
    next(value) {
      if (done) return;
      for (const observer of [...observers]) observer.next?.(value);
    },
    complete() {
      if (done) return;
      done = true;
      for (const observer of [...observers]) observer.complete?.();
      observers.clear();
    },
  };
}
