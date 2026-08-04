import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";

// Exercise 048 — HttpParams and HttpHeaders (reference solution).

export interface SearchQuery {
  readonly term: string;
  readonly tags?: readonly string[];
  readonly page?: number;
}

@Injectable({ providedIn: "root" })
export class SearchApi {
  static readonly base = "/api/search";

  private readonly http = inject(HttpClient);

  buildParams(query: SearchQuery): HttpParams {
    // Reassigned every time: HttpParams is immutable, so the return value is the only result.
    let params = new HttpParams().set("q", query.term.trim());
    for (const tag of query.tags ?? []) {
      // append, not set — `?tag=a&tag=b` is how a list is expressed.
      params = params.append("tag", tag);
    }
    if (query.page !== undefined) {
      params = params.set("page", String(query.page));
    }
    return params;
  }

  buildHeaders(options?: { token?: string; traceIds?: readonly string[] }): HttpHeaders {
    let headers = new HttpHeaders().set("Accept", "application/json");
    if (options?.token !== undefined) {
      headers = headers.set("Authorization", `Bearer ${options.token}`);
    }
    for (const id of options?.traceIds ?? []) {
      headers = headers.append("X-Trace-Id", id);
    }
    return headers;
  }

  search(query: SearchQuery, token?: string): Observable<readonly string[]> {
    return this.http.get<string[]>(SearchApi.base, {
      params: this.buildParams(query),
      headers: this.buildHeaders({ token }),
    });
  }

  buildParamsWithSet(query: SearchQuery): HttpParams {
    let params = new HttpParams().set("q", query.term.trim());
    for (const tag of query.tags ?? []) {
      // Deliberately wrong: set replaces, so only the last tag survives. This reads like the
      // back end ignoring the filter.
      params = params.set("tag", tag);
    }
    return params;
  }

  buildParamsIgnoringResult(query: SearchQuery): HttpParams {
    const params = new HttpParams();
    // Deliberately wrong: the results are thrown away, so `params` is still empty. The same
    // mistake as calling array.filter() and not using what it returns.
    params.set("q", query.term.trim());
    for (const tag of query.tags ?? []) {
      params.append("tag", tag);
    }
    return params;
  }
}
