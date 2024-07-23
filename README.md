# 将 SQLite 数据库提供程序与 EF Core 结合使用

1. 安装 Entity Framework Core 工具
   首先，确保你已安装了 Entity Framework Core 工具。你可以通过 dotnet CLI 安装这些工具。

dotnet tool install --global dotnet-ef

2. 安装 Entity Framework Core 设计包
   确保你的项目中安装了 Entity Framework Core 设计包。这可以通过 NuGet 包管理器或者 Package Manager Console 完成。

在 Package Manager Console 中运行以下命令：

Install-Package Microsoft.EntityFrameworkCore.Design

3. 运行迁移命令
   在你的项目文件夹中打开命令行，然后运行以下命令来添加迁移：

dotnet ef migrations add InitialCreate
EF Core 将在项目目录中创建“Migrations”文件夹，该文件夹包含两个文件，其中包含表示数据库迁移的代码。

```
dotnet ef database update
```

你应在项目目录中看到新创建的 Pizzas.db 文件。

# 使用工具

下载：https://sqlitestudio.pl/

使用 SQLiteStudio(3.4.3) 连接加密数据库

1、选择数据库类型 SQLCipher

2、文件：先择相应的数据库文件

3、密码：填入 123456 （上面程序设置的密码）

4、加密算法配置（SQCipher 4）：

PRAGMA kdf_iter = '256000';

PRAGMA cipher_page_size = 4096;

PRAGMA cipher_hmac_algorithm = HMAC_SHA512;

PRAGMA cipher_default_kdf_algorithm = PBKDF2_HMAC_SHA512;

```C#
        /// <summary>
        /// 初始化 并加密数据库文件
        /// </summary>
        private void initDb()
        {
            var sqliteConnectionString = new SqliteConnectionStringBuilder("Filename=t1.db")
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                Password = "123456",
                //DataSource = "resource/t1.db", // 数据库文件路径
            }.ToString();

            using (var connection = new SqliteConnection(sqliteConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT quote($newPassword);";
                command.Parameters.AddWithValue("$newPassword", "123456");
                var quotedNewPassword = (string)command.ExecuteScalar();

                command.CommandText = "PRAGMA rekey = " + quotedNewPassword;
                command.Parameters.Clear();
                command.ExecuteNonQuery();
            }
        }
```

    <ItemGroup>
    	<None Update="evaluate.db">
    		<CopyToOutputDirectory>Always</CopyToOutputDirectory>
    	</None>
    </ItemGroup>
