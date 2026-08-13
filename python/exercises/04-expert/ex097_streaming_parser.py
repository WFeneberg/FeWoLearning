"""Exercise 097 — incremental parsing with an explicit state machine (expert).

Goal:   Parse a stream of length-prefixed frames (a 4-byte big-endian length,
        followed by that many bytes of payload) that arrives in arbitrary chunks —
        one byte at a time, ten frames at once, or split right through the middle
        of a length prefix. The parser must not assume chunk boundaries line up
        with frame boundaries at all.
Drills: an explicit state machine (waiting for a header vs. waiting for the rest of
        a body) driven by how many bytes are needed *next*, buffering partial input
        across calls, and turning "not enough bytes yet" into "wait for the next
        `feed`" rather than an error.
Passes: when `pytest exercises/04-expert/test_ex097_streaming_parser.py` is green.

Note:   `encode_frame` is the encoder side (build a frame from a payload);
        `FrameParser` is the decoder side, and the two are inverses of each other —
        tests build inputs with one and check outputs against the other.
"""

import enum

HEADER_SIZE = 4


def encode_frame(payload: bytes) -> bytes:
    """Encode `payload` as a frame: its length as 4 big-endian bytes, then itself."""
    raise NotImplementedError


class _State(enum.Enum):
    HEADER = enum.auto()
    BODY = enum.auto()


class FrameParser:
    """Incrementally decodes a stream of `encode_frame`-framed payloads.

    Call `feed` with however many bytes just arrived — any split is fine, including
    mid-header or mid-payload — and it returns every payload that became complete
    as a result of this call (possibly none, possibly several).
    """

    def __init__(self) -> None:
        """Start in the HEADER state, needing `HEADER_SIZE` bytes, with an empty
        internal buffer."""
        raise NotImplementedError

    def feed(self, chunk: bytes) -> list[bytes]:
        """Append `chunk` to the internal buffer, then decode as far as the
        buffered bytes allow.

        Loop: while the buffer holds at least as many bytes as the current state
        needs, consume them — in HEADER, that means reading the length and
        switching to BODY needing that many bytes; in BODY, that means slicing out
        the payload, appending it to the result, and switching back to HEADER
        needing `HEADER_SIZE` bytes. Stop (without erroring) once the buffer no
        longer holds enough for whatever is needed next — the rest arrives in a
        later `feed` call.
        """
        raise NotImplementedError

    def pending_bytes(self) -> int:
        """How many bytes are currently buffered but not yet part of a decoded
        payload (a partial header or a partial body)."""
        raise NotImplementedError
