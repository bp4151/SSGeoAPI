SSGeoAPI
========

Technologies

ServiceStack API
MongoDB
C# .NET
RestSharp
Everlive .NET SDK

The intentions of this project are:

to build an open-source API that allows developers to host/run their own geofencing server with the ability to edit functionality as they see fit.
to allow users of the API to manage their own places.
There are many services out there that offer geolocation and geofencing PaaS, but most don't allow the mobile client to manage location data at the app level. My specific needs were to have a global read/write layer where any mobile client could create their own places and have the system message other users who have entered a place. To date, this still isn't available out of the box, and the closest I have come is Geoloqi, a paid service that has announced their deprecation. Geoloqi has everything but the read/write global layer, which required me to write a proxy API that would act as the application owner and pass the requests between my mobile clients and Geoloqi. This allowed a client to create, update, and delete places and triggers, and send messages.

Basic Functionality

- Store per-user location data including latitude, longitude with option to store only most recent N records. To establish if a user is in a place, we really only need the most recent two reported locations.
- Store per-user place data, including latitude, longitude, and radius, where the owner/creator of the place has full CRUD capability.
- Establish if a user's current location is in a place, if they have just entered a place, or have just left a place. If a user has entered a place, add their identifier (email address, device token, etc.) to the place.usersInPlace list. If a user has left a place, remove their identifier (email address, device token, etc.) from the place.usersInPlace list
- Store per-place triggers that would perform an action if any user has entered or exited a place, such as send push, email, SMS etc. Trigger owners would have full CRUD capability on their trigger records.

Additional Functionality

Push notification is currently coded for Everlive and Appcelerator Cloud Services. Push notification uses the Repository Model so additional push services can easily be added. In addition, the desired push service can be called from an app.config AppSettings value.

Feel free to fork and take this for a test drive! I appreciate your comments and/or contributions. Also, the API is currently in use at http://geoapi.jaxmeier.org. You can test drive that as well using http://geoapi.jaxmeier.org/swagger-ui/index.html
