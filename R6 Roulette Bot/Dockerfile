# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Set the working directory
WORKDIR /app

# Clone the git repository
RUN git clone https://github.com/bourk94/R6RouletteDiscordBot.git .

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Create a directory for the List_R6_Roulette folder
RUN mkdir ./List_R6_Roulette

# Copy everything else and build the project
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Install dependencies
RUN apt-get update && apt-get install -y \
    libopus-dev \
    libsodium-dev

WORKDIR /app
COPY --from=build-env /app/out .

# Make sure the app binds to port 80
ENV ASPNETCORE_URLS=http://+:80

# Start the app
ENTRYPOINT ["dotnet", "R6 Roulette Bot.dll"]