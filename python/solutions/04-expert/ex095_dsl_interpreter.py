"""Exercise 095 — a tiny expression language: tokenizer, parser, evaluator (reference solution)."""

from dataclasses import dataclass
from typing import Union


class DSLSyntaxError(Exception):
    pass


@dataclass(frozen=True)
class Token:
    kind: str
    text: str


@dataclass(frozen=True)
class Num:
    value: float


@dataclass(frozen=True)
class Var:
    name: str


@dataclass(frozen=True)
class BinOp:
    op: str
    left: "Expr"
    right: "Expr"


@dataclass(frozen=True)
class UnaryOp:
    op: str
    operand: "Expr"


Expr = Union[Num, Var, BinOp, UnaryOp]

_SINGLE_CHAR_TOKENS = {
    "+": "PLUS",
    "-": "MINUS",
    "*": "STAR",
    "/": "SLASH",
    "(": "LPAREN",
    ")": "RPAREN",
}


def tokenize(source: str) -> list[Token]:
    tokens: list[Token] = []
    i = 0
    n = len(source)
    while i < n:
        ch = source[i]
        if ch.isspace():
            i += 1
            continue
        if ch.isdigit():
            start = i
            seen_dot = False
            while i < n and (source[i].isdigit() or (source[i] == "." and not seen_dot)):
                if source[i] == ".":
                    seen_dot = True
                i += 1
            tokens.append(Token("NUMBER", source[start:i]))
            continue
        if ch.isalpha() or ch == "_":
            start = i
            while i < n and (source[i].isalnum() or source[i] == "_"):
                i += 1
            tokens.append(Token("IDENT", source[start:i]))
            continue
        if ch in _SINGLE_CHAR_TOKENS:
            tokens.append(Token(_SINGLE_CHAR_TOKENS[ch], ch))
            i += 1
            continue
        raise DSLSyntaxError(f"unexpected character {ch!r} at position {i}")
    tokens.append(Token("EOF", ""))
    return tokens


class Parser:
    def __init__(self, tokens: list[Token]) -> None:
        self._tokens = tokens
        self._pos = 0

    def _peek(self) -> Token:
        return self._tokens[self._pos]

    def _advance(self) -> Token:
        token = self._tokens[self._pos]
        self._pos += 1
        return token

    def _expect(self, kind: str) -> Token:
        token = self._peek()
        if token.kind != kind:
            raise DSLSyntaxError(f"expected {kind}, got {token.kind} ({token.text!r})")
        return self._advance()

    def parse(self) -> Expr:
        expr = self.parse_expr()
        self._expect("EOF")
        return expr

    def parse_expr(self) -> Expr:
        left = self.parse_term()
        while self._peek().kind in ("PLUS", "MINUS"):
            op_token = self._advance()
            right = self.parse_term()
            left = BinOp(op_token.text, left, right)
        return left

    def parse_term(self) -> Expr:
        left = self.parse_factor()
        while self._peek().kind in ("STAR", "SLASH"):
            op_token = self._advance()
            right = self.parse_factor()
            left = BinOp(op_token.text, left, right)
        return left

    def parse_factor(self) -> Expr:
        token = self._peek()
        if token.kind == "NUMBER":
            self._advance()
            return Num(float(token.text))
        if token.kind == "IDENT":
            self._advance()
            return Var(token.text)
        if token.kind == "MINUS":
            self._advance()
            return UnaryOp("-", self.parse_factor())
        if token.kind == "LPAREN":
            self._advance()
            expr = self.parse_expr()
            self._expect("RPAREN")
            return expr
        raise DSLSyntaxError(f"unexpected token {token.kind} ({token.text!r})")


def parse(source: str) -> Expr:
    return Parser(tokenize(source)).parse()


def evaluate(expr: Expr, env: dict[str, float]) -> float:
    if isinstance(expr, Num):
        return expr.value
    if isinstance(expr, Var):
        if expr.name not in env:
            raise NameError(f"undefined variable {expr.name!r}")
        return env[expr.name]
    if isinstance(expr, UnaryOp):
        return -evaluate(expr.operand, env)
    if isinstance(expr, BinOp):
        left = evaluate(expr.left, env)
        right = evaluate(expr.right, env)
        if expr.op == "+":
            return left + right
        if expr.op == "-":
            return left - right
        if expr.op == "*":
            return left * right
        return left / right
    raise TypeError(f"unknown expression node: {expr!r}")
