import { TestBed } from "@angular/core/testing";
import { AppErrorHandler, ERROR_SINK, ErrorReport, ErrorSink } from "./app-error-handler";

class FakeSink implements ErrorSink {
  readonly reports: ErrorReport[] = [];
  report(entry: ErrorReport): void {
    this.reports.push(entry);
  }
}

describe("AppErrorHandler", () => {
  let sink: FakeSink;
  let handler: AppErrorHandler;

  beforeEach(() => {
    sink = new FakeSink();
    TestBed.configureTestingModule({
      providers: [AppErrorHandler, { provide: ERROR_SINK, useValue: sink }],
    });
    handler = TestBed.inject(AppErrorHandler);
  });

  it("reports a real Error's message and stack", () => {
    const error = new Error("boom");

    handler.handleError(error);

    expect(sink.reports).toHaveLength(1);
    expect(sink.reports[0].message).toBe("boom");
    expect(sink.reports[0].stack).toBe(error.stack);
  });

  it("includes a numeric timestamp", () => {
    const before = Date.now();
    handler.handleError(new Error("x"));
    const after = Date.now();

    expect(sink.reports[0].timestamp).toBeGreaterThanOrEqual(before);
    expect(sink.reports[0].timestamp).toBeLessThanOrEqual(after);
  });

  it("reports a non-Error thrown value by stringifying it, with no stack", () => {
    handler.handleError("just a string");

    expect(sink.reports).toHaveLength(1);
    expect(sink.reports[0].message).toBe("just a string");
    expect(sink.reports[0].stack).toBeUndefined();
  });

  it("stringifies a thrown plain object rather than throwing while formatting it", () => {
    expect(() => handler.handleError({ code: 500 })).not.toThrow();

    expect(sink.reports).toHaveLength(1);
    expect(sink.reports[0].message).toBe(String({ code: 500 }));
  });

  it("de-duplicates two consecutive identical error messages, reporting only the first", () => {
    handler.handleError(new Error("repeated"));
    handler.handleError(new Error("repeated"));

    expect(sink.reports).toHaveLength(1);
  });

  it("does not de-duplicate across a different error in between", () => {
    handler.handleError(new Error("repeated"));
    handler.handleError(new Error("different"));
    handler.handleError(new Error("repeated"));

    expect(sink.reports.map((report) => report.message)).toEqual([
      "repeated",
      "different",
      "repeated",
    ]);
  });

  it("never throws out of handleError, even for a de-duplicated call", () => {
    expect(() => {
      handler.handleError(new Error("same"));
      handler.handleError(new Error("same"));
    }).not.toThrow();
  });
});
