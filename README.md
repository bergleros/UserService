# UserService
A simple REST service that allows users to get a userid based on a secret, and gives administrators access to user data.

## Project structure
The UserService project contains the controllers and logic, and UserServiceTests contains end-to-end tests for controllers and unit tests for the logic class. 
There are two controllers, one for user endpoints and another for admin endpoints.

## Running
The solution can be run from within VisualStudio either as an IIS application or in a Docker container.
The Docker image is also available at docker.io as berglindros/userservice. 
To get the image and start a container:
```
docker pull berglindros/userservice
docker run --publish 80:80 berglindros/userservice
```
When the service is running, use e.g. Postman to send requests to the endpoints listed below.

## Endpoints
* GET /user.api/v1/user?secret={secret} - Get userid from a provided secret. If no user exists for the secret, it will be created and the new userid returned.
* GET /admin.api/v1/users - Get all existing users
* GET /admin.api/v1/user/{id} - Get a user by userid

## Todo
* Tests for admin endpoints and corresponding logic methods are missing
* Swagger UI isn't available when running Docker container from image
* Various considerations should be addressed for a production version, see comments in code 
