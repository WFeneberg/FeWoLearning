import io
from typing import Any

import pytest

from ex091_wsgi_micro_framework import App, Response


def make_environ(
    method: str = "GET", path: str = "/", query: str = "", body: bytes = b""
) -> dict[str, Any]:
    environ: dict[str, Any] = {
        "REQUEST_METHOD": method,
        "PATH_INFO": path,
        "QUERY_STRING": query,
        "wsgi.input": io.BytesIO(body),
    }
    if body:
        environ["CONTENT_LENGTH"] = str(len(body))
    return environ


def call_app(app: App, environ: dict[str, Any]) -> tuple[str, list[tuple[str, str]], bytes]:
    captured: dict[str, Any] = {}

    def start_response(status: str, headers: list[tuple[str, str]]) -> None:
        captured["status"] = status
        captured["headers"] = headers

    body = b"".join(app(environ, start_response))
    return captured["status"], captured["headers"], body


def test_basic_route_returns_the_handlers_body():
    app = App()

    @app.route("/")
    def index(request):
        return "hello"

    status, headers, body = call_app(app, make_environ(path="/"))

    assert status == "200 OK"
    assert body == b"hello"
    assert ("Content-Type", "text/plain; charset=utf-8") in headers
    assert ("Content-Length", "5") in headers


def test_dynamic_path_segment_is_passed_to_the_handler():
    app = App()

    @app.route("/users/<user_id>")
    def show_user(request, user_id):
        return f"user {user_id}"

    _status, _headers, body = call_app(app, make_environ(path="/users/42"))

    assert body == b"user 42"


def test_query_string_is_parsed():
    app = App()

    @app.route("/search")
    def search(request):
        return request.query["q"][0]

    _status, _headers, body = call_app(app, make_environ(path="/search", query="q=python"))

    assert body == b"python"


def test_request_body_is_readable():
    app = App()

    @app.route("/echo", methods=["POST"])
    def echo(request):
        return request.body.decode("utf-8").upper()

    _status, _headers, body = call_app(app, make_environ(method="POST", path="/echo", body=b"hi"))

    assert body == b"HI"


def test_unmatched_path_is_404():
    app = App()

    @app.route("/")
    def index(request):
        return "hello"

    status, _headers, body = call_app(app, make_environ(path="/nope"))

    assert status == "404 Not Found"
    assert body == b"Not Found"


def test_matched_path_wrong_method_is_405():
    app = App()

    @app.route("/only-get", methods=["GET"])
    def only_get(request):
        return "ok"

    status, _headers, body = call_app(app, make_environ(method="POST", path="/only-get"))

    assert status == "405 Method Not Allowed"
    assert body == b"Method Not Allowed"


def test_handler_may_return_a_response_directly():
    app = App()

    @app.route("/blocked")
    def blocked(request):
        return Response("nope", status=403, headers={"X-Reason": "blocked"})

    status, headers, body = call_app(app, make_environ(path="/blocked"))

    assert status == "403 Forbidden"
    assert body == b"nope"
    assert ("X-Reason", "blocked") in headers


def test_first_matching_route_wins():
    app = App()

    @app.route("/users/<user_id>")
    def dynamic(request, user_id):
        return f"dynamic:{user_id}"

    @app.route("/users/me")
    def me(request):
        return "static:me"

    _status, _headers, body = call_app(app, make_environ(path="/users/me"))

    assert body == b"dynamic:me"


def test_content_length_counts_encoded_bytes_not_characters():
    app = App()

    @app.route("/greet")
    def greet(request):
        return "café"  # 4 characters, 5 bytes in UTF-8

    _status, headers, body = call_app(app, make_environ(path="/greet"))

    assert body == "café".encode("utf-8")
    assert ("Content-Length", "5") in headers
