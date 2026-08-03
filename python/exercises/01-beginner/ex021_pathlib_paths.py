"""Exercise 021 — pathlib (beginner).

Goal:   Manipulate paths as objects instead of gluing strings together.
Drills: Path joining with /, name/stem/suffix/parent, with_suffix, relative_to,
        glob, exists/is_file, iterating a directory deterministically.
Passes: when `pytest exercises/01-beginner/test_ex021_pathlib_paths.py` is green.
"""

from pathlib import Path


def join(base: Path, *parts: str) -> Path:
    """Join `parts` onto `base` using the / operator, not string concatenation."""
    raise NotImplementedError


def split_name(path: Path) -> tuple[str, str, str]:
    """Return ``(parent_as_str, stem, suffix)``.

    For ``/tmp/data/report.tar.gz`` the stem is ``report.tar`` and the suffix
    ``.gz`` — pathlib only peels one extension.
    """
    raise NotImplementedError


def change_extension(path: Path, suffix: str) -> Path:
    """Return the path with its extension replaced.

    `suffix` arrives with or without a leading dot; both must work. An empty
    suffix removes the extension.
    """
    raise NotImplementedError


def relative(path: Path, base: Path) -> Path:
    """Return `path` expressed relative to `base`.

    A `path` not under `base` raises ValueError — that is what ``relative_to``
    already does.
    """
    raise NotImplementedError


def list_files(directory: Path) -> list[str]:
    """Return the names of the files directly inside `directory`, sorted.

    Sub-directories are excluded. Iteration order from the filesystem is not
    guaranteed, so sort before returning.
    """
    raise NotImplementedError


def find_by_suffix(directory: Path, suffix: str) -> list[str]:
    """Return the sorted names of files anywhere under `directory` with `suffix`.

    `suffix` arrives like ``".txt"``. Searches recursively.
    """
    raise NotImplementedError


def ensure_dir(path: Path) -> Path:
    """Create `path` as a directory, including parents, and return it.

    Calling it again for an existing directory must not raise.
    """
    raise NotImplementedError
