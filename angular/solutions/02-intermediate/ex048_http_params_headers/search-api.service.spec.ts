import { provideHttpClient } from "@angular/common/http";
import {
  HttpTestingController,
  provideHttpClientTesting,
} from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { SearchApi } from "./search-api.service";

describe("SearchApi", () => {
  let api: SearchApi;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(SearchApi);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
  });

  describe("buildParams", () => {
    it("always carries the trimmed term", () => {
      expect(api.buildParams({ term: "  angular  " }).get("q")).toBe("angular");
    });

    it("omits page when it is not given", () => {
      expect(api.buildParams({ term: "x" }).has("page")).toBe(false);
    });

    it("includes page when given", () => {
      expect(api.buildParams({ term: "x", page: 3 }).get("page")).toBe("3");
    });

    it("keeps every tag", () => {
      const params = api.buildParams({ term: "x", tags: ["a", "b", "c"] });

      // append, not set: this is how a list is expressed in a query string.
      expect(params.getAll("tag")).toEqual(["a", "b", "c"]);
    });

    it("handles a single tag", () => {
      expect(api.buildParams({ term: "x", tags: ["only"] }).getAll("tag")).toEqual(["only"]);
    });

    it("handles no tags", () => {
      expect(api.buildParams({ term: "x", tags: [] }).has("tag")).toBe(false);
    });

    it("produces a repeated key in the query string", () => {
      const params = api.buildParams({ term: "x", tags: ["a", "b"] });

      expect(params.toString()).toBe("q=x&tag=a&tag=b");
    });

    it("encodes values without being asked", () => {
      const params = api.buildParams({ term: "a b&c" });

      // Passed raw, encoded once. Pre-encoding here would give %2520 and quietly succeed.
      expect(params.get("q")).toBe("a b&c");
      expect(params.toString()).toContain("q=a%20b%26c");
    });
  });

  describe("the set/append distinction", () => {
    it("loses all but the last tag when set is used", () => {
      const params = api.buildParamsWithSet({ term: "x", tags: ["a", "b", "c"] });

      // Looks like the back end ignoring the filter; it is actually this.
      expect(params.getAll("tag")).toEqual(["c"]);
    });

    it("keeps all of them with append", () => {
      expect(api.buildParams({ term: "x", tags: ["a", "b", "c"] }).getAll("tag")).toEqual([
        "a",
        "b",
        "c",
      ]);
    });

    it("builds nothing at all when the result is discarded", () => {
      const params = api.buildParamsIgnoringResult({ term: "x", tags: ["a"] });

      // HttpParams is immutable: an unassigned set() is the same mistake as an unused filter().
      expect(params.keys()).toEqual([]);
    });
  });

  describe("buildHeaders", () => {
    it("always accepts JSON", () => {
      expect(api.buildHeaders().get("Accept")).toBe("application/json");
    });

    it("is keyed case-insensitively", () => {
      const headers = api.buildHeaders();

      expect(headers.has("accept")).toBe(true);
      expect(headers.has("ACCEPT")).toBe(true);
    });

    it("omits authorization without a token", () => {
      expect(api.buildHeaders().has("Authorization")).toBe(false);
    });

    it("adds a bearer token", () => {
      expect(api.buildHeaders({ token: "abc123" }).get("Authorization")).toBe("Bearer abc123");
    });

    it("appends every trace id", () => {
      const headers = api.buildHeaders({ traceIds: ["one", "two"] });

      expect(headers.getAll("X-Trace-Id")).toEqual(["one", "two"]);
    });

    it("omits trace ids when there are none", () => {
      expect(api.buildHeaders({ traceIds: [] }).has("X-Trace-Id")).toBe(false);
    });

    it("combines a token and trace ids", () => {
      const headers = api.buildHeaders({ token: "abc", traceIds: ["one"] });

      expect(headers.get("Accept")).toBe("application/json");
      expect(headers.get("Authorization")).toBe("Bearer abc");
      expect(headers.getAll("X-Trace-Id")).toEqual(["one"]);
    });
  });

  describe("search", () => {
    it("sends the params and headers it built", () => {
      let received: readonly string[] | undefined;
      api.search({ term: "angular", tags: ["forms", "http"], page: 2 }, "abc").subscribe(
        (results) => (received = results),
      );

      const request = http.expectOne((candidate) => candidate.url === "/api/search");
      expect(request.request.params.get("q")).toBe("angular");
      expect(request.request.params.getAll("tag")).toEqual(["forms", "http"]);
      expect(request.request.params.get("page")).toBe("2");
      expect(request.request.headers.get("Authorization")).toBe("Bearer abc");
      expect(request.request.headers.get("Accept")).toBe("application/json");
      request.flush(["a", "b"]);

      expect(received).toEqual(["a", "b"]);
    });

    it("sends no authorization when there is no token", () => {
      api.search({ term: "x" }).subscribe();

      const request = http.expectOne((candidate) => candidate.url === "/api/search");
      expect(request.request.headers.has("Authorization")).toBe(false);
      request.flush([]);
    });
  });
});
