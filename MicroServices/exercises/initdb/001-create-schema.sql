-- ex032: runs once, when the Postgres container initialises an EMPTY data directory.
CREATE SCHEMA IF NOT EXISTS ordering;
CREATE TABLE IF NOT EXISTS ordering.orders (id integer PRIMARY KEY, sku text NOT NULL);
