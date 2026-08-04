import { Signal } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { BehaviorSubject, Observable, Subject, of } from "rxjs";
import {
  countingSignal,
  makeSignal,
  toObservableFrom,
  toSignalMaybe,
  toSignalRequireSync,
  toSignalWithInitial,
} from "./clock-bridge";

/**
 * Everything here needs an injection context, so every call goes through this.
 *
 * Note the flushEffects() calls below: toObservable is built on an effect, so its emissions are
 * scheduled rather than synchronous and need change detection to be flushed.
 */
const inContext = <T>(build: () => T): T => TestBed.runInInjectionContext(build);

describe("toSignal", () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it("reads the initial value before the source emits", () => {
    const source = new Subject<number>();
    const value = inContext(() => toSignalWithInitial(source, 0));

    expect(value()).toBe(0);
  });

  it("follows the source", () => {
    const source = new Subject<number>();
    const value = inContext(() => toSignalWithInitial(source, 0));

    source.next(7);
    expect(value()).toBe(7);

    source.next(8);
    expect(value()).toBe(8);
  });

  it("keeps the last value after the source completes", () => {
    const source = new Subject<number>();
    const value = inContext(() => toSignalWithInitial(source, 0));

    source.next(7);
    source.complete();

    // A signal has no notion of completion — it just keeps its current value.
    expect(value()).toBe(7);
  });

  it("takes a synchronous emission immediately", () => {
    const value = inContext(() => toSignalWithInitial(of(42), 0));

    expect(value()).toBe(42);
  });

  it("reads undefined with no initial value", () => {
    const source = new Subject<number>();
    const value = inContext(() => toSignalMaybe(source));

    // Honest, and a nuisance for every consumer downstream — which is the argument for
    // supplying an initial value.
    expect(value()).toBeUndefined();

    source.next(1);
    expect(value()).toBe(1);
  });

  it("needs no initial value for a synchronous source", () => {
    const value = inContext(() => toSignalRequireSync(new BehaviorSubject(5)));

    // requireSync earns the non-optional type by promising the source emits on subscribe.
    expect(value()).toBe(5);
  });

  it("follows a synchronous source afterwards", () => {
    const source = new BehaviorSubject(5);
    const value = inContext(() => toSignalRequireSync(source));

    source.next(6);

    expect(value()).toBe(6);
  });

  it("fails loudly when the promise of a synchronous emission is broken", () => {
    const source = new Subject<number>();

    // Better than reading undefined from something typed as non-optional. Matched on the
    // error code, since an unimplemented stub throws too.
    expect(() => inContext(() => toSignalRequireSync(source))).toThrow(/NG0601/);
  });

  it("refuses to run outside an injection context", () => {
    // It has a subscription to tear down, so it needs something to tie the lifetime to.
    expect(() => toSignalWithInitial(new Subject<number>(), 0)).toThrow(/NG0203/);
  });
});

describe("toObservable", () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it("emits the signal's current value", () => {
    const source = makeSignal(3);
    const stream: Observable<number> = inContext(() => toObservableFrom(source));

    const seen: number[] = [];
    stream.subscribe((value) => seen.push(value));
    TestBed.flushEffects();

    expect(seen).toEqual([3]);
  });

  it("emits again when the signal changes", () => {
    const source = makeSignal(3);
    const stream = inContext(() => toObservableFrom(source));
    const seen: number[] = [];
    stream.subscribe((value) => seen.push(value));
    TestBed.flushEffects();

    source.set(4);
    TestBed.flushEffects();

    expect(seen).toEqual([3, 4]);
  });

  it("does not emit for a set to the same value", () => {
    const source = makeSignal(3);
    const stream = inContext(() => toObservableFrom(source));
    const seen: number[] = [];
    stream.subscribe((value) => seen.push(value));
    TestBed.flushEffects();

    source.set(3);
    TestBed.flushEffects();

    // Signal equality decides, exactly as it does everywhere else.
    expect(seen).toEqual([3]);
  });

  it("refuses to run outside an injection context", () => {
    expect(() => toObservableFrom(makeSignal(1))).toThrow(/NG0203/);
  });
});

describe("countingSignal", () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it("counts each emission", () => {
    const counter = { count: 0 };
    const source = new Subject<number>();
    const value: Signal<number> = inContext(() => countingSignal(source, counter, 0));

    source.next(1);
    source.next(2);

    expect(counter.count).toBe(2);
    expect(value()).toBe(2);
  });

  it("counts nothing before the source emits", () => {
    const counter = { count: 0 };
    inContext(() => countingSignal(new Subject<number>(), counter, 0));

    expect(counter.count).toBe(0);
  });

  it("subscribes eagerly, unlike a plain observable", () => {
    const counter = { count: 0 };
    const source = new BehaviorSubject(9);

    const value = inContext(() => countingSignal(source, counter, 0));

    // toSignal subscribes at once — nothing has to read the signal for the value to arrive.
    expect(counter.count).toBe(1);
    expect(value()).toBe(9);
  });
});
