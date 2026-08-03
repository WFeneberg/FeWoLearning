"""Exercise 003 — String methods (reference solution)."""


def normalize_whitespace(text: str) -> str:
    # split() with no argument splits on runs of any whitespace and drops the
    # empty leading/trailing pieces, which is exactly the collapse we want.
    return " ".join(text.split())


def slugify(title: str) -> str:
    return "-".join(word.lower() for word in title.split())


def initials(full_name: str) -> str:
    return "".join(f"{word[0].upper()}." for word in full_name.split())


def mask_email(email: str) -> str:
    local, separator, domain = email.partition("@")
    if not separator:
        return email
    return f"{local[0]}{'*' * (len(local) - 1)}{separator}{domain}"


def count_case_insensitive(text: str, needle: str) -> int:
    if not needle:
        # str.count("") would return len(text) + 1, which is not a useful answer.
        return 0
    return text.lower().count(needle.lower())
