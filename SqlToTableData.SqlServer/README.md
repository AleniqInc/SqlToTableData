# SqlToTableData.SqlServer
Get SQL SELECT query result as a table (2 dimensional array). This package only works with SQL Server databases.

# How to use

```
// import package
using SqlToTableData.SqlServer;

// set connection string following ADO.NET syntax
string connectionString = "Server=(localdb)\\mssqllocaldb;Database=SampleDB";

// create new instance of DataProducer
DataProducer dataProducer = new DataProducer(connectionString);

// construct your SQL SELECT query
string sqlQuery = "SELECT ID, PRODUCT_NANE, PRICE FROM PRODUCT";

// call DataProducer method to get results as a table
var result = dataProducer.GetTable(sqlQuery);

...

```


## Output example

For SQL query **SELECT ID, NAME, PRICE FROM PRODUCT**, result would look like this:

| ID | NAME  | PRICE | *[string, string, string]* |
| -- | ----- | ----- | ------------------------ |
|  1 | Eggs  |  6.50 | *[int, string, double]*    |
|  2 | Milk  |  9.20 | *[int, string, double]*    |

The first row (result[0]) conains array of strings representing column names.



