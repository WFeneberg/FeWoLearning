import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, Observable, of, retry, tap, throwError } from "rxjs";

// Exercise 047 — HTTP error handling (reference solution).

export class ReportError extends Error {
  constructor(
    readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = "ReportError";
  }
}

export interface Report {
  readonly id: number;
  readonly title: string;
  readonly total: number;
}

export const EMPTY_REPORT: Report = { id: 0, title: "unavailable", total: 0 };

@Injectable({ providedIn: "root" })
export class ReportApi {
  static readonly base = "/api/reports";

  private readonly http = inject(HttpClient);

  attempts = 0;

  classify(error: HttpErrorResponse): string {
    // Checked first and separately: 0 means no response arrived at all, so it is neither a
    // client nor a server problem and the only useful advice is "check your connection".
    if (error.status === 0) {
      return "offline";
    }
    if (error.status >= 400 && error.status < 500) {
      return "client";
    }
    if (error.status >= 500) {
      return "server";
    }
    return "unknown";
  }

  describe(error: HttpErrorResponse): string {
    return `${this.classify(error)}: ${error.status}`;
  }

  fetchOrDefault(id: number): Observable<Report> {
    return this.http.get<Report>(`${ReportApi.base}/${id}`).pipe(
      // Returning an observable turns the error into a normal value, so the stream completes
      // and the caller never sees a failure. Right for optional data, wrong for anything the
      // user is waiting on — it hides the problem completely.
      catchError(() => of(EMPTY_REPORT)),
    );
  }

  fetchOrThrow(id: number): Observable<Report> {
    return this.http
      .get<Report>(`${ReportApi.base}/${id}`)
      .pipe(catchError((error: HttpErrorResponse) => this.toDomainError(error)));
  }

  fetchWithRetry(id: number): Observable<Report> {
    this.attempts = 0;
    return this.http.get<Report>(`${ReportApi.base}/${id}`).pipe(
      // Counted here, inside the retried part, so every attempt is seen.
      tap({ subscribe: () => (this.attempts += 1) }),
      // Two retries on top of the original: three attempts in total. Only safe because a GET
      // is idempotent — retrying a POST could create two records.
      retry({ count: 2 }),
      catchError((error: HttpErrorResponse) => this.toDomainError(error)),
    );
  }

  private toDomainError(error: HttpErrorResponse): Observable<never> {
    // throwError takes a *factory*, so the error is built at subscribe time rather than once
    // when the pipeline was assembled.
    return throwError(() => new ReportError(error.status, this.describe(error)));
  }
}
