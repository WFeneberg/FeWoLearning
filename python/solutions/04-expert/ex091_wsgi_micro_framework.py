"""Exercise 091 — a WSGI micro framework (reference solution)."""

import re
from typing import Any, Callable, Iterable
from urllib.parse import parse_qs

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
    def __init__(self, environ: dict[str, Any]) -> None:
        self.environ = environ
        self.method = environ.get("REQUEST_METHOD", "GET").upper()
        self.path = environ.get("PATH_INFO", "/")
        self.query = parse_qs(environ.get("QUERY_STRING", ""))
        length = int(environ.get("CONTENT_LENGTH") or 0)
        self.body = environ["wsgi.input"].read(length) if length else b""


class Response:
    def __init__(
        self,
        body: str = "",
        status: int = 200,
        headers: dict[str, str] | None = None,
    ) -> None:
        self.body = body
        self.status = status
        self.headers = dict(headers) if headers else {}

    def start(self, start_response: Callable[[str, list[tuple[str, str]]], Any]) -> list[bytes]:
        phrase = HTTP_STATUS_PHRASES.get(self.status, "")
        status_line = f"{self.status} {phrase}".rstrip()
        body_bytes = self.body.encode("utf-8")

        headers = []
        if "Content-Type" not in self.headers:
            headers.append(("Content-Type", "text/plain; charset=utf-8"))
        headers.extend(self.headers.items())
        headers.append(("Content-Length", str(len(body_bytes))))

        start_response(status_line, headers)
        return [body_bytes]


def _compile_pattern(pattern: str) -> re.Pattern[str]:
    parts = []
    for segment in pattern.split("/"):
        if segment.startswith("<") and segment.endswith(">"):
            name = segment[1:-1]
            parts.append(f"(?P<{name}>[^/]+)")
        else:
            parts.append(re.escape(segment))
    return re.compile("^" + "/".join(parts) + "$")


class App:
    def __init__(self) -> None:
        self._routes: list[tuple[list[str], re.Pattern[str], Handler]] = []

    def route(self, pattern: str, methods: list[str] | None = None) -> Callable[[Handler], Handler]:
        allowed = [m.upper() for m in (methods or ["GET"])]
        regex = _compile_pattern(pattern)

        def decorator(handler: Handler) -> Handler:
            self._routes.append((allowed, regex, handler))
            return handler

        return decorator

    def __call__(
        self, environ: dict[str, Any], start_response: Callable[[str, list[tuple[str, str]]], Any]
    ) -> Iterable[bytes]:
        request = Request(environ)
        path_matched = False

        for allowed, regex, handler in self._routes:
            match = regex.match(request.path)
            if not match:
                continue
            path_matched = True
            if request.method in allowed:
                result = handler(request, **match.groupdict())
                response = result if isinstance(result, Response) else Response(str(result))
                return response.start(start_response)

        if path_matched:
            return Response("Method Not Allowed", status=405).start(start_response)
        return Response("Not Found", status=404).start(start_response)
