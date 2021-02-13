1) For success application start you should be start first migration to OnlineMarket.DataAccess where the database will populate the superadmin account and another roles.
(if you want to change superadmin data go to OnlineMarket.DataAccess/DbInitializer and change, run migration to OnlineMarket.DataAccess or you will have an exeption)

2) Application use BrainTreePayment, MailJET, TWILIO, add your API's keys and id to OnlineMarket/appsettings.json
   PS Admin and Employee can cancel order and refund if something goes wrong, not customer. If order status shipped, you can refund only in BrainTree Transaction page.

3) To send EMAIL, the application uses MailJET. It's model is in OnlineMarket.Utility/MailJetEmailSender
   To change API Key and Id go to OnlineMarket/appsettings.json

4) To send SMS, the application uses TWILIO. It's model is in OnlineMarket.Utility/SmsSendSettings
   To change API PhoneNumber, AuthToken and Id go to OnlineMarket/appsettings.json
   To configure SMS-sender message go to Areas/Customer/Controller/CartController/OrderConfirmation
   PC: Turn on Permission to send SMS to your region! To change the first registration phone number for users, go to Identity/Pages/Account/Register and go to code line 38.
   Turn on permission to send SMS for your region on the Twilio page!

5) To change products count on the view page, go to HomeController in the pageSize field.

6) To change the phonenumber placeholder go to Areas/Identity/Pages/Account and change the fileds in Register.cshtml(53) and ExternalLogin.cshtml(35)

7) There are 5 roles in the app(SuperAdmin, Admin, Employee, Individual Customer and Company Customer). 
   SuperAdmin and Admin can do anything.
   Employee can add/handle product, orders and manage application data(address, social linck), but can't add another employees.

8) After publish add app domain to facebook and google or users won't registration with them account.
   Don't forget about Facebook Login/Settings/ and add app domain to Valid OAuth Redirect URLs:
   https://yourdomainname.net/signin-facebook