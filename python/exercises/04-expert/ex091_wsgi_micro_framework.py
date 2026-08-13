"""Exercise 091 — a WSGI micro framework (expert).

Goal:   Build the smallest thing that is still a real web framework: a WSGI
        callable, request/response objects wrapping the raw WSGI contract, and a
        router with path parameters — enough to see why frameworks like Flask
        exist without pulling one in.
Drills: the WSGI callable signature `(environ, start_response) -> iterable[bytes]`,
        parsing `PATH_INFO`/`QUERY_STRING`/the request body out of `environ`,
        compiling a route pattern like ``/users/<id>`` into a matching regex, and
        telling "no route matches this path" (404) apart from "a route matches, but
        not this method" (405).
Passes: when `pytest exercises/04-expert/test_ex091_wsgi_micro_framework.py` is green.

Note:   `HTTP_STATUS_PHRASES` is provided data, not part of the exercise — it is
        what turns a bare status code into the reason phrase a status line needs
        (``"200 OK"``, not ``"200"``).

Note:   there is no real server here — tests call `app(environ, start_response)`
        directly with a hand-built `environ`, exactly as a WSGI server would, which
        is the standard way to unit-test a WSGI application without opening a socket.
"""

import re
from typing import Any, Callable, Iterable

HTTP_STATUS_PHRASES = {
    200: "OK",
    201: "Created",
    400: "Bad Request",
    403: "Forbidden",
    404: "Not Found",
    405: "Method Not Allowed",
    500: "Internal Server Error",
}

Handler = Callable[..., Any]


class Request:
    """Wraps one WSGI `environ` dict with the pieces a handler actually wants."""

    def __init__(self, environ: dict[str, Any]) -> None:
        """Populate, from `environ`:

        - `method` — `REQUEST_METHOD`, uppercased.
        - `path` — `PATH_INFO`.
        - `query` — `QUERY_STRING` parsed with `urllib.parse.parse_qs` (each value a
          list of strings, `parse_qs`'s own behaviour — do not flatten it).
        - `body` — up to `CONTENT_LENGTH` bytes read from `environ["wsgi.input"]`,
          or `b""` if there is no content length (WSGI never guarantees this key is
          present, and a missing/empty value means "no body").
        """
        raise NotImplementedError


class Response:
    """A status code, a body, and headers — turned into the WSGI response contract
    by `start`."""

    def __init__(
        self,
        body: str = "",
        status: int = 200,
        headers: dict[str, str] | None = None,
    ) -> None:
        raise NotImplementedError

    def start(self, start_response: Callable[[str, list[tuple[str, str]]], Any]) -> list[bytes]:
        """Call `start_response(status_line, headers_list)` and return the body as
        WSGI wants it: an iterable of `bytes` chunks.

        `status_line` looks like ``"200 OK"`` — `HTTP_STATUS_PHRASES[self.status]`
        supplies the phrase. Encode `self.body` as UTF-8 and always send a
        `Content-Length` header computed from the *encoded* bytes, plus
        `Content-Type` (default ``"text/plain; charset=utf-8"`` unless `self.headers`
        already set one) and any other headers from `self.headers`.
        """
        raise NotImplementedError


def _compile_pattern(pattern: str) -> re.Pattern[str]:
    """Compile a route pattern into a fully-anchored matching regex.

    ``/users/<id>`` becomes something equivalent to ``^/users/(?P<id>[^/]+)$`` — a
    literal segment matches itself (escaped), a ``<name>`` segment becomes a named
    group matching anything but a slash.
    """
    raise NotImplementedError


class App:
    """A WSGI callable with route registration."""

    def __init__(self) -> None:
        raise NotImplementedError

    def route(self, pattern: str, methods: list[str] | None = None) -> Callable[[Handler], Handler]:
        """Return a decorator that registers `handler` for `pattern`.

        `methods` defaults to ``["GET"]``; store it uppercased. Registration order
        matters for matching (first match wins) but not for this method's return
        value — return the handler unchanged, so it is still usable directly.
        """
        raise NotImplementedError

    def __call__(
        self, environ: dict[str, Any], start_response: Callable[[str, list[tuple[str, str]]], Any]
    ) -> Iterable[bytes]:
        """Dispatch one request.

        Find the *first* registered route whose pattern matches `request.path`.

        - No route matches the path at all: 404.
        - At least one route matches the path, but none of those matches also
          accepts `request.method`: 405.
        - A match on both: call ``handler(request, **path_params)`` where
          `path_params` are the named groups the pattern captured. If the handler
          returns a `Response`, use it as-is; if it returns anything else, wrap it
          in ``Response(str(result))``.

        Either way, finish by calling `.start(start_response)` on the resulting
        `Response` and returning what that gives back.
        """
        raise NotImplementedError
