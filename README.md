# cvdaETL

Takes csvs for CVDACTION and pushes to the database for PowerBI

The command prompt for the script is

cvdaETL.exe -f "<path to csv folder including root>" -d "2024-05-19"

-d is in date format YYYY-MM-DD (example above)

eg cvdaETL.exe -f "C:\PowerBI\cvda\csvs" -d "2024-05-19"

Example above.

 -d represents the insertion date (so the date you ran the script).
 -f is the path to the folder location
