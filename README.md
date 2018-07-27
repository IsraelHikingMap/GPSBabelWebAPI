# GPSBabelWebAPI
This repository holds the key building blocks to create the GPSBabel web API.

It uses ASP .Net core to run the server to recieve HTTP calls and a NuGet package that has all the compiled binaries needed to run GPSBabel.

This is a very simple project aimed to make GPSBabel a micro service that does not run from command line. for that purpose this project uses Docker.

Please note that this implementation hold a security risk for parameters variable so this service should not be exposed publicly.
