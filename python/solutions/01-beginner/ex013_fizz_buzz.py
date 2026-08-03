"""Exercise 013 — FizzBuzz and friends (reference solution)."""


def fizz_buzz(n: int) -> str:
    # The composite case has to be tested first, or 15 would return "Fizz".
    if n % 15 == 0:
        return "FizzBuzz"
    if n % 3 == 0:
        return "Fizz"
    if n % 5 == 0:
        return "Buzz"
    return str(n)


def fizz_buzz_range(start: int, stop: int) -> list[str]:
    return [fizz_buzz(n) for n in range(start, stop)]


def apply_rules(n: int, rules: list[tuple[int, str]]) -> str:
    if any(divisor == 0 for divisor, _ in rules):
        raise ValueError("apply_rules() divisor must not be zero")
    # Building the label from all matching rules removes the need for a special
    # composite branch: 15 matches both and concatenates to "FizzBuzz".
    label = "".join(text for divisor, text in rules if n % divisor == 0)
    return label or str(n)


def count_multiples(stop: int, divisor: int) -> int:
    if divisor == 0:
        raise ValueError("count_multiples() divisor must not be zero")
    return sum(1 for n in range(1, stop) if n % divisor == 0)


def is_leap_year(year: int) -> bool:
    return year % 4 == 0 and (year % 100 != 0 or year % 400 == 0)
