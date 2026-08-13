import { HttpClient, HttpErrorResponse, provideHttpClient, withInterceptors } from "@angular/common/http";
import {
  HttpTestingController,
  provideHttpClientTesting,
} from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { firstValueFrom } from "rxjs";
import { RETRY_CONFIG, retryInterceptor } from "./retry-interceptor";

/** Let a real (tiny) backoff timer elapse before the interceptor issues its next attempt. */
const settle = () => new Promise<void>((resolve) => setTimeout(resolve, 20));

describe("retryInterceptor", () => {
  let http: HttpClient;
  let controller: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([retryInterceptor])),
        provideHttpClientTesting(),
        { provide: RETRY_CONFIG, useValue: { maxAttempts: 3, baseDelayMs: 1 } },
      ],
    });
    http = TestBed.inject(HttpClient);
    controller = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    controller.verify();
  });

  it("passes a successful response through untouched", async () => {
    const result = firstValueFrom(http.get("/api/data"));

    controller.expectOne("/api/data").flush({ ok: true });

    await expect(result).resolves.toEqual({ ok: true });
  });

  it("retries a 503 and resolves once the backend recovers", async () => {
    const result = firstValueFrom(http.get("/api/data"));

    controller.expectOne("/api/data").flush("down", { status: 503, statusText: "Service Unavailable" });
    await settle();

    controller.expectOne("/api/data").flush({ ok: true });

    await expect(result).resolves.toEqual({ ok: true });
  });

  it("gives up after maxAttempts and surfaces the final error", async () => {
    const result = firstValueFrom(http.get("/api/data")).catch((error: HttpErrorResponse) => error);

    controller.expectOne("/api/data").flush("down", { status: 500, statusText: "Server Error" });
    await settle();
    controller.expectOne("/api/data").flush("down", { status: 500, statusText: "Server Error" });
    await settle();
    controller.expectOne("/api/data").flush("down", { status: 500, statusText: "Server Error" });

    const error = await result;
    expect(error).toBeInstanceOf(HttpErrorResponse);
    expect((error as HttpErrorResponse).status).toBe(500);
  });

  it("does not retry a 404 — it is not a transient failure", async () => {
    const result = firstValueFrom(http.get("/api/data")).catch((error: HttpErrorResponse) => error);

    controller.expectOne("/api/data").flush("missing", { status: 404, statusText: "Not Found" });
    await settle();

    const error = await result;
    expect(error).toBeInstanceOf(HttpErrorResponse);
    expect((error as HttpErrorResponse).status).toBe(404);

    controller.verify(); // exactly one request went out — no retry happened
  });
});
