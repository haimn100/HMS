install self signed certificate. should use only https for image catpturing
	-- under iis root > Features View > Server Certificates > Create Self Signed Certificate

install IIS scripting tools from "internet information services" feature
run: cscript C:\inetpub\AdminScripts\adsutil.vbs set w3svc/2/uploadreadaheadsize 10000
	** or C:\inetpub\AdminScripts\adsutil.vbs set w3svc/1/uploadreadaheadsize 10000

install mysql server
import db.sql
create db user with credentials that exists in web.config

** For https **
Open IIS
Navigate under <your website>
Scroll down to Management and open Configuration Editor
Select following section (drop down at the top) system.webServer and expand it, then locate serverRuntime
you'll find there the current value of uploadReadAheadSize value, which you can change (change to 9000000)


configure https in website binding
configure casa-benjamin.system in host file

verify ghost user in database (is_hidden = true)

configure logging folder with nlog file

* give permissons to the guestimages folder

App Pool Advanced Settings
------------------------------
Disable rapid failure in app pool
Increase proccess idle timeout
Load User Profile = false
Change Identety to NetworkService
disable recycling


-- verify "GuestImages"  folder exist under application root


--- DB Change ---
added app_settings
added cash_register_event
changed reservation
changed room
changed room_bed
changed check_outs (added credit_charge_percentage, cash_deposit , credit deposit)
changed menu_order (added credit_charge_percentage)
added pin to the staff table
add no-image.png to guestimages
added "discount" to menu_order
added to user table barcode and document_type
added table user_repay


added default image to images folder

yaa