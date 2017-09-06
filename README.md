# debonair
Micro Orm - Its just got simpler

![version](https://img.shields.io/badge/version-v2.0.0.0-4FC921.svg) ![released](https://img.shields.io/badge/released-2017/09/06-D6AE22.svg) ![branch](https://img.shields.io/badge/branch-Master-A1A1A1.svg) ![branch](https://img.shields.io/badge/.Net-Standard_2.0-4FC921.svg)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/JamesStuddart/debonair/master/License)

![Debonair](/debonair-round.png?raw=true "Debonair Micro Orm")

Debonair is a very light weight ORM, that does not contain object tracking, it does not support child objects. Its back to basics, Keep It Simple Stupid.


## Updates
---
### Update v2.0.0.0
---
We have ported Debonair over to **.netStandard 2.0**, this has meant a few changes to the architecture.

#### Changes:
- Debonair project moved to **.Net Standard 2.0**
- Debonair.Tests project moved to **.Net Core 2.0**
- Unit test project moved to XUnit (N.B. The XUnit project is a **.netCore** project)
- Caching rewritten and now uses *Microsoft.Extensions.Caching.Memory*

#### Bugs Fixed:
- Insert/Update Column names not being populated when you provide an EntityMapping.
- Improved performance, although the mapping to your POCOs still needs some work.
- Errors fixed in README.md

#### Outstanding Issues/Problems:
- Mapping to POCOs is slow, when a lot of data is present, see [Performance Stats](#performance-stats-for-selecting) for more information
- Reliance on *SqlConnection* from *System.Data.SqlClient*
- Lacking quality Unit tests

---
### Update v1.1.0.0
---
We have removed the requirement of the base class *DebonairStandard* this was seen by many as breaking separation of concern and merging some of the Data and Business responsibilities, and thus coupling the entities to this ORM. 

We have also removed the attribute decorations for mapping the entities requirements (this is not to be mistaken with validation), as with the original base class requirement it was felt to be harming the separation of code and tightly coupling you into the Debonair ORM.

So, we couldn't just take away a load of features and not give you a way to achieve this, so we present to you in full technicolor! Debonair with its own **fluent mapping API**!  

***Please note: there are no useful unit tests for this library at this time, but we are working on fixing that, sorry to the TDD purists out there.***

---

## Why Debonair?
The goal of debonair is to take things back to basics, providing quick data access, while stopping you (the developer) attempting to make the orm do the heavy lifting that should be done by the database. The likes of Entity Framework and Linq2SQL are great, but are very slow, produce horrible sql and let you do a lot of stuff that you really shouldn't be doing.

*If you want masses of bells and whistles then Debonair probably isn't for you, but if you just want access to your data then give it a try.*

---

## Getting Debonair Setup

Simply build the solution and add the DLL to your projects

**OR**

Debonair is also available via Nuget, [NuGet library](https://www.nuget.org/packages/Debonair/) so you can add it to your project via the Nuget Package Manager.

## Usage
Debonair uses standard connection strings, but currently only supports Microsoft SQL Server, it also manages the connection state, you do not need to open and close the connections yourself.

You can declare the repositories when you need, as shown in the crude examples, or you can implement a better more robust solution. I suggest you build a more robust solution.

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

Note: Your entities no longer need to inherit from the Debonair base class **DebonairStandard**, which has now been removed

#### Gotta Select 'Em All! 
```csharp
public class Customer
{
   public int Id { get; set; }
   public string CustomerName { get; set; }
   public DateTime DateOfBirth { get; set; }
   public string EmailAddress { get; set; }
}   
       
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));
 
var foundCustomers = repo.Select();
```

#### Select with criteria

We harness the power of linq to produce the *WHERE* clauses, this means its sleek and familiar to you.


```csharp
public class Customer
{
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
var noGmailers = repo.Select(x => !x.EmailAddress.EndsWith("@gmail.com"));
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

### Fluent Mapping API

NEW to v1.1.0.0 is the Fluent Mapping API, this is designed to help you quickly and easily define the requirements of the properties within your entities, without tightly coupling your entities to this ORM.

To setup a mapping for the *Customer* entity that we saw earlier all we need to do is create a class that will contain our mapping information. This class can be placed anywhere in your system, and by default Debonair will find them and use them. Below is the blank mapping class, and as we continue we will fill it in.

*Example Usage:*
```csharp
public class CustomerMapping : EntityMapping<Customer>
{
   public CustomerMapping()
   {  

   }
}
```

### Table Names vs Entity Names
Your entities should reflect the table names within the database, where this isn't so or isn't possible you can use the mapping option **SetTableName** to define the tables name.

*Example Usage:*
You have an Entity called **Customer**, but it is correctly stored in the Database table **Customers**

```csharp
public class CustomerMapping : EntityMapping<Customer>
{
   public CustomerMapping()
   {
      SetTableName("Customers");
   }
}
```

### Columns vs Properties
The properties of your entities, like tables, should accurately reflect the column names in the database. Where this isn't so or isn't possible you can use the mapping option **Column** to define the tables name.  

*Example Usage:*
```csharp
public class CustomerMapping : EntityMapping<Customer>
{
   public CustomerMapping()
   {
      SetTableName("Customers");
      SetColumnName(x => x.DateOfBirth, "DoB");
   }
}  
```

### Schemas
You may have different schemas within your database, so not all tables sit in *dbo*, if this is the case you can attribute your classes with a schema name using **SetSchemaName**
This is used when generating the SQL that will be executed by your queries.

*Example Usage:*
```csharp
public class CustomerMapping : EntityMapping<Customer>
{
   public CustomerMapping()
   {
      SetTableName("Customers");
      SetSchemaName("Sales");
      SetColumnName(x => x.DateOfBirth, "DoB");
   }
}  
```
The mapping system also offers chaining of mappings, if you wish to use it, as seen below: 

*Example Usage:*
```csharp
public class CustomerMapping : EntityMapping<Customer>
{
   public CustomerMapping()
   {
   	  SetTableName("Customers").SetTableName("Sales");
      SetColumnName(x => x.DateOfBirth, "DoB");
   }
}  
```


### Primary Keys
To define your entity's primary key, use the mapping option **SetPrimaryKey**
When inserting a new record, the primary key property will be updated with the new Id from the database.

*Note:* Your primary key needs to be an integer at this point

*Example Usage:*
```csharp
public class CustomerMapping : EntityMapping<Customer>
{
   public CustomerMapping()
   {
      SetTableName("Customers").SetTableName("Sales");
      SetColumnName(x => x.DateOfBirth, "DoB");
      SetPrimaryKey(x => x.Id);
   }
}  
```

### Soft Delete
Rather than deleting a row from the database it is often a requirement that the row is dismissed by the application, this is achieved with an *IsDeleted* type column.
You can achieve this with the mapping option **IsDeleted**

*Note:* Your IsDeleted property needs to be a boolean

*Example Usage:*
```csharp
public class CustomerMapping : EntityMapping<Customer>
{
   public CustomerMapping()
   {   	
      SetTableName("Customers").SetTableName("Sales");
      SetColumnName(x => x.DateOfBirth, "DoB");
      SetPrimaryKey(x => x.Id);
      SetIsDeletedProperty(x => x.IsDeleted);
   }
}  
```

You can override this mapping when required, by passing *true* into the Delete method on the repository.

*Example Usage:*
```csharp
var repo = new DataRepository<Customer>(new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString));

var customer = repo.Select(x => x.CustomerName.StartsWith("Joe")).FirstOrDefault();

//This would just mark our object as deleted, IF the entity has an IsDeletedProperty mapping option . 
 repo.Delete(customer);

//This would remove the row from the database
var foundCustomers = repo.Delete(customer, true);
```

### Ignore me!
You will at times have properties that have nothing to do with the database structures and you need to tell Debonair to not try and use them. This can be acheived with the mapping option **Ignore**

*Example Usage:*
```csharp
public class Customer
{
   public int Id { get; set; }
   public string CustomerName { get; set; }
   public DateTime DateOfBirth { get; set; }
   public string EmailAddress { get; set; }
   public int Age {get { Return (int)(DateTime.Now - DateOfBirth)}};
}  

public class CustomerMapping : EntityMapping<Customer>
{
   public CustomerMapping()
   {      	
      SetTableName("Customers").SetTableName("Sales");
      SetColumnName(x => x.DateOfBirth, "DoB");
      SetPrimaryKey(x => x.Id);
      SetIsDeletedProperty(x => x.IsDeleted);
      SetIgnore(x => x.Age);
   }
}  
  
```


## Performance stats for selecting

I have not run a lot of performance tests, this project was the child of a disagreement and was mainly to prove that basic retrieval of data using *Linq2SQL* can be slow.

The results below come from running the Orm-Comparison project by Fran Hoey, you can find here [HERE](https://github.com/franhoey/Orm-Comparison)

*Note: Stats in Orm Comparison repository as based on an older version of Debonair*

**How Orm-Comparison work**

Using each ORM each Stored Procedure is timed while being executed 5000 times. Before the timed run each Store Procedure is executed 500 times to allow the ORM and Database to warm up.

**The Results**
> *These stats will vary depending on your machine, I know Fran had faster speeds on his runs*

|Orm                      	|SelectOne       	|SelectAll       	|RunNonQuery     |
|---------------------------|-------------------|-------------------|----------------|
|LinqToSql                	|00:00:02.5329189	|00:00:06.4598690	|00:00:01.7674956|
|EntityFramework          	|00:00:01.5493002	|00:00:05.7021158	|00:00:02.3565100|
|RawDataAccess            	|00:00:00.6248822	|00:00:03.8076024	|00:00:00.5771884|
|Dapper                   	|00:00:00.6452659	|00:00:03.1739489	|00:00:00.6425601|
|PetaPoco                 	|00:00:00.8815409	|00:00:03.6603908	|00:00:00.8907718|
|NPoco                    	|00:00:01.9791860	|00:00:04.8401289	|00:00:00.9578563|
|MicroLite                	|00:00:00.6442267	|00:00:03.7207547	|00:00:00.6357562|
|Debonair                 	|00:00:00.7615803	|00:00:14.8886053	|00:00:00.6443041|
|ServiceStack.OrmLite     	|00:00:00.6697889	|00:00:03.2370396	|00:00:00.5983928|

As you can see Debonair is performing well, until the SelectAll, this is an issue with mapping and need fixing.