1) For success application start you should be start first migration to OnlineMarket.DataAccess where the database will populate the superadmin account and another roles.
(if you want to change superadmin data go to OnlineMarket.DataAccess/DbInitializer and change, run migration to OnlineMarket.DataAccess or you will have an exeption)

2) Application use Stripe Payment, SendGridKey and SendGridUser, add your API's keys and id to OnlineMarket/appsettings.json

3) To send EMAIL, the application uses SendGrid. It's model is in OnlineMarket.Utility/EmailSender and EmailOptions
   To change API Key and Id go to OnlineMarket/appsettings.json

4) To send SMS, the application uses TWILIO. It's model is in OnlineMarket.Utility/SmsSendSettings
   To change API PhoneNumber, AuthToken and Id go to OnlineMarket/appsettings.json
   To configure SMS-sender message go to Areas/Customer/Controller/CartController/OrderConfirmation
   PC: Turn on Permission to send SMS to your region! To change the first registration phone number for users, go to Identity/Pages/Account/Register and go to code line 38.
   Turn on permission to send SMS for your region on the Twilio page!

5) To change products count on the view page, go to HomeController in the pageSize field.

6) To change the phonenumber placeholder go to Areas/Identity/Pages/Account and change the fileds in Register.cshtml(53) and ExternalLogin.cshtml(35)