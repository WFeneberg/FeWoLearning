import { HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

// Exercise 048 — HttpParams and HttpHeaders (intermediate).
// Goal:   build query strings and headers correctly, including the repeated-key case.
// Drills: HttpParams immutability, set vs append, appendAll, encoding, HttpHeaders, and the
//         difference between a header you replace and one you add to.
// Passes: when `npx jest exercises/02-intermediate/ex048_http_params_headers` is green.
//
// Both classes are immutable, and both have the same pair of methods with genuinely different
// meanings. `set` replaces every existing value for a key; `append` adds another one. That is
// not a stylistic choice: `?tag=a&tag=b` is how a list is expressed in a query string, and
// reaching for `set` in a loop leaves you with only the last value — a bug that looks like the
// back end ignoring your filter.
//
// Immutability is the other half. `params.set(...)` returns a new HttpParams and leaves the
// receiver untouched, so a loop that forgets to reassign builds nothing at all. It is the same
// shape of mistake as `array.filter(...)` without using the result.
//
// Encoding is handled for you — a space becomes %20, an ampersand %26 — so values must be passed
// raw. Encoding them yourself gets you double-encoding (%2520), which is worse than not encoding
// at all because it silently succeeds.
//
// Headers are keyed case-insensitively, which trips people up when they check `has("Accept")`
// after setting `accept`. And a header set to an empty string is still a header that is present.

export interface SearchQuery {
  readonly term: string;
  readonly tags?: readonly string[];
  readonly page?: number;
}

@Injectable({ providedIn: "root" })
export class SearchApi {
  static readonly base = "/api/search";

  /** TODO: inject HttpClient. */

  /**
   * Build the query parameters for a search.
   *
   * `q` is the trimmed term, always present. Each tag is *appended* under the key `tag`, so
   * several tags survive. `page` appears only when given.
   */
  buildParams(query: SearchQuery): HttpParams {
    throw new Error("TODO: implement buildParams");
  }

  /**
   * Build the request headers.
   *
   * Always `Accept: application/json`. When `token` is given, also
   * `Authorization: Bearer <token>`. When `traceIds` is given, one `X-Trace-Id` per id,
   * appended rather than replaced.
   */
  buildHeaders(options?: {
    token?: string;
    traceIds?: readonly string[];
  }): HttpHeaders {
    throw new Error("TODO: implement buildHeaders");
  }

  /** GET /api/search with the params and headers built above. */
  search(query: SearchQuery, token?: string): Observable<readonly string[]> {
    throw new Error("TODO: implement search");
  }

  /**
   * The wrong way, kept so the spec can show what goes missing.
   *
   * Add every tag with `set` instead of `append`, and see how many survive.
   */
  buildParamsWithSet(query: SearchQuery): HttpParams {
    throw new Error("TODO: implement buildParamsWithSet");
  }

  /**
   * The other wrong way: call set without using the result.
   *
   * Returns whatever the unassigned calls left behind, which is nothing.
   */
  buildParamsIgnoringResult(query: SearchQuery): HttpParams {
    throw new Error("TODO: implement buildParamsIgnoringResult");
  }
}
