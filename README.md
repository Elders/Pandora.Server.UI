#Pandora.Server.UI setup dev environment

#####Prerequisites:
- Roslyn
- .NET Framework 4.6
- ASP.NET 5
- HttpPlatformHandler v1.2 - http://www.iis.net/downloads/microsoft/httpplatformhandler#additionalDownloads (use the additional downloads section)

- - -
		Create google credentials for Pandora on https://console.developers.google.com
		(You can skip this step if you've already did it):

		1. Click on "Use Google APIs"
		2. Click on "Credentials" (on the left side panel)
		3. Click on "Create credentials" > "OAuth client ID"
		4. Select "Web application"
		5. Enter "Pandora" as a name
		6. Save the client id and the client secret.
		7. Go to "Overview" (on the left side panel)
		8. Click on "Google+ API"
		9. Click "Enable"

1. Clone the repository from `git@github.com:Elders/Pandora.Server.UI.git` or `https://github.com/Elders/Pandora.Server.UI.git`
2. Go to `Pandora.Server.UI\src` and create a copy of the `Pandora.Server.UI.Configuration.Sample` folder
3. Rename the newly created folder to `Pandora.Server.UI.Configuration` (remove `.Sample - Copy`)
4. Go to `Pandora.Server.UI.Configuration`
5. Open all Json config files in a text editor

 - Elders.Pandora.Server.Authentication.json
    This configuration file is for the web UI authentication
    * Replase the `machine-name` node with your machine's name (tip: execute `hostname` in cmd)
    * Set your client id
    * Set your client secret
    * Set `https://www.googleapis.com/auth/plus.login openid profile email` as scopes (yes, the first scope is a url)
 -  Elders.Pandora.Server.UI.json
    This contains the Pandora.Api url and the super admin user. Also references the other configuration file(s)
    * Replase the `machine-name` node with your machine's name
    * Set your email for `super_admin_users` (the same email you used for google credentials)

6. Open a cmd window as an administrator in the repository directory and execute the `set_variables-as-admin.bat` script
7. Execute `dnu restore`
8. Execute `run.cmd`
9. Happy coding!



