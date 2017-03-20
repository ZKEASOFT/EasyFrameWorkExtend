   
# EasyFrameWorkExtend
EasyFrameWork的扩展
## EasyFrameWork.MySql
```sh
<connectionStrings>
 <add name="Easy" connectionString="Database='zkeacms';Data Source=localhost;User ID=root;Password=xxx;CharSet=utf8;" />
</connectionStrings>
<appSettings>
 <add key="DataBase" value="MySql" />
</appSettings>
```
[MySql](http://www.zkea.net/zkeacms/extend/detail/post-103)

## EasyFrameWork.SQLite
```sh
<connectionStrings>
 <add name="Easy" connectionString="App_Data\DataBase.sqlite3" />
</connectionStrings>
<appSettings>
 <add key="DataBase" value="SQLite" />
</appSettings>
```
[SQLite](http://www.zkea.net/zkeacms/extend/detail/post-68)

## EasyFrameWork.Storage.QCloud EasyFrameWork.Storage.QCloudBakcSource
```sh
<configSections>
 <section name="QCloudStorage" type="Easy.Storage.QCloud.Configuration,Easy.Storage.QCloud" />
 </configSections>
 <QCloudStorage bucketName="" appId="" secretId="" secretKey="" />
```
[QCloud](http://www.zkea.net/zkeacms/extend/detail/post-117)
