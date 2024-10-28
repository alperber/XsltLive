## XsltLive
A command-line tool to transform XML files with XSLT templates, generating an HTML output served locally. Any changes to XML or XSLT files trigger an automatic reload, keeping your development flow responsive.

### Usage
```shell
./XsltLive -xml ./data.xml -xslt ./design.xslt
```

### Build
To build this project, you'll need the .NET 8 SDK.

For Windows users on an x86/64 CPU, use the following script:
```shell
git clone https://github.com/alperber/XsltLive.git
cd XsltLive
dotnet publish -r win-x64 -c Release -o ./build  /p:PublishSingleFile=true --self-contained true
```
If you are using a different runtime, replace the -r parameter with the appropriate runtime identifier. You can find the full [list of runtime identifiers on MSDN](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog).



### Options
| Option                            | Description                                         | Is Optional? |
|-----------------------------------|-----------------------------------------------------|--------------|
| **-xml** or **--xml-file-path**   | Xml file path, it can be relative or absolute       | No           |
| **-xslt** or **--xslt-file-path** | Xslt file path, it can be relative or absolute      | No           |
| **-hp** or **--http-port**        | Which port http server listen, default is 5000      | Yes          |
| **-wsp** or **--websocket-port**  | Which port websocket server listen, default is 5001 | Yes          |
