#escape=`
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR .
COPY *.csproj .
RUN dotnet restore
COPY . .
#ENV configuration Debug
RUN dotnet build -c Debug

 FROM build AS publish
RUN dotnet publish --no-build -c Debug -o /app 

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
USER ContainerAdministrator
RUN reg add hklm\system\currentcontrolset\services\cexecsvc /v ProcessShutdownTimeoutSeconds /t REG_DWORD /d 7200  
RUN reg add hklm\system\currentcontrolset\control /v WaitToKillServiceTimeout /t REG_SZ /d 7200000 /f

WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dockergraceful.exe"]