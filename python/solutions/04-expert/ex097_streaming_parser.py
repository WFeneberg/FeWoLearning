"""Exercise 097 — incremental parsing with an explicit state machine (reference solution)."""

import enum

HEADER_SIZE = 4


def encode_frame(payload: bytes) -> bytes:
    return len(payload).to_bytes(HEADER_SIZE, "big") + payload


class _State(enum.Enum):
    HEADER = enum.auto()
    BODY = enum.auto()


class FrameParser:
    def __init__(self) -> None:
        self._state = _State.HEADER
        self._buffer = bytearray()
        self._needed = HEADER_SIZE
        self._payload_length = 0

    def feed(self, chunk: bytes) -> list[bytes]:
        self._buffer.extend(chunk)
        frames: list[bytes] = []

        while len(self._buffer) >= self._needed:
            if self._state is _State.HEADER:
                header = bytes(self._buffer[:HEADER_SIZE])
                del self._buffer[:HEADER_SIZE]
                self._payload_length = int.from_bytes(header, "big")
                self._state = _State.BODY
                self._needed = self._payload_length
            else:
                payload = bytes(self._buffer[: self._payload_length])
                del self._buffer[: self._payload_length]
                frames.append(payload)
                self._state = _State.HEADER
                self._needed = HEADER_SIZE

        return frames

    def pending_bytes(self) -> int:
        return len(self._buffer)
