from ex097_streaming_parser import FrameParser, encode_frame


def test_encode_frame_prefixes_a_four_byte_big_endian_length():
    assert encode_frame(b"hi") == b"\x00\x00\x00\x02hi"


def test_encode_frame_with_empty_payload():
    assert encode_frame(b"") == b"\x00\x00\x00\x00"


def test_a_single_frame_fed_whole():
    parser = FrameParser()
    assert parser.feed(encode_frame(b"hello")) == [b"hello"]


def test_an_empty_payload_frame():
    parser = FrameParser()
    assert parser.feed(encode_frame(b"")) == [b""]


def test_multiple_frames_in_one_chunk():
    parser = FrameParser()
    chunk = encode_frame(b"a") + encode_frame(b"bb") + encode_frame(b"ccc")

    assert parser.feed(chunk) == [b"a", b"bb", b"ccc"]


def test_a_frame_split_byte_by_byte_only_completes_on_the_last_byte():
    parser = FrameParser()
    frame = encode_frame(b"data")

    results = []
    for i in range(len(frame) - 1):
        results.append(parser.feed(frame[i : i + 1]))

    assert all(r == [] for r in results)
    assert parser.feed(frame[-1:]) == [b"data"]


def test_a_split_exactly_at_the_header_boundary():
    parser = FrameParser()
    frame = encode_frame(b"payload")

    assert parser.feed(frame[:4]) == []
    assert parser.feed(frame[4:]) == [b"payload"]


def test_a_split_in_the_middle_of_the_payload():
    parser = FrameParser()
    frame = encode_frame(b"0123456789")

    assert parser.feed(frame[:7]) == []
    assert parser.feed(frame[7:]) == [b"0123456789"]


def test_pending_bytes_reflects_unconsumed_buffered_input():
    parser = FrameParser()
    frame = encode_frame(b"xyz")

    parser.feed(frame[:2])
    assert parser.pending_bytes() == 2

    parser.feed(frame[2:])
    assert parser.pending_bytes() == 0


def test_frames_trickling_in_across_many_feed_calls():
    parser = FrameParser()
    stream = encode_frame(b"one") + encode_frame(b"two") + encode_frame(b"three")

    decoded: list[bytes] = []
    chunk_size = 3
    for i in range(0, len(stream), chunk_size):
        decoded.extend(parser.feed(stream[i : i + chunk_size]))

    assert decoded == [b"one", b"two", b"three"]


def test_parser_instances_are_independent():
    a = FrameParser()
    b = FrameParser()
    frame = encode_frame(b"hello")

    a.feed(frame[:2])  # only part of the 4-byte header — not enough to decode yet

    assert a.pending_bytes() == 2
    assert b.pending_bytes() == 0
