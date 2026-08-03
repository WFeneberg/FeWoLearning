"""Exercise 012 — Digit arithmetic (reference solution)."""

_ALPHABET = "0123456789abcdef"


def sum_of_digits(number: int) -> int:
    total = 0
    remaining = abs(number)
    while remaining:
        remaining, digit = divmod(remaining, 10)
        total += digit
    return total


def digits(number: int) -> list[int]:
    remaining = abs(number)
    if remaining == 0:
        # The loop below would produce an empty list for 0.
        return [0]
    result: list[int] = []
    while remaining:
        remaining, digit = divmod(remaining, 10)
        result.append(digit)
    return result[::-1]


def digital_root(number: int) -> int:
    remaining = abs(number)
    while remaining >= 10:
        remaining = sum_of_digits(remaining)
    return remaining


def count_digits(number: int) -> int:
    return len(digits(number))


def reverse_number(number: int) -> int:
    sign = -1 if number < 0 else 1
    reversed_value = 0
    remaining = abs(number)
    while remaining:
        remaining, digit = divmod(remaining, 10)
        reversed_value = reversed_value * 10 + digit
    return sign * reversed_value


def to_base(number: int, base: int) -> str:
    if number < 0:
        raise ValueError("to_base() does not accept negative numbers")
    if not 2 <= base <= 16:
        raise ValueError("to_base() base must be between 2 and 16")
    if number == 0:
        return "0"

    out: list[str] = []
    remaining = number
    while remaining:
        remaining, digit = divmod(remaining, base)
        out.append(_ALPHABET[digit])
    return "".join(reversed(out))
