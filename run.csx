// declare libraries to be used in this function
#r "Newtonsoft.Json"
#r "System.Configuration"
#r "System.Data"

// declare "using" so that the function can be use in code.
using System;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

/*
<summary>
    create object class that match the data arrive in json format
</summary>

<param name = Types></param>
<param name = data></param>
<param name = Product></param>
*/
public class Types 
{
    public object type {get;set;} 
    public object url {get;set;} 
} 
public class Data 
{
    public string time {get;set;}
    public string value {get;set;}
}
public class Product
{
    public Types types { get; set; }
    public List<Data> data { get; set; }
    
}

/*
<summary>
    parse string date to datetime format
</summary>

<param name = string x></param>
*/
public static string convertDate(string x)
{

    DateTime dateValue;
    DateTime.TryParse(x, out dateValue);
    return dateValue.ToString();
    
}

/*
<summary>
    run function
</summary>

<param name = myQueueItem> </param>
<param name = log></param>
*/
public static void Run(string myQueueItem, TraceWriter log)
{

    // initialize new object named product
    Product product = new Product();

    // deserialized the object
    Product deserializedProduct = JsonConvert.DeserializeObject<Product>(myQueueItem);
    
    // get the attribute data of deserializedProduct and put it in array
    var data = deserializedProduct.data;

    // count to count no of loops
    int count = 0;

    /*
    <summary>
        loop every data in variable data. loop thorugh the whole array.
        check the attribute of each object and construct sql statement.
    </summary>

    <param name = Data> Array of data fetch from fb url </param>
    <return name = empty></return>
    */
    foreach (Data element in data)
    {
        // check the type of data attribute from the fetch data and construct sql statement
        if((String)deserializedProduct.types.type == "adsNetReqCount" ){

            /* 
                construct sql statement and put into variable text.
                type are replace with number:
                    1. advertisement request (count)
                    2. advertisement request (sum)
                    3. advertisement impression
                    4. advertisement click
                    5. advertisement revenue
            */
            var text = ($"INSERT INTO dbo.FBActivity (FBActivityCount, FBActivityDate, FBActivityType)" +
                        $" VALUES('{element.value}', '{convertDate(element.time)}', '1');");
            
            // called insertDB function to run the sql statement.
            insertDB(text);
        
        } 
        else if((String)deserializedProduct.types.type == "adsNetReqSum" ){

            var text = ($"INSERT INTO dbo.FBActivity (FBActivityCount, FBActivityDate, FBActivityType)" +
                        $" VALUES('{element.value}', '{convertDate(element.time)}', '2');");
            insertDB(text);
        
        }
        else if((String)deserializedProduct.types.type == "adsNetImp" ){

            var text = ($"INSERT INTO dbo.FBActivity (FBActivityCount, FBActivityDate, FBActivityType)" +
                        $" VALUES('{element.value}', '{convertDate(element.time)}', '3');");
            insertDB(text);
        
        }
        else if((String)deserializedProduct.types.type == "adsClick" ){

            var text = ($"INSERT INTO dbo.FBActivity (FBActivityCount, FBActivityDate, FBActivityType)" +
                        $" VALUES('{element.value}', '{convertDate(element.time)}', '4');");
            insertDB(text);
        
        }
        else if((String)deserializedProduct.types.type == "adsRevenue" ){

            var text = ($"INSERT INTO dbo.FBActivity (FBActivityCount, FBActivityDate, FBActivityType)" +
                        $" VALUES('{element.value}', '{convertDate(element.time)}', '5');");
            insertDB(text);
        
        }

        // ignore other type or value from the data. do nothing
        else {

        }

        // count the number of loops
        count++;
    }

    // log the number of loop for debugging purposes
    log.Info(count.ToString());

}

/*
<summary>
    function insertDB is to accept parameter string containing sql command and execute it.
    insert data to table based on the sql command.
</summary>

<param name = commandText> string datatype name commandText </param>

<return> execute sql command </return>
*/
public static void insertDB(string commandText)
{   
    // called sqldb connection based on connection string declare in azure function.
    // purpose is to establish connection to the database
    // comment either one line for development (sqldb_development) or live (sqldb_Gorgias)
    
    var str = ConfigurationManager.ConnectionStrings["sqldb_Gorgias" ].ConnectionString;
    // var str = ConfigurationManager.ConnectionStrings["sqldb_development" ].ConnectionString;

    // declare sql connection based on the connection string
    using (SqlConnection conn = new SqlConnection(str))
    {
        
        // open the sql connection
        conn.Open();

        // initialized SqlCommand object to accept sql statement
        // SqlCommand accept sql sattement and connection string to execute
        using (SqlCommand cmd = new SqlCommand(commandText, conn))
        {
            // use try and catch method
            try 
            {
                // execute sql statement
                cmd.ExecuteNonQuery();
            }
            catch 
            {
                // throw error if an error occurs
                throw;
            }

        }
    }
}

