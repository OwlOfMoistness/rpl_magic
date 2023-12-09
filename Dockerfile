FROM mcr.microsoft.com/dotnet/core/runtime:3.1

COPY /rpl_magic/bin/Release/netcoreapp3.1/publish/ app/

ENTRYPOINT dotnet app/rpl_magic.dll "$TOKEN" "$PREFIX" "$RPC"
