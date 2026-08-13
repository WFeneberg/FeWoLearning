import { HttpClient, provideHttpClient, withInterceptors } from "@angular/common/http";
import {
  HttpTestingController,
  provideHttpClientTesting,
} from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { TokenStore, authInterceptor } from "./auth-interceptor";

describe("authInterceptor", () => {
  let http: HttpClient;
  let controller: HttpTestingController;
  let tokenStore: TokenStore;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
      ],
    });
    http = TestBed.inject(HttpClient);
    controller = TestBed.inject(HttpTestingController);
    tokenStore = TestBed.inject(TokenStore);
  });

  afterEach(() => {
    controller.verify();
  });

  it("sends no Authorization header while logged out", () => {
    http.get("/api/profile").subscribe();

    const req = controller.expectOne("/api/profile");
    expect(req.request.headers.has("Authorization")).toBe(false);
    req.flush({});
  });

  it("attaches a Bearer header once a token is set", () => {
    tokenStore.set("abc123");

    http.get("/api/profile").subscribe();

    const req = controller.expectOne("/api/profile");
    expect(req.request.headers.get("Authorization")).toBe("Bearer abc123");
    req.flush({});
  });

  it("preserves the request's other headers when attaching the auth header", () => {
    tokenStore.set("abc123");

    http.get("/api/profile", { headers: { "X-Trace-Id": "trace-1" } }).subscribe();

    const req = controller.expectOne("/api/profile");
    expect(req.request.headers.get("X-Trace-Id")).toBe("trace-1");
    expect(req.request.headers.get("Authorization")).toBe("Bearer abc123");
    req.flush({});
  });

  it("stops attaching the header again once logged out", () => {
    tokenStore.set("abc123");
    http.get("/api/profile").subscribe();
    controller.expectOne("/api/profile").flush({});

    tokenStore.set(null);
    http.get("/api/profile").subscribe();

    const req = controller.expectOne("/api/profile");
    expect(req.request.headers.has("Authorization")).toBe(false);
    req.flush({});
  });
});
