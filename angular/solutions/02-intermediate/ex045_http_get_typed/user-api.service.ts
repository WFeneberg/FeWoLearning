import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";

// Exercise 045 — typed HttpClient.get (reference solution).

export interface User {
  readonly id: number;
  readonly name: string;
  readonly email: string;
}

export interface UserQuery {
  readonly page?: number;
  readonly search?: string;
  readonly activeOnly?: boolean;
}

@Injectable({ providedIn: "root" })
export class UserApi {
  static readonly base = "/api/users";

  private readonly http = inject(HttpClient);

  getUser(id: number): Observable<User> {
    // The generic is an assertion about what the server sends, not a check on it.
    return this.http.get<User>(`${UserApi.base}/${id}`);
  }

  listUsers(query?: UserQuery): Observable<readonly User[]> {
    return this.http.get<User[]>(UserApi.base, { params: this.buildParams(query) });
  }

  getUserName(id: number): Observable<string> {
    return this.getUser(id).pipe(map((user) => user.name));
  }

  listEmails(query?: UserQuery): Observable<readonly string[]> {
    return this.listUsers(query).pipe(
      // Mapping here means callers never see the wire shape at all.
      map((users) => users.map((user) => user.email).sort()),
    );
  }

  private buildParams(query?: UserQuery): HttpParams {
    let params = new HttpParams();
    if (query === undefined) {
      return params;
    }
    if (query.page !== undefined) {
      // HttpParams is immutable — set() returns a new one, so the result must be reassigned.
      params = params.set("page", String(query.page));
    }
    const search = query.search?.trim() ?? "";
    if (search !== "") {
      // A blank search is left out entirely: "?search=" is a different request from no search.
      params = params.set("search", search);
    }
    if (query.activeOnly === true) {
      params = params.set("activeOnly", "true");
    }
    return params;
  }
}
