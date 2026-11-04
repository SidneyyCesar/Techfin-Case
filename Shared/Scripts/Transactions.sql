CREATE TABLE IF NOT EXISTS Transactions
(
    Id TEXT PRIMARY KEY,
    CustomerId TEXT NOT NULL,
    Amount REAL NOT NULL,
    CreatedAt TEXT NOT NULL
);