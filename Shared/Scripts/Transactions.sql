CREATE TABLE IF NOT EXISTS Transactions
(
    Id TEXT PRIMARY KEY,
    TransactionId TEXT UNIQUE,
    CustomerId TEXT NOT NULL,
    Amount REAL NOT NULL,
    CreatedAt TEXT NOT NULL
);
