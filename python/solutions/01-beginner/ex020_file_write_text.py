"""Exercise 020 — Writing text files (reference solution)."""

from pathlib import Path


def write_lines(path: Path, lines: list[str]) -> int:
    payload = "".join(f"{line}\n" for line in lines)
    # newline="\n" stops the platform translating "\n" to "\r\n", so the bytes on
    # disk are the same everywhere. write() returns characters, not bytes.
    with path.open("w", encoding="utf-8", newline="\n") as handle:
        return handle.write(payload)


def append_line(path: Path, line: str) -> None:
    # "a" creates the file when missing and always writes at the end.
    with path.open("a", encoding="utf-8", newline="\n") as handle:
        handle.write(f"{line}\n")


def write_once(path: Path, text: str) -> None:
    # "x" fails if the path exists. Checking exists() first would leave a window
    # in which someone else creates the file between the check and the write.
    with path.open("x", encoding="utf-8") as handle:
        handle.write(text)


def write_in_new_dir(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


def write_csv_ish(path: Path, rows: list[tuple[str, int]]) -> None:
    lines = ["name,value"] + [f"{name},{value}" for name, value in rows]
    write_lines(path, lines)


def round_trip(path: Path, lines: list[str]) -> list[str]:
    write_lines(path, lines)
    return path.read_text(encoding="utf-8").splitlines()
