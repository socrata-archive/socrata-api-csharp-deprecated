The Socrata .NET API
====================

This library provides an interface to the [SODA][] Publisher API. If you're new to all this, you may want to brush up on the [getting started][] guide.

If you're curious about how things work under the hood, you can also browse the [API documentation][] directly.

[soda]: http://dev.socrata.com/
[getting started]: http://dev.socrata.com/publisher/getting-started
[api documentation]: http://opendata.socrata.com/api/docs/

Examples
========

First, make sure you've configured your application settings (see 'Configuration' below). Then you can move on to...

Creating a dataset
------------------

    View v = new View();
    v.name = "Cyborg Research Expenditure";

    Column c = new Column();
    c.name = "Project Title;
    c.Type = DataType.Text;
    v.Columns().Add(c);

    Column c2 = new Column();
    c2.name = "Dollar Amount";
    c2.Type = DataType.Money;
    v.Columns().Add(c2);

    v = v.Create();

Importing a file
----------------

    // Simplest method: allow the server to scan your columns
    // and choose data types as well as header names.
    View v = new View();
    v.name = "Cyborg Import";
    // Your first column is a header, so skip it.
    v = v.ImportFile("cyborgs.csv", 1);


    // OR, specify your columns before import
    View v = new View();
    v.name = "Single column import";

    Column c = new column();
    c.Type = DataType.Number;
    c.name = "First Column";
    v.Columns().Add(c);

    v = v.ImportFile("cyborgs_onecolumn.csv", 1);

Creating a working copy
-----------------------

    // Find your existing dataset, either by search or from ID
    View v = View.fromId("cybo-rgs1");

    // Create a working copy of the dataset
    View workingCopy = v.WorkingCopy();

Appending to a dataset
----------------------

    // Find your existing dataset, either by search or from ID
    View v = View.fromId("cybo-rgs1");
    View workingCopy = v.WorkingCopy();    
    workingCopy = workingCopy.Append("cyborg_update.csv");

Replacing the rows in an existing dataset
-----------------------------------------

    View v = View.fromId("cybo-rgs1");
    View workingCopy = v.WorkingCopy();
    workingCopy.Replace("cyborgs_replace.csv");

Publishing a dataset
--------------------
    
    View v = View.fromId("cybo-rgs1");
    v.Publish();

...and more!
------------

See the 'Tests' directory for more nitty-gritties, if that's your thing.

Configuration
=============

In order to use the [SODA][] API, you'll need both a Socrata account and an application token. To create an account, visit the [Sign Up][] link on your preferred Socrata-powered data site.

Once you have an account, you can register an application by going to your [profile page][]. If you're not writing a web application, you can fill in the Callback Prefix with any https://server that's a valid URL as this will not be used. You can always come back and change these fields later.

[soda]: http://dev.socrata.com/
[sign up]: http://opendata.socrata.com/signup
[profile page]: http://opendata.socrata.com/profile/app_tokens

Finished with all of that? You're almost there. Now you just need to poke those values into your settings file. We use .NET's standard configuration manager, so in a default project layout you'll want to add your settings to the 'app.config' file inside your project.

If you didn't have any other settings to configure, your app.config might look like this:

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <appSettings>
        <add key="socrata.host" value="http://opendata.socrata.com" />
        <add key="socrata.username" value="YOUR USERNAME HERE" />
        <add key="socrata.password" value="YOUR PASSWORD HERE" />
        <add key="socrata.app_token" value="YOUR APPLICATION TOKEN HERE" />
      </appSettings>
    </configuration>

Testing
=======

Out of the box, the tests will run with [NUnit][] 2.4.8. This is a slighly older release, but it's what comes with [Mono][] and thus allows us to run it on our continuous integration server :) To run under Visual Studio, you'll need to download a 2.4.x release and install it, then fix the reference in the Tests project if Visual Studio doesn't automatically pick up the assembly.

[nunit]: http://www.nunit.org/
[mono]: http://www.mono-project.com/

Issues / Patches / Pull Requests...
===================================

... are welcome! If you add a new feature, please add tests so we don't accidentally break it in future releases.
