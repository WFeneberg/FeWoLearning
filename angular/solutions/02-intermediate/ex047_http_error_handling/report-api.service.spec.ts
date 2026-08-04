import { HttpErrorResponse, provideHttpClient } from "@angular/common/http";
import {
  HttpTestingController,
  provideHttpClientTesting,
} from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { EMPTY_REPORT, Report, ReportApi, ReportError } from "./report-api.service";

const SALES: Report = { id: 7, title: "sales", total: 42 };

describe("ReportApi", () => {
  let api: ReportApi;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(ReportApi);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
  });

  const errorWith = (status: number): HttpErrorResponse =>
    new HttpErrorResponse({ status, statusText: "x", url: "/api/reports/7" });

  describe("classify", () => {
    it("calls status 0 offline, not a server error", () => {
      // The one that catches people out: no response at all, so nothing to blame the server for.
      expect(api.classify(errorWith(0))).toBe("offline");
    });

    it("classifies client failures", () => {
      expect(api.classify(errorWith(400))).toBe("client");
      expect(api.classify(errorWith(404))).toBe("client");
      expect(api.classify(errorWith(499))).toBe("client");
    });

    it("classifies server failures", () => {
      expect(api.classify(errorWith(500))).toBe("server");
      expect(api.classify(errorWith(503))).toBe("server");
    });

    it("classifies anything else as unknown", () => {
      expect(api.classify(errorWith(302))).toBe("unknown");
    });

    it("builds a readable message", () => {
      expect(api.describe(errorWith(500))).toBe("server: 500");
      expect(api.describe(errorWith(0))).toBe("offline: 0");
      expect(api.describe(errorWith(404))).toBe("client: 404");
    });
  });

  describe("fetchOrDefault", () => {
    it("passes a successful response through", () => {
      let received: Report | undefined;
      api.fetchOrDefault(7).subscribe((report) => (received = report));

      http.expectOne("/api/reports/7").flush(SALES);

      expect(received).toEqual(SALES);
    });

    it("falls back on a server failure", () => {
      let received: Report | undefined;
      let errored = false;
      api.fetchOrDefault(7).subscribe({
        next: (report) => (received = report),
        error: () => (errored = true),
      });

      http.expectOne("/api/reports/7").flush("nope", { status: 500, statusText: "Server Error" });

      expect(received).toEqual(EMPTY_REPORT);
      // A caller of this never sees an error at all.
      expect(errored).toBe(false);
    });

    it("falls back when offline", () => {
      let received: Report | undefined;
      api.fetchOrDefault(7).subscribe((report) => (received = report));

      http.expectOne("/api/reports/7").error(new ProgressEvent("error"), { status: 0 });

      expect(received).toEqual(EMPTY_REPORT);
    });

    it("completes normally after falling back", () => {
      let completed = false;
      api.fetchOrDefault(7).subscribe({ complete: () => (completed = true) });

      http.expectOne("/api/reports/7").flush("", { status: 404, statusText: "Not Found" });

      expect(completed).toBe(true);
    });
  });

  describe("fetchOrThrow", () => {
    it("passes a successful response through", () => {
      let received: Report | undefined;
      api.fetchOrThrow(7).subscribe((report) => (received = report));

      http.expectOne("/api/reports/7").flush(SALES);

      expect(received).toEqual(SALES);
    });

    it("translates a failure into a domain error", () => {
      let caught: unknown;
      api.fetchOrThrow(7).subscribe({ error: (error: unknown) => (caught = error) });

      http.expectOne("/api/reports/7").flush("", { status: 500, statusText: "Server Error" });

      // Nothing above this layer has to know HttpErrorResponse exists.
      expect(caught).toBeInstanceOf(ReportError);
      expect((caught as ReportError).status).toBe(500);
      expect((caught as ReportError).message).toBe("server: 500");
    });

    it("translates an offline failure too", () => {
      let caught: ReportError | undefined;
      api.fetchOrThrow(7).subscribe({ error: (error: ReportError) => (caught = error) });

      http.expectOne("/api/reports/7").error(new ProgressEvent("error"), { status: 0 });

      expect(caught?.message).toBe("offline: 0");
    });

    it("does not leak the HttpErrorResponse", () => {
      let caught: unknown;
      api.fetchOrThrow(7).subscribe({ error: (error: unknown) => (caught = error) });

      http.expectOne("/api/reports/7").flush("", { status: 404, statusText: "Not Found" });

      expect(caught).not.toBeInstanceOf(HttpErrorResponse);
    });
  });

  describe("fetchWithRetry", () => {
    it("succeeds on the first attempt without retrying", () => {
      let received: Report | undefined;
      api.fetchWithRetry(7).subscribe((report) => (received = report));

      http.expectOne("/api/reports/7").flush(SALES);

      expect(received).toEqual(SALES);
      expect(api.attempts).toBe(1);
    });

    it("recovers on the second attempt", () => {
      let received: Report | undefined;
      api.fetchWithRetry(7).subscribe((report) => (received = report));

      // Re-subscribing an HttpClient observable sends the request again, so each attempt is a
      // fresh request the test has to answer.
      http.expectOne("/api/reports/7").flush("", { status: 500, statusText: "Server Error" });
      http.expectOne("/api/reports/7").flush(SALES);

      expect(received).toEqual(SALES);
      expect(api.attempts).toBe(2);
    });

    it("recovers on the third attempt", () => {
      let received: Report | undefined;
      api.fetchWithRetry(7).subscribe((report) => (received = report));

      for (const _ of [1, 2]) {
        http.expectOne("/api/reports/7").flush("", { status: 503, statusText: "Unavailable" });
      }
      http.expectOne("/api/reports/7").flush(SALES);

      expect(received).toEqual(SALES);
      expect(api.attempts).toBe(3);
    });

    it("gives up after three attempts", () => {
      let caught: ReportError | undefined;
      api.fetchWithRetry(7).subscribe({ error: (error: ReportError) => (caught = error) });

      for (const _ of [1, 2, 3]) {
        http.expectOne("/api/reports/7").flush("", { status: 500, statusText: "Server Error" });
      }

      expect(api.attempts).toBe(3);
      expect(caught).toBeInstanceOf(ReportError);
      expect(caught?.message).toBe("server: 500");
    });

    it("makes no fourth attempt", () => {
      api.fetchWithRetry(7).subscribe({ error: () => undefined });

      for (const _ of [1, 2, 3]) {
        http.expectOne("/api/reports/7").flush("", { status: 500, statusText: "Server Error" });
      }

      // verify() in afterEach would also catch a stray fourth request.
      http.expectNone("/api/reports/7");
    });
  });
});
