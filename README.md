# GPSBabelWebAPI
This repository holds the key building blocks to create the GPSBabel web API based on docker.

It uses ASP .Net core to run the server to recieve HTTP calls. It uses apt-get to install gpsbabel executable globally. This project can't be used if gpsbabel in not installed globally - therefore it is advised to build the docker image and run it to get it working:

```
docker build -t gpsbabel-webapi .
docker run -p 11984:80 gpsbabel-webapi
```
Now surf to localhost:11984/swagger/ to get a simple UI to interact with gpsbabel

This is a very simple project aimed to make GPSBabel a micro service that does not run from command line. For that purpose this project uses Docker.

Please note that this implementation hold a security risk for parameters variable so this service should not be exposed publicly!
