# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the 'List_R6_Roulette' directory from your local machine to the Docker image
COPY ./bin/Debug/net6.0/List_R6_Roulette ./List_R6_Roulette

# Copy everything else and build the project
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/List_R6_Roulette ./List_R6_Roulette

# Make sure the app binds to port 80
ENV ASPNETCORE_URLS=http://+:80

# Start the app
ENTRYPOINT ["dotnet", "R6 Roulette Bot.dll"]