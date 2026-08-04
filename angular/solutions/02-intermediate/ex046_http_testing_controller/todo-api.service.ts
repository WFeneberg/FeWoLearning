import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable, of, tap } from "rxjs";

// Exercise 046 — HttpTestingController (reference solution).

export interface Todo {
  readonly id: number;
  readonly title: string;
  readonly done: boolean;
}

@Injectable({ providedIn: "root" })
export class TodoApi {
  static readonly base = "/api/todos";

  private readonly http = inject(HttpClient);

  private cache: readonly Todo[] | null = null;

  get cached(): boolean {
    return this.cache !== null;
  }

  list(): Observable<readonly Todo[]> {
    if (this.cache !== null) {
      // of() is synchronous and never touches the transport, which is what makes expectNone
      // a meaningful assertion about this path.
      return of(this.cache);
    }
    return this.fetch();
  }

  refresh(): Observable<readonly Todo[]> {
    return this.fetch();
  }

  create(title: string): Observable<Todo> {
    const trimmed = title.trim();
    if (trimmed === "") {
      // Thrown eagerly, before any observable exists, so no request is ever queued.
      throw new RangeError("title must not be blank");
    }
    return this.http
      .post<Todo>(TodoApi.base, { title: trimmed, done: false })
      .pipe(tap(() => this.invalidate()));
  }

  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${TodoApi.base}/${id}`).pipe(tap(() => this.invalidate()));
  }

  setDone(id: number, done: boolean): Observable<Todo> {
    return this.http
      .patch<Todo>(`${TodoApi.base}/${id}`, { done })
      .pipe(tap(() => this.invalidate()));
  }

  private fetch(): Observable<readonly Todo[]> {
    return this.http.get<Todo[]>(TodoApi.base).pipe(
      // tap, not map: the cache is a side effect and the value passes through untouched. It
      // only runs on success, so a failed fetch leaves no cache behind.
      tap((todos) => (this.cache = todos)),
    );
  }

  private invalidate(): void {
    // Any write makes the list stale. Without this the UI keeps showing pre-write data.
    this.cache = null;
  }
}
