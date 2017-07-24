# QueueTrigger
Trigger function based on queue storage on Azure function. 
Establish connection to the sql server database and execute sql query statement. 

## How To
1. Go to azure function > Platform features > Under development tools, select Advance tools (Kudu).   
2. On the option header, choose Debug console dropdown > CMD   
3. Select (+) sign on the root directory to create a new file/folder   
4. Upload the file to the desire folder  
5. Test and run the function   
6. ......   
7. Profit!

### Setup file

* create a connection string in azure function app. put your credential information as well as database server endpoint
```javascript
var str = ConfigurationManager.ConnectionStrings["sqldb_Gorgias" ].ConnectionString;
```
