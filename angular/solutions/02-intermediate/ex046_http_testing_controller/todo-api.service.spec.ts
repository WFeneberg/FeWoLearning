import { provideHttpClient } from "@angular/common/http";
import {
  HttpTestingController,
  provideHttpClientTesting,
} from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { Todo, TodoApi } from "./todo-api.service";

const WRITE: Todo = { id: 1, title: "write", done: false };
const REVIEW: Todo = { id: 2, title: "review", done: true };

describe("TodoApi", () => {
  let api: TodoApi;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(TodoApi);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // Without this, an unexpected extra request goes completely unnoticed.
    http.verify();
  });

  it("fetches the list", () => {
    let received: readonly Todo[] | undefined;
    api.list().subscribe((todos) => (received = todos));

    http.expectOne("/api/todos").flush([WRITE, REVIEW]);

    expect(received).toEqual([WRITE, REVIEW]);
  });

  it("matches a request by method and url", () => {
    api.list().subscribe();

    const request = http.expectOne({ method: "GET", url: "/api/todos" });
    request.flush([]);

    expect(request.request.method).toBe("GET");
  });

  it("caches the list", () => {
    api.list().subscribe();
    http.expectOne("/api/todos").flush([WRITE]);
    expect(api.cached).toBe(true);

    let received: readonly Todo[] | undefined;
    api.list().subscribe((todos) => (received = todos));

    // The only way to test a cache: assert that nothing went out.
    http.expectNone("/api/todos");
    expect(received).toEqual([WRITE]);
  });

  it("bypasses the cache on refresh", () => {
    api.list().subscribe();
    http.expectOne("/api/todos").flush([WRITE]);

    let received: readonly Todo[] | undefined;
    api.refresh().subscribe((todos) => (received = todos));

    http.expectOne("/api/todos").flush([WRITE, REVIEW]);
    expect(received).toEqual([WRITE, REVIEW]);
  });

  it("replaces the cache from a refresh", () => {
    api.refresh().subscribe();
    http.expectOne("/api/todos").flush([WRITE]);

    let received: readonly Todo[] | undefined;
    api.list().subscribe((todos) => (received = todos));

    http.expectNone("/api/todos");
    expect(received).toEqual([WRITE]);
  });

  it("posts a new todo with a body", () => {
    let created: Todo | undefined;
    api.create("ship").subscribe((todo) => (created = todo));

    const request = http.expectOne({ method: "POST", url: "/api/todos" });
    expect(request.request.body).toEqual({ title: "ship", done: false });
    request.flush({ id: 3, title: "ship", done: false });

    expect(created).toEqual({ id: 3, title: "ship", done: false });
  });

  it("refuses a blank title before making a request", () => {
    expect(() => api.create("   ")).toThrow(RangeError);

    // Nothing queued: the guard runs before the request is built.
    http.expectNone({ method: "POST", url: "/api/todos" });
  });

  it("invalidates the cache after a create", () => {
    api.list().subscribe();
    http.expectOne("/api/todos").flush([WRITE]);
    expect(api.cached).toBe(true);

    api.create("ship").subscribe();
    http.expectOne({ method: "POST", url: "/api/todos" }).flush({ id: 3, title: "ship", done: false });

    // Stale the moment the write succeeded, so the next list must go back to the network.
    expect(api.cached).toBe(false);
    api.list().subscribe();
    http.expectOne("/api/todos").flush([]);
  });

  it("deletes by id", () => {
    let completed = false;
    api.remove(1).subscribe(() => (completed = true));

    const request = http.expectOne({ method: "DELETE", url: "/api/todos/1" });
    request.flush(null);

    expect(completed).toBe(true);
  });

  it("invalidates the cache after a delete", () => {
    api.list().subscribe();
    http.expectOne("/api/todos").flush([WRITE]);

    api.remove(1).subscribe();
    http.expectOne({ method: "DELETE", url: "/api/todos/1" }).flush(null);

    expect(api.cached).toBe(false);
  });

  it("patches the done flag", () => {
    api.setDone(2, false).subscribe();

    const request = http.expectOne({ method: "PATCH", url: "/api/todos/2" });
    expect(request.request.body).toEqual({ done: false });
    request.flush({ ...REVIEW, done: false });

    expect(api.cached).toBe(false);
  });

  it("matches several requests at once", () => {
    api.refresh().subscribe();
    api.refresh().subscribe();

    const requests = http.match("/api/todos");

    expect(requests).toHaveLength(2);
    for (const request of requests) {
      request.flush([]);
    }
  });

  it("selects a request with a predicate", () => {
    api.remove(1).subscribe();
    api.remove(2).subscribe();

    const second = http.expectOne(
      (candidate) => candidate.method === "DELETE" && candidate.url.endsWith("/2"),
    );
    second.flush(null);

    http.expectOne("/api/todos/1").flush(null);
  });

  it("can answer with a failure status", () => {
    let status: number | undefined;
    api.list().subscribe({ error: (error: { status: number }) => (status = error.status) });

    http.expectOne("/api/todos").flush("nope", { status: 500, statusText: "Server Error" });

    expect(status).toBe(500);
    // A failed fetch must not leave a cache behind.
    expect(api.cached).toBe(false);
  });

  it("makes no request until subscribed", () => {
    api.list();
    api.create("ship");

    http.expectNone("/api/todos");
  });
});
