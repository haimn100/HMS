# Getting Started
install self signed certificate. should use only https for image catpturing

install fucking IIS scripting tools from "internet information services" feature
run: cscript C:\inetpub\AdminScripts\adsutil.vbs set w3svc/2/uploadreadaheadsize 10000

Open IIS
Navigate under <your website>
Scroll down to Management and open Configuration Editor
Select following section (drop down at the top) system.webServer and expand it, then locate serverRuntime
you'll find there the current value of uploadReadAheadSize value, which you can change
change to 9000000


verify ghost user in database (is_hidden = true)

config nlog.config for logging

App Pool Advanced Settings
------------------------------
Disable rapid failure in app pool
Increase proccess idle timeout
Load User Profile = false
Change Identety to NetworkService


ON CLIENT COMPUTERS
--------------------
edit host file
