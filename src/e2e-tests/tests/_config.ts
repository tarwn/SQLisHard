export const testConfig = {
    exercises: {
        initial: {
            id: "S1.0",
            solution: "SELECT * FROM Customers;"
        },
        alternate: {
            id: "S1.1",
            solution: "SELECT Id, FirstName FROM Customers;",
            nextId: "S1.2"
        },
        withTip: {
            id: "S4.0"
        }
    },
    queries: {
        nonPaginated: "SELECT TOP 10 * FROM dbo.Customers",
        barelyPaginated: "SELECT TOP 101 * FROM dbo.Customers",
        veryPaginated: "SELECT TOP 1000 * FROM dbo.Customers"
    }
};