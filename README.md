## **Reddit History Manager**
This is a quick project i through together in an hour to manage reddit comment history without the plauge of infinite scroll and the reddit ui. This project is not in active development.

#### Features
- Responsive UI preloading all retrievable comments as you review them
- Search for exact words or phrases
- Sort by date or upvotes
- Filter by specific subreddit
- Comments edited before deletion for privacy
- 1 Click delete without confirmation


![features](https://github.com/user-attachments/assets/ee8e114e-6378-4509-bcfc-571b73c68699)


#### Getting Started
Place your reddit api keys in appsettings.json compile and run.

![apiexample](https://github.com/user-attachments/assets/d77276fb-0436-48e3-ba7a-59541a87ee2a)


#### Limitations
 - Reddit has an arbitrary 1000 "thing" limit so the most comments you can pull is 4000. 1000 "New", 1000 "Hot" etc etc. Even after deleting these reddit will not repopulate this. All comments outside of those limits are not retrievable via the api and have to be done via a data download request.
