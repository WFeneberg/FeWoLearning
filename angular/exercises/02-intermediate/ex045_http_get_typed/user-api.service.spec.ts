import { provideHttpClient } from "@angular/common/http";
import {
  HttpTestingController,
  provideHttpClientTesting,
} from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { User, UserApi } from "./user-api.service";

const ADA: User = { id: 1, name: "Ada", email: "ada@example.com" };
const GRACE: User = { id: 2, name: "Grace", email: "grace@example.com" };

describe("UserApi", () => {
  let api: UserApi;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      // provideHttpClient first, then the testing backend that replaces its transport.
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(UserApi);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // Fails the test if any request went out that no expectation accounted for.
    http.verify();
  });

  it("gets one user by id", () => {
    let received: User | undefined;
    api.getUser(1).subscribe((user) => (received = user));

    const request = http.expectOne("/api/users/1");
    expect(request.request.method).toBe("GET");
    request.flush(ADA);

    expect(received).toEqual(ADA);
  });

  it("sends no parameters when there is no query", () => {
    api.listUsers().subscribe();

    const request = http.expectOne("/api/users");
    expect(request.request.params.keys()).toEqual([]);
    request.flush([]);
  });

  it("sends no parameters for an empty query object", () => {
    api.listUsers({}).subscribe();

    http.expectOne("/api/users").flush([]);
  });

  it("includes the page when given", () => {
    api.listUsers({ page: 2 }).subscribe();

    const request = http.expectOne((candidate) => candidate.url === "/api/users");
    expect(request.request.params.get("page")).toBe("2");
    request.flush([]);
  });

  it("includes a search term", () => {
    api.listUsers({ search: "ada" }).subscribe();

    const request = http.expectOne((candidate) => candidate.url === "/api/users");
    expect(request.request.params.get("search")).toBe("ada");
    request.flush([]);
  });

  it("trims a search term", () => {
    api.listUsers({ search: "  ada  " }).subscribe();

    const request = http.expectOne((candidate) => candidate.url === "/api/users");
    expect(request.request.params.get("search")).toBe("ada");
    request.flush([]);
  });

  it("leaves a blank search out entirely", () => {
    api.listUsers({ search: "   " }).subscribe();

    const request = http.expectOne("/api/users");
    // "?search=" is a different request from no search — not the same thing at all.
    expect(request.request.params.has("search")).toBe(false);
    request.flush([]);
  });

  it("includes activeOnly only when true", () => {
    api.listUsers({ activeOnly: true }).subscribe();
    const withFlag = http.expectOne((candidate) => candidate.url === "/api/users");
    expect(withFlag.request.params.get("activeOnly")).toBe("true");
    withFlag.flush([]);

    api.listUsers({ activeOnly: false }).subscribe();
    const withoutFlag = http.expectOne("/api/users");
    expect(withoutFlag.request.params.has("activeOnly")).toBe(false);
    withoutFlag.flush([]);
  });

  it("combines several parameters", () => {
    api.listUsers({ page: 3, search: "grace", activeOnly: true }).subscribe();

    const request = http.expectOne((candidate) => candidate.url === "/api/users");
    expect(request.request.params.keys().sort()).toEqual(["activeOnly", "page", "search"]);
    request.flush([]);
  });

  it("returns the list it was sent", () => {
    let received: readonly User[] | undefined;
    api.listUsers().subscribe((users) => (received = users));

    http.expectOne("/api/users").flush([ADA, GRACE]);

    expect(received).toEqual([ADA, GRACE]);
  });

  it("maps a user down to a name", () => {
    let received: string | undefined;
    api.getUserName(2).subscribe((name) => (received = name));

    http.expectOne("/api/users/2").flush(GRACE);

    expect(received).toBe("Grace");
  });

  it("maps a list down to sorted emails", () => {
    let received: readonly string[] | undefined;
    api.listEmails().subscribe((emails) => (received = emails));

    http.expectOne("/api/users").flush([GRACE, ADA]);

    // The caller never sees the wire shape.
    expect(received).toEqual(["ada@example.com", "grace@example.com"]);
  });

  it("passes the query through to listEmails", () => {
    api.listEmails({ page: 1 }).subscribe();

    const request = http.expectOne((candidate) => candidate.url === "/api/users");
    expect(request.request.params.get("page")).toBe("1");
    request.flush([]);
  });

  it("makes no request until something subscribes", () => {
    api.getUser(1);

    // HttpClient observables are cold — no subscribe, no request. The afterEach verify()
    // would fail if one had gone out.
    http.expectNone("/api/users/1");
  });
});
