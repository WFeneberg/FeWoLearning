import { TestBed } from "@angular/core/testing";
import {
  APP_VERSION,
  ConsoleLogger,
  DEBUG_MODE,
  DIAGNOSTICS_PROVIDERS,
  Diagnostics,
  LOG_LEVEL,
} from "./diagnostics";

describe("Diagnostics — useFactory, useExisting, useValue", () => {
  const configure = (extra: object[] = []): void => {
    TestBed.configureTestingModule({
      providers: [...DIAGNOSTICS_PROVIDERS, { provide: APP_VERSION, useValue: "1.4.0" }, ...extra],
    });
  };

  it("reads a useValue token verbatim", () => {
    configure();

    expect(TestBed.inject(Diagnostics).report("ready")).toBe("v1.4.0 [info] ready");
  });

  it("the factory reacts to another injected token", () => {
    configure([{ provide: DEBUG_MODE, useValue: true }]);

    expect(TestBed.inject(Diagnostics).report("ready")).toBe("v1.4.0 [debug] ready");
  });

  it("the factory defaults to DEBUG_MODE's own root default when nothing overrides it", () => {
    configure();

    expect(TestBed.inject(LOG_LEVEL)).toBe("info");
  });

  it("useExisting aliases rather than instantiating a second logger", () => {
    configure();

    expect(TestBed.inject(Diagnostics).sameInstance()).toBe(true);
  });

  it("the aliased logger really is the one ConsoleLogger instance", () => {
    configure();

    const diagnostics = TestBed.inject(Diagnostics);
    const logger = TestBed.inject(ConsoleLogger);
    diagnostics.report("hello");

    // If useExisting had instead built a second ConsoleLogger, this would still be empty.
    expect(logger.lines).toEqual(["v1.4.0 [info] hello"]);
  });

  it("report sends exactly what it returned to the logger, once", () => {
    configure([{ provide: DEBUG_MODE, useValue: true }]);

    const diagnostics = TestBed.inject(Diagnostics);
    const logger = TestBed.inject(ConsoleLogger);
    const result = diagnostics.report("checking in");

    expect(logger.lines).toEqual([result]);
  });

  it("a different APP_VERSION provider changes nothing else", () => {
    TestBed.configureTestingModule({
      providers: [...DIAGNOSTICS_PROVIDERS, { provide: APP_VERSION, useValue: "9.9.9" }],
    });

    expect(TestBed.inject(Diagnostics).report("ready")).toBe("v9.9.9 [info] ready");
  });
});
