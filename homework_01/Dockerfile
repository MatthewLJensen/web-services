FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-image
WORKDIR /home/app
COPY ./*.sln ./
COPY ./*/*.csprof ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
RUN dotnet restore
COPY . .
RUN dotnet test ./Tests/Tests.csproj
RUN dotnet publish ./CompanyEmployees/CompanyEmployees.csproj -o /publish/
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /publish
COPY --from=build-image /publish .
ENV ASPNETCORE_URLS=https://+:5001;http://5000
ENV SECRET=CodeMazeSecretKey
ENTRYPOINT [ "dotnet", "CompanyEmployees.dll" ]

