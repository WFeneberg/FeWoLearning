// Reference solution — exercise 099.

const READ = 0;
const WRITE = 1;
const DATA = 2;

export function createRing(capacity) {
  // One slot is sacrificed so that "write === read" can mean empty and
  // nothing else.
  const slots = capacity + 1;
  const view = new Int32Array(new SharedArrayBuffer((DATA + slots) * 4));

  const next = (index) => (index + 1) % slots;

  return {
    capacity,
    get size() {
      const read = Atomics.load(view, READ);
      const write = Atomics.load(view, WRITE);
      return (write - read + slots) % slots;
    },
    push(value) {
      const write = Atomics.load(view, WRITE);
      const after = next(write);
      if (after === Atomics.load(view, READ)) return false; // full
      Atomics.store(view, DATA + write, value);
      Atomics.store(view, WRITE, after);
      return true;
    },
    shift() {
      const read = Atomics.load(view, READ);
      if (read === Atomics.load(view, WRITE)) return undefined; // empty
      const value = Atomics.load(view, DATA + read);
      Atomics.store(view, READ, next(read));
      return value;
    },
  };
}

export function countWithAtomics(times) {
  const view = new Int32Array(new SharedArrayBuffer(4));
  for (let i = 0; i < times; i++) Atomics.add(view, 0, 1);
  return Atomics.load(view, 0);
}

export function tryCompareExchange(view, expected, next) {
  const previous = Atomics.compareExchange(view, 0, expected, next);
  return { previous, written: previous === expected };
}
