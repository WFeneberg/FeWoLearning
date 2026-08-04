import { of, Subject } from "rxjs";
import {
  asMoney,
  countInto,
  double,
  evensOnly,
  moneyForEvens,
  moneyForEvensImperative,
  runningTotal,
} from "./number-pipeline";

/** Collect everything a stream emits, synchronously. */
const collect = <T>(build: () => import("rxjs").Observable<T>): T[] => {
  const seen: T[] = [];
  build().subscribe((value) => seen.push(value));
  return seen;
};

describe("double", () => {
  it("doubles each value", () => {
    expect(collect(() => of(1, 2, 3).pipe(double()))).toEqual([2, 4, 6]);
  });

  it("handles an empty stream", () => {
    expect(collect(() => of<number>().pipe(double()))).toEqual([]);
  });
});

describe("evensOnly", () => {
  it("keeps the even values", () => {
    expect(collect(() => of(1, 2, 3, 4).pipe(evensOnly()))).toEqual([2, 4]);
  });

  it("keeps zero", () => {
    expect(collect(() => of(0, 1).pipe(evensOnly()))).toEqual([0]);
  });

  it("emits nothing when none qualify", () => {
    expect(collect(() => of(1, 3, 5).pipe(evensOnly()))).toEqual([]);
  });
});

describe("asMoney", () => {
  it("fixes two decimals", () => {
    expect(collect(() => of(3, 4.5, 0).pipe(asMoney()))).toEqual(["3.00", "4.50", "0.00"]);
  });

  it("rounds", () => {
    expect(collect(() => of(1.239).pipe(asMoney()))).toEqual(["1.24"]);
  });
});

describe("runningTotal", () => {
  it("emits once per input", () => {
    expect(collect(() => of(1, 2, 3).pipe(runningTotal()))).toEqual([1, 3, 6]);
  });

  it("emits before the source completes", () => {
    const source = new Subject<number>();
    const seen: number[] = [];
    source.pipe(runningTotal()).subscribe((total) => seen.push(total));

    source.next(5);
    source.next(5);

    // reduce would still be waiting for a completion that has not happened.
    expect(seen).toEqual([5, 10]);
  });

  it("handles an empty stream", () => {
    expect(collect(() => of<number>().pipe(runningTotal()))).toEqual([]);
  });
});

describe("countInto", () => {
  it("passes every value through unchanged", () => {
    const counter = { count: 0 };

    expect(collect(() => of(1, 2, 3).pipe(countInto(counter)))).toEqual([1, 2, 3]);
  });

  it("counts them", () => {
    const counter = { count: 0 };
    collect(() => of(1, 2, 3).pipe(countInto(counter)));

    expect(counter.count).toBe(3);
  });

  it("counts nothing until something subscribes", () => {
    const counter = { count: 0 };

    of(1, 2, 3).pipe(countInto(counter));

    // pipe() builds a recipe; nothing runs without a subscriber.
    expect(counter.count).toBe(0);
  });

  it("counts only what reaches it", () => {
    const counter = { count: 0 };

    collect(() => of(1, 2, 3, 4).pipe(evensOnly(), countInto(counter)));

    // Position in the pipeline decides what a tap sees.
    expect(counter.count).toBe(2);
  });

  it("counts everything when placed first", () => {
    const counter = { count: 0 };

    collect(() => of(1, 2, 3, 4).pipe(countInto(counter), evensOnly()));

    expect(counter.count).toBe(4);
  });
});

describe("moneyForEvens", () => {
  it("composes the three operators", () => {
    expect(collect(() => moneyForEvens(of(1, 2, 3, 4)))).toEqual(["4.00", "8.00"]);
  });

  it("emits nothing for an all-odd source", () => {
    expect(collect(() => moneyForEvens(of(1, 3)))).toEqual([]);
  });

  it("works on a live stream", () => {
    const source = new Subject<number>();
    const seen: string[] = [];
    moneyForEvens(source).subscribe((value) => seen.push(value));

    source.next(2);
    source.next(3);
    source.next(10);

    expect(seen).toEqual(["4.00", "20.00"]);
  });

  it("does nothing before subscription", () => {
    const source = new Subject<number>();
    moneyForEvens(source);

    source.next(2);

    // Nothing subscribed, so nothing was built to receive it.
    const seen: string[] = [];
    moneyForEvens(source).subscribe((value) => seen.push(value));
    expect(seen).toEqual([]);
  });
});

describe("moneyForEvensImperative", () => {
  it("reaches the same answer", () => {
    expect(moneyForEvensImperative(of(1, 2, 3, 4))).toEqual(["4.00", "8.00"]);
  });

  it("agrees with the composed version", () => {
    const source = [1, 2, 3, 4, 5, 6];

    expect(moneyForEvensImperative(of(...source))).toEqual(
      collect(() => moneyForEvens(of(...source))),
    );
  });

  it("only works because the source is synchronous", () => {
    const source = new Subject<number>();

    // Returns immediately with nothing, and there is no way to wait — which is the whole
    // problem with doing the work in the callback and returning an array.
    expect(moneyForEvensImperative(source)).toEqual([]);
  });
});
