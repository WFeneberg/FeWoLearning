import { TestBed } from "@angular/core/testing";
import { API_CONFIG, ApiClient, ApiConfig, PAGE_SIZE } from "./api-config";

const CONFIG: ApiConfig = { baseUrl: "https://api.example.com", timeoutMs: 5000 };

describe("InjectionToken configuration", () => {
  const configure = (providers: unknown[] = []): void => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [{ provide: API_CONFIG, useValue: CONFIG }, ...providers] as never,
    });
  };

  beforeEach(() => configure());

  it("injects a value that is not a class", () => {
    expect(TestBed.inject(API_CONFIG)).toEqual(CONFIG);
  });

  it("hands the config to a consumer", () => {
    expect(TestBed.inject(ApiClient).config).toEqual(CONFIG);
  });

  it("falls back to the token's own factory", () => {
    // No provider for PAGE_SIZE anywhere, yet it resolves.
    expect(TestBed.inject(PAGE_SIZE)).toBe(25);
    expect(TestBed.inject(ApiClient).pageSize).toBe(25);
  });

  it("lets an application override the default", () => {
    configure([{ provide: PAGE_SIZE, useValue: 100 }]);

    expect(TestBed.inject(PAGE_SIZE)).toBe(100);
    expect(TestBed.inject(ApiClient).pageSize).toBe(100);
  });

  it("throws a readable error for a token with no provider and no factory", () => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({});

    // The description passed to the constructor is what makes this error readable —
    // matched on the name, since "it threw something" would be true of anything.
    expect(() => TestBed.inject(API_CONFIG)).toThrow(/API_CONFIG/);
  });

  it("joins a path onto the base URL", () => {
    expect(TestBed.inject(ApiClient).url("users")).toBe("https://api.example.com/users");
  });

  it("does not double up slashes", () => {
    configure();
    const client = TestBed.inject(ApiClient);

    expect(client.url("/users")).toBe("https://api.example.com/users");
  });

  it("handles a base URL with a trailing slash", () => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [
        { provide: API_CONFIG, useValue: { baseUrl: "https://x/", timeoutMs: 1 } },
      ] as never,
    });

    const client = TestBed.inject(ApiClient);

    expect(client.url("/users")).toBe("https://x/users");
    expect(client.url("users")).toBe("https://x/users");
  });

  it("describes everything it was given", () => {
    expect(TestBed.inject(ApiClient).describe("users")).toBe(
      "https://api.example.com/users (timeout 5000ms, page 25)",
    );
  });

  it("describes an overridden page size", () => {
    configure([{ provide: PAGE_SIZE, useValue: 10 }]);

    expect(TestBed.inject(ApiClient).describe("users")).toBe(
      "https://api.example.com/users (timeout 5000ms, page 10)",
    );
  });

  it("uses distinct tokens for distinct settings", () => {
    // Two tokens of the same underlying shape must not collide the way strings would.
    expect(API_CONFIG).not.toBe(PAGE_SIZE);
  });
});
