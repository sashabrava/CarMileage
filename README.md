# CarMileage
ASP.NET Core MVC project for calculating car mileage statistics

<img src="https://user-images.githubusercontent.com/11160215/74154968-f9f69d80-4c13-11ea-9a32-a94bcc92d6d2.png" alt="index" width="70%" />

Usage notes:
- User must be logged in to add the car and its mileage.
- User can add their car mileage manually or by uploading GPX tracks. Algorithm calculates track distance and adds it to car's mileage at that day. Same file can't be uploaded multiple times for 1 car.
- Admin can change the owner of car and view/edit cars of all users, change mileage of any car.
- Admin can change User-owner of car.

<img src="https://user-images.githubusercontent.com/11160215/74155613-2d85f780-4c15-11ea-9095-c82d73c9a571.png" alt="edit_car" width="70%" />

Calculating mileage statistics works the following way:
1. Algorithm finds two User-specified odometer counters.
2. Searches for all User-specified daily mileages between those counters.
3. Calculates average daily mileage (excluding User-specified daily mileages).
4. Calculates predicted odometer counters the day before and the day after each User-specified daily mileage.
<img src="https://user-images.githubusercontent.com/11160215/74161366-1b10bb80-4c1f-11ea-94f2-66277ed93758.png" alt="stats" width="70%" />
