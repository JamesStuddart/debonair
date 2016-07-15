# debonair
Micro Orm - Its just got simpler

![Debonair](http://jamesstuddart.co.uk/Content/Images/debonair-round.png)

Debonair is a very light weight ORM, that does not contain object tracking, it does not support child objects. Its back to basics, Keep It Simple Stupid.

## Why Debonair?
The goal of debonair is to take things back to basics, providing quick data access, while stopping you (the developer) attempting to make the orm do the heavy lifting that should be done by the database. The likes of Entity Framework and Linq2SQL are great, but are very slow, produce horrible sql and let you do a lot of stuff that you really shouldn't be doing.

*If you want masses of bells and whistles then Debonair probably isn't for you, but if you just want access to your data then give it a try.*

---

## Setup

Simply build the solution and add the DLL to your projects

**OR**

Debonair is alaso available via Nuget, [NuGet library](https://www.nuget.org/packages/Debonair/) so you can add it to your project via the Nuget Package Manager.


## Usage
Debonair uses standard connection strings, but currently only supports Microsoft SQL Server, it also manages the connection state, you do not need to open and close the connections yourself.

You can declare the repositories when you need, as shown in the crude examples, or you can implement a better more robust soltuion. I suggest you build a more robust solution.

*Example usage:*
```CSharp
public class ServiceManager : IDisposable
{
    public SqlConnection Connection { get; set; }

    public ServiceManager(SqlConnection connection)
    {
        Connection = connection;
    }

    public IDataRepository<MyObject> FailItems => new DataRepository<MyObject>(Connection);

    #region disposable
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {

            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion disposable
}
```
### Execute a select query
*Example usage:*

Note: All your entities need to inherit from the Debonair base class **DebonairStandard**

#### Gotta Select 'Em All! 
```csharp
public class Customer : DebonairStandard
{
    [KeyProperty(true)]
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}   
       
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));
 
var foundCustomers = repo.Select();
```

#### Select with criteria

We harness the power of linq to produce the *WHERE* clauses, this means its sleak and familiar to you.


```csharp
public class Customer : DebonairStandard
{
    [KeyProperty(true)]
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}   
       
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));

//Will select customers with the Id of 2000
var customer2000 = repo.Select(x => x.Id == 2000).FirstOrDefault();
 
//Will select customers with the name Smith
var smithCustomers = repo.Select(x => x.CustomerName.Contains("Smith"));

//Will select non-Gmail Customers
var noGamilers = repo.Select(x => !x.EmailAddress.EndsWith("@gmail.com"));
```

### Insert an object
When inserting a new object, if you have defined the primary key, this will be updated to the new Id from the database.

*Example Usage:*
```csharp
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));

var customer = new Customer(){ CustomerName = "Joe Bloggs", DateOfBirth = DateTime.Now, EmailAddress = "joe@bloggs.com"};

repo.Insert(customer);
```
### Update an object
We are editing the customer we inserted above.

*Example Usage:*
```csharp
//See Insert an object above...

customer.EmailAddress = "joe.bloggs@gmail.com";

repo.Update(customer);
```

### Delete an object
*Example Usage:*
```csharp
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));

var customer = repo.Select(x => x.CustomerName.StartsWith("Joe")).FirstOrDefault();

//This would just mark our object as deleted, IF the entity has an IsDeleted attributed property, see below for more information. 
 repo.Delete(customer);

//This would remove the row from the database
var foundCustomers = repo.Delete(customer, true);
```

## Stored Procedures

### Execute a query stored procedure
It was a conscious that Debonair would try and help you to not preform tasks that really should be done in the database. So all the CRUD functionality is basic, if you need to do anything a little more complex then you should use a stored procedure. 

*The below example is not a good example of this, but should give you the idea*

*Example Usage:*
```csharp
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));
 
var foundCustomers = repo.ExecuteStoredProcedure("dbo.spGetCustomerByName", new {CustomerName = "Joe Bloggs"})
```
### Execute a nonquery stored procedure
Not all stored procedures will return any data, when this is the situation you can use the **ExecuteNonQueryStoredProcedure** method.

*Example Usage:*
```csharp
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));

repo.ExecuteStoredProcedure("dbo.spDeleteCustomersThatAreTooOld", new {MaxAge = 80})
```


## Features

### Table Names vs Entity Names
Your entities should reflect the table names within the database, where this isn't so or isn't possible you can use the attribute **Table** to define the tables name.

*Example Usage:*
You have an Entity called **Customer**, but it is correctly stored in the Database table **Customers**

```csharp
[Table("Customers")]
public class Customer : DebonairStandard
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}   
```

### Columns vs Properties
The properties of your entities, like tables, should accurately reflect the column names in the database. Where this isn't so or isn't possible you can use the attribute **Column** to define the tables name.  

*Example Usage:*
```csharp
[Table("Customers")]
public class Customer : DebonairStandard
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    [Column("DoB")]
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}   
```

### Schemas
You may have different schemas within your database, so not all tables sit in *dbo*, if this is the case you can attribute your classes with a schema name using **Schema**
This is used when generating the SQL that will be executed by your queries.

*Example Usage:*
```csharp
[Schema("Sales")]
[Table("Customers")]
public class Customer : DebonairStandard
{
    [KeyProperty(true)]
    public int Id { get; set; }
    public string CustomerName { get; set; }
    [Column("DoB")]
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}   
```


### Primary Keys
To define your entity's primary key, use the attribute **KeyProperty**
When inserting a new record, the primary key property will be updated with the new Id from the database.

*Note:* Your primary key needs to be an integer at this point

*Example Usage:*
```csharp
[Schema("Sales")]
[Table("Customers")]
public class Customer : DebonairStandard
{
    [KeyProperty(true)]
    public int Id { get; set; }
    public string CustomerName { get; set; }
    [Column("DoB")]
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
}   
```

### Soft Delete
Rather than deleting a row from the database it is often a requirement that the row is dismissed by the application, this is achieved with an *IsDeleted* type column.
You can achieve this with the attribute **IsDeletedProperty**

*Note:* Your IsDeleted property needs to be a boolean

*Example Usage:*
```csharp
[Schema("Sales")]
[Table("Customers")]
public class Customer : DebonairStandard
{
    [KeyProperty(true)]
    public int Id { get; set; }
    public string CustomerName { get; set; }
    [Column("DoB")]
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
    [IsDeleted]
    public bool IsDeleted { get; set; }
}   
```

You can override this attribute when required, by passing *true* into the Delete method on the repository.

*Example Usage:*
```csharp
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));

var customer = repo.Select(x => x.CustomerName.StartsWith("Joe")).FirstOrDefault();

//This would just mark our object as deleted, IF the entity has an IsDeleted attributed property. 
 repo.Delete(customer);

//This would remove the row from the database
var foundCustomers = repo.Delete(customer, true);
```

### Ignore me!
You will at times have properties that have nothing to do with the database structures and you need to tell Debonair to not try and use them. This can be acheived with the attribute **Ignore**

*Example Usage:*
```csharp
[Schema("Sales")]
[Table("Customers")]
public class Customer : DebonairStandard
{
    [KeyProperty(true)]
    public int Id { get; set; }
    public string CustomerName { get; set; }
    [Column("DoB")]
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
    [Ignore]
    public int Age {get { Return (int)(DateTime.Now - DateOfBirth)}};
}   
```


## Some crude performance stats for selecting

I have not run a lot of performance tests, this project was the child of a disagreement and was mainly to prove that basic retrieval of data using *Linq2SQL* can be slow.

Both *Linq2Sql* and *Debonair* are also mapping the results to the entities within the project, where as Sql Server Management Studio is just the raw selection of data.

<table>
    <tr>
        <th>Solution</th>
        <th>Record Count</th>
        <th>Quickest Duration (seconds)</th>
        <th>Longest Duration (seconds)</th>
    </tr>
    <tr>
        <td>SSMS</td>
        <td>1,800,265</td>
        <td>47</td>
        <td>50</td>
    </tr>
    <tr>
        <td>Debonair</td>
        <td>1,800,265</td>
        <td>48.56</td>
        <td>52.56</td>
    </tr>
    <tr>
        <td>Linq2Sql</td>
        <td>1,800,265</td>
        <td>63.23</td>
        <td>80.41</td>
    </tr>
</table>


*These times are not conclusive, but they are the extremes of several runs of the code.*

