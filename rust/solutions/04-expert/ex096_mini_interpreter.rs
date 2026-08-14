//! Exercise 096 — A mini interpreter: tokenizer, AST, evaluation,
//! environments (expert).
//! Goal:   a tiny interpreter for arithmetic-with-variables programs like
//!         `"let x = 10; let y = x * 2; x + y;"`. Four stages, each a small
//!         piece: `tokenize` (text → `Token`s), a recursive-descent `Parser`
//!         (tokens → `Stmt`/`Expr` AST, handling `*`/`/` binding tighter than
//!         `+`/`-`, and parentheses), `eval_expr` (AST → `i64`, resolving
//!         variables against an `Environment`), and `run` (drives a whole
//!         program, updating the environment for `let` and collecting the
//!         value of every bare-expression statement).
//! Drills: a hand-written lexer, recursive-descent parsing with operator
//!         precedence via two mutually-deferring functions, tree-walking
//!         evaluation, a `HashMap`-backed environment.

use std::collections::HashMap;

#[derive(Debug, Clone, PartialEq)]
pub enum Token {
    Number(i64),
    Ident(String),
    Plus,
    Minus,
    Star,
    Slash,
    LParen,
    RParen,
    Let,
    Equals,
    Semicolon,
}

/// Splits `input` into tokens. Whitespace is skipped. The bare word `let` is
/// the `Let` keyword; any other run of ASCII letters/digits/`_` starting
/// with a letter or `_` is an `Ident`; a run of ASCII digits is a `Number`.
pub fn tokenize(input: &str) -> Vec<Token> {
    let chars: Vec<char> = input.chars().collect();
    let mut tokens = Vec::new();
    let mut i = 0;
    while i < chars.len() {
        let c = chars[i];
        match c {
            ' ' | '\t' | '\n' | '\r' => i += 1,
            '+' => {
                tokens.push(Token::Plus);
                i += 1;
            }
            '-' => {
                tokens.push(Token::Minus);
                i += 1;
            }
            '*' => {
                tokens.push(Token::Star);
                i += 1;
            }
            '/' => {
                tokens.push(Token::Slash);
                i += 1;
            }
            '(' => {
                tokens.push(Token::LParen);
                i += 1;
            }
            ')' => {
                tokens.push(Token::RParen);
                i += 1;
            }
            '=' => {
                tokens.push(Token::Equals);
                i += 1;
            }
            ';' => {
                tokens.push(Token::Semicolon);
                i += 1;
            }
            c if c.is_ascii_digit() => {
                let start = i;
                while i < chars.len() && chars[i].is_ascii_digit() {
                    i += 1;
                }
                let text: String = chars[start..i].iter().collect();
                tokens.push(Token::Number(text.parse().expect("digit run parses as i64")));
            }
            c if c.is_ascii_alphabetic() || c == '_' => {
                let start = i;
                while i < chars.len() && (chars[i].is_ascii_alphanumeric() || chars[i] == '_') {
                    i += 1;
                }
                let word: String = chars[start..i].iter().collect();
                if word == "let" {
                    tokens.push(Token::Let);
                } else {
                    tokens.push(Token::Ident(word));
                }
            }
            other => panic!("unexpected character while tokenizing: {other:?}"),
        }
    }
    tokens
}

#[derive(Debug, Clone, PartialEq)]
pub enum Expr {
    Number(i64),
    Var(String),
    Add(Box<Expr>, Box<Expr>),
    Sub(Box<Expr>, Box<Expr>),
    Mul(Box<Expr>, Box<Expr>),
    Div(Box<Expr>, Box<Expr>),
}

#[derive(Debug, Clone, PartialEq)]
pub enum Stmt {
    /// `let NAME = EXPR ;`
    Let(String, Expr),
    /// `EXPR ;` — a bare expression statement, evaluated for its value.
    Expr(Expr),
}

/// A recursive-descent parser over a token slice, tracking its own read
/// position.
struct Parser<'t> {
    tokens: &'t [Token],
    pos: usize,
}

impl<'t> Parser<'t> {
    fn new(tokens: &'t [Token]) -> Self {
        Self { tokens, pos: 0 }
    }

    fn peek(&self) -> Option<&Token> {
        self.tokens.get(self.pos)
    }

    fn advance(&mut self) -> Option<&Token> {
        let tok = self.tokens.get(self.pos);
        self.pos += 1;
        tok
    }

    /// `additive := multiplicative (('+' | '-') multiplicative)*` — lower
    /// precedence than `*`/`/`, so `2 + 3 * 4` parses as `2 + (3 * 4)`.
    fn parse_additive(&mut self) -> Expr {
        let mut left = self.parse_multiplicative();
        loop {
            match self.peek() {
                Some(Token::Plus) => {
                    self.advance();
                    let right = self.parse_multiplicative();
                    left = Expr::Add(Box::new(left), Box::new(right));
                }
                Some(Token::Minus) => {
                    self.advance();
                    let right = self.parse_multiplicative();
                    left = Expr::Sub(Box::new(left), Box::new(right));
                }
                _ => break,
            }
        }
        left
    }

    /// `multiplicative := primary (('*' | '/') primary)*`
    fn parse_multiplicative(&mut self) -> Expr {
        let mut left = self.parse_primary();
        loop {
            match self.peek() {
                Some(Token::Star) => {
                    self.advance();
                    let right = self.parse_primary();
                    left = Expr::Mul(Box::new(left), Box::new(right));
                }
                Some(Token::Slash) => {
                    self.advance();
                    let right = self.parse_primary();
                    left = Expr::Div(Box::new(left), Box::new(right));
                }
                _ => break,
            }
        }
        left
    }

    /// `primary := NUMBER | IDENT | '(' additive ')'`
    fn parse_primary(&mut self) -> Expr {
        match self.advance().cloned() {
            Some(Token::Number(n)) => Expr::Number(n),
            Some(Token::Ident(name)) => Expr::Var(name),
            Some(Token::LParen) => {
                let expr = self.parse_additive();
                match self.advance() {
                    Some(Token::RParen) => {}
                    other => panic!("expected ')', got {other:?}"),
                }
                expr
            }
            other => panic!("unexpected token in primary position: {other:?}"),
        }
    }

    /// `statement := 'let' IDENT '=' additive ';' | additive ';'`
    fn parse_statement(&mut self) -> Stmt {
        if matches!(self.peek(), Some(Token::Let)) {
            self.advance();
            let name = match self.advance().cloned() {
                Some(Token::Ident(name)) => name,
                other => panic!("expected identifier after `let`, got {other:?}"),
            };
            match self.advance() {
                Some(Token::Equals) => {}
                other => panic!("expected '=' in let statement, got {other:?}"),
            }
            let expr = self.parse_additive();
            match self.advance() {
                Some(Token::Semicolon) => {}
                other => panic!("expected ';' at end of statement, got {other:?}"),
            }
            Stmt::Let(name, expr)
        } else {
            let expr = self.parse_additive();
            match self.advance() {
                Some(Token::Semicolon) => {}
                other => panic!("expected ';' at end of statement, got {other:?}"),
            }
            Stmt::Expr(expr)
        }
    }
}

/// Parses a full program: zero or more `;`-terminated statements.
pub fn parse(tokens: &[Token]) -> Vec<Stmt> {
    let mut parser = Parser::new(tokens);
    let mut statements = Vec::new();
    while parser.peek().is_some() {
        statements.push(parser.parse_statement());
    }
    statements
}

pub type Environment = HashMap<String, i64>;

/// Evaluates `expr` against `env`, resolving variables by lookup (panics on
/// an undefined variable).
pub fn eval_expr(expr: &Expr, env: &Environment) -> i64 {
    match expr {
        Expr::Number(n) => *n,
        Expr::Var(name) => *env
            .get(name)
            .unwrap_or_else(|| panic!("undefined variable: {name}")),
        Expr::Add(l, r) => eval_expr(l, env) + eval_expr(r, env),
        Expr::Sub(l, r) => eval_expr(l, env) - eval_expr(r, env),
        Expr::Mul(l, r) => eval_expr(l, env) * eval_expr(r, env),
        Expr::Div(l, r) => eval_expr(l, env) / eval_expr(r, env),
    }
}

/// Runs a full program against `env` (updating it in place for every `let`),
/// returning the value of every bare-expression statement, in order.
pub fn run(program: &[Stmt], env: &mut Environment) -> Vec<i64> {
    let mut results = Vec::new();
    for stmt in program {
        match stmt {
            Stmt::Let(name, expr) => {
                let value = eval_expr(expr, env);
                env.insert(name.clone(), value);
            }
            Stmt::Expr(expr) => {
                results.push(eval_expr(expr, env));
            }
        }
    }
    results
}

#[cfg(test)]
mod tests {
    use super::*;

    fn run_program(source: &str) -> Vec<i64> {
        let tokens = tokenize(source);
        let program = parse(&tokens);
        let mut env = Environment::new();
        run(&program, &mut env)
    }

    #[test]
    fn tokenize_produces_expected_tokens() {
        let tokens = tokenize("let x = 1 + 2;");
        assert_eq!(
            tokens,
            vec![
                Token::Let,
                Token::Ident("x".to_string()),
                Token::Equals,
                Token::Number(1),
                Token::Plus,
                Token::Number(2),
                Token::Semicolon,
            ]
        );
    }

    #[test]
    fn evaluates_arithmetic_with_precedence() {
        assert_eq!(run_program("2 + 3 * 4;"), vec![14]);
    }

    #[test]
    fn evaluates_parenthesized_expressions() {
        assert_eq!(run_program("(2 + 3) * 4;"), vec![20]);
    }

    #[test]
    fn subtraction_and_division_are_left_associative() {
        assert_eq!(run_program("20 - 5 - 5;"), vec![10]);
        assert_eq!(run_program("100 / 5 / 2;"), vec![10]);
    }

    #[test]
    fn let_bindings_are_visible_to_later_statements() {
        assert_eq!(run_program("let x = 10; let y = x * 2; x + y;"), vec![30]);
    }

    #[test]
    fn let_statements_produce_no_result_value() {
        assert!(run_program("let x = 5; let y = 6;").is_empty());
    }

    #[test]
    fn multiple_expression_statements_each_produce_a_result() {
        assert_eq!(run_program("1 + 1; 2 + 2; 3 + 3;"), vec![2, 4, 6]);
    }
}
