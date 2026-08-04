import { TestBed } from "@angular/core/testing";
import {
  AuditService,
  ClassicAuditService,
  Logger,
  Telemetry,
  createTicker,
} from "./audit.service";

describe("inject() vs constructor injection", () => {
  let logger: Logger;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    logger = TestBed.inject(Logger);
  });

  it("injects into a field initialiser", () => {
    const audit = TestBed.inject(AuditService);

    expect(audit.logger).toBe(logger);
  });

  it("records through the injected logger", () => {
    TestBed.inject(AuditService).record("login");

    expect(logger.entries).toEqual(["audit: login"]);
  });

  it("records several actions in order", () => {
    const audit = TestBed.inject(AuditService);
    audit.record("login");
    audit.record("logout");

    expect(logger.entries).toEqual(["audit: login", "audit: logout"]);
  });

  it("tolerates a provider that does not exist", () => {
    const audit = TestBed.inject(AuditService);

    // Nothing provides Telemetry, and optional injection turns that into null.
    expect(audit.telemetry).toBeNull();
  });

  it("skips the optional dependency when recording", () => {
    const audit = TestBed.inject(AuditService);

    expect(() => audit.record("login")).not.toThrow();
    expect(logger.entries).toEqual(["audit: login"]);
  });

  it("uses the optional dependency when it is provided", () => {
    // A fresh TestBed, this time with Telemetry available.
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({ providers: [Telemetry] });
    const audit = TestBed.inject(AuditService);

    audit.record("login");

    expect(audit.telemetry).not.toBeNull();
    expect(audit.telemetry?.pings).toEqual(["login"]);
  });

  it("reaches the same singleton through constructor injection", () => {
    const classic = TestBed.inject(ClassicAuditService);

    expect(classic.logger).toBe(logger);
  });

  it("behaves identically whichever style is used", () => {
    TestBed.inject(AuditService).record("one");
    TestBed.inject(ClassicAuditService).record("two");

    expect(logger.entries).toEqual(["audit: one", "audit: two"]);
  });

  it("lets a plain function inject, given a context", () => {
    const tick = TestBed.runInInjectionContext(() => createTicker());

    tick();
    tick();

    expect(logger.entries).toEqual(["tick 1", "tick 2"]);
  });

  it("refuses to inject outside an injection context", () => {
    // NG0203 — inject() only works while Angular is constructing something.
    expect(() => createTicker()).toThrow(/NG0203/);
  });

  it("keeps separate tickers counting separately", () => {
    const first = TestBed.runInInjectionContext(() => createTicker());
    const second = TestBed.runInInjectionContext(() => createTicker());

    first();
    second();

    // Each closure has its own counter, but they share the one Logger.
    expect(logger.entries).toEqual(["tick 1", "tick 1"]);
  });
});
