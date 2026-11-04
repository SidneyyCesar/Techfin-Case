CREATE TABLE IF NOT EXISTS Transactions
(
    Id TEXT PRIMARY KEY,
    RequestId TEXT UNIQUE,
    TransactionId TEXT UNIQUE,
    CustomerId TEXT NOT NULL,
    Amount REAL NOT NULL,    
    CreatedAt TEXT NOT NULL,
    Status TEXT NOT NULL
);


