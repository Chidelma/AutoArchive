# Auto-Archive
Largest database for auctioned automobiles

### Platforms
 - AWS <br/>
 - Azure

### Dependencies (dotnet)
 - AWSSDK.Core <br/>
 - AWSSDK.Extensions.NETCore.Setup <br/>
 - AWSSDK.S3 <br/>
 - CsvHelper <br/>
 - HtmlAgilityPack <br/>
 - MailKit <br/>
 - Microsoft.AspNetCore.Razor.Design <br/>
 - Microsoft.AspNetCore.Razor.SqlServer <br/>
 - Microsoft.AspNetCore.Razor.Tools <br/>
 - morelinq <br/>
 - System.Runtime.Caching <br/>
 - WebEssentials.AspNetCore.PWA <br/>
 - WindowsAzure.Storage 

### Data, Storage and Format
The Data is stored in both Amazon RDS (Sql Server) and Azure Sql Server (backup). The data is imported from an CSV file and its respective images through the website by an admin user. The data in the file is first sent directly to AWS RDS and then the file itself (CSV and images) is sent to Azure Storage as a blob. Once the blob is uploaded, the blob triggers a Logic App Service to import the data from the blob to the Azure Sql Server Database using a Stored Procedure within Azure Sql Server Database. Code for this process can be found in the directory /Controllers/UploadController.cs

#### The format for the database goes as follows:
 - ID : Int <br/>
 - Year : Int <br/>
 - Model : String <br/>
 - VIN : String <br/>
 - Mileage : Int <br/>
 - Auction Date : Date (YYYY-MM-DD) <br/>
 - Price : Int <br/>
 - CYL : Int <br/>
 - First Image : byte array <br/>
 - Second Image : byte array <br/>
 - Third Image : byte array <br/>
 - Fourth Image : byte array <br/>
 - Fifth Image : byte array <br/>
 - Duplicate : binary (1 or 0) <br/>
 - Coments : xml (array)

### Authenication
AWS Cognito is used for authenication. When a user signs up on the web app, their details are saved in AWS Cognito and also in a seperate Table (USERS) in AWS RDS (Sql Server) Database. When a user signs into the web their ID (sub) is retireved through claims and compared to the USERS Table in database to retrieve their record. A user must sign up in order to use the web app and all its functionalities. User have the option to use Facebook and Google to register and login. 

#### Admin Authenication
For users who are part of the admin group. These users are created through the Azure Active Directory in Azure Portal, then AAD is link to AWS Cognito as a federated login using SAML.

#### User details stored at AWS Cognito: 
 - Name : string (Collected at sign Up) <br/>
 - Email : string (Collected at sign Up) <br/>
 - Sub : UUID (Created at sign Up) <br/>
 - Password : string (Collected at sign up and unknown) <br/>

#### User details stored at AWS RDS:
 - Name : string <br/>
 - Email : string <br/>
 - Sub : string <br/>
 - UserGarage : xml (array) <br/>
 - CompareGarage : xml (array)
 
### Hosting 
This web app is hosted on Azure Web App Service resource. Deloyment is easily managed by selecting the correct Web App Server and a click of a button using Visual Studio 2017 or 2019, assuming the creditenals on Visual Studio match that of Azure Portal.

### Usage
Once a user is logged in, they're greeted with the "Popular Auctions of the Week", how these items are selected can be found in the "getWeekAuctions" method at /Controllers/HomeController.cs directory. The user has the ability to search for any record they want using the search bar and filter records by selecting filter options such as Model, Year, CYL, Max Mileage, Min Mileage, Max Price, Min Price, Only Images, Has Auction History and Auction Date. How the search terms are queried can be found in the "Results" method at /Controllers/HomeController.cs directory. 

#### View Format of Records (Search Results)
Each record is viewed as an index card (rectangle). Each record has an image (First Image) and if there are no images, an image message is shown instead.  Then the details of the record are shown with their designated labels which are VIN, CYL, Model, Auctioned Date, Price and Mileage. Each record also has two buttons; "Add to Garage" and "Compare", and a third which is revealed when the mouse is hovered on the record itself showing a fade in "View Analytics" message. 

#### User Garage View
This view is divided into two sections: the first section can contain side by side records (6 records max) for comparsion. In order to add a record for comparsion, a user needs to click the "Compare" button from search results or from the "Popular Auctions of the Week" view. The user can add a total of six records, once the max has being reached, the "Compare" button will no longer available. As a record can be added for comparsion, so can it be removed from comparsion; a "remove" button is present at the buttom of each record in the first section. The second section can contain a list/collection of records without any relation to each order. The purpose for this section is for users to add any record to their garage so that they won't need to search for them again; they simple go to their garage. In order to add a record to this section, a user needs to click "Add to garage" button from the search results. Each record in this section is formatted the same as search result records except that the two buttons are "Remove" (remove a record from garage) and "Compare" (to add the record for comparision). These proccesses can be found in the /Controller/GarageController.cs directory.

#### Analytics View 
The analytics view is divided into three sections: Line Chart of Record's price history, Record Auction History list with Record Statistics and Comments for that record(s). All the processes for the this view can found in the /Controller/AnalyticController directory. For the line chart, the record's price(s) (y-axis) and Auction Date(s) (x-axis) for the record itself is displayed. As for the record's auction history list, this is a list of the record's current and past auctions with the same view exactly as the search results. The record's statics shows: the number of times the record has being auctioned, the max, min and average price of the record, preicted price for future auction, average price of relative records and value profit/buy price (potential profit when record is fixed and resold within the year). The comments section is a list of comments from other users and where the current user can post a comment also. 

### Improvements
Speed of page loads need to be faster, considering switching to scripting languages: Javascript for Front End and Data Logic (post and get from database); Python for Back End and Business Logic (Calculations from Database). The python scripts will be AWS Lambda (Serverless), while javascript code will be stored in an S3 bucket (http hosting) and will be visible in the client browser. As for the database, move to either dynamodb or firestore (firebase), Hence switching from relational database to NoSql database. Authenication schemes will still stay the same. Overall this project will be using Javascript Framework (Svelte specifically). 

### Native App
For the native app will be developed using React Native (Javascript/Typescript) for easier code portability from the web app and will have the same UI and UX has the mobile view of the web app. Since the web app will have PWA (Progrssive Web App) technology, the web app will be easily converted to an android native app using TWA (Trusted Web Activities). 
