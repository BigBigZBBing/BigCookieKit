# BigCookieKit
超级工具箱
所有的经验积累库 包含的强大功能

##### BigCookieKit.Reflect 高性能反射库

[BigCookieKit.Reflect 高性能反射库](#BigCookieKit.Reflect)

替代表达式树 作为更高性能的解决方案

##### BigCookieKit.Communication 高性能通信库

使用SEAE异步方式以及极简化结构实现通信

##### BigCookieKit.Office 高性能读写Excel

使用OpenXML方式读写Excel数据的方案 性能是Npoi的3倍以上

##### SequelizeStructured 结构化查询结果

仿造SequelizeORM的结构化结果方案

##### BigCookieKit 工具箱

开发中的常用套件 工具件


# BigCookieKit.Reflect

### BigCookieKit.Reflect反射库使用手册

##### 开始使用 创建一个动态函数以委托为入口
```csharp
var action = SmartBuilder.DynamicMethod<Action>(string.Empty, emit =>
{
    var lb = emit.ArgumentRef(index,typeof(string)); //获取传入的参数 index是传入参数的索引
    emit.Argument(index); //直接抛出传入的参数 index是索引
    emit.Return(); //不能漏
});
action.Invoke();
```

##### 基础类型
```csharp
//初始化string
var to_string = emit.String("字符串");
//初始化int
var to_int = emit.Int32(int.MaxValue);
//初始化long
var to_long = emit.Int64(long.MaxValue);
//初始化datetime
var to_datetime = emit.DateTime(DateTime.Now);
//初始化float
var to_float = emit.Float(float.MaxValue);
//初始化double
var to_double = emit.Double(double.MaxValue);
//初始化decimal
var to_decimal = emit.Decimal(decimal.MaxValue);
//初始化bool
var to_bool = emit.Boolean(true);
//初始化array
var to_array = emit.Array<string>(10);
//初始化model
var to_model = emit.Entity<TestModel>();
//初始化list
var to_list = emit.List<TestModel>();
//初始化可空类型
var to_nullable_int = emit.Int32(int.MaxValue).AsNullable();
//初始化object
var to_object = emit.Object(to_model);
```

##### 特殊初始化
```csharp
//初始化特殊构造的
var to_init = emit.Initialize(typeof(List<string>),emit.Int32(50));
//再丢给Object来驱动
var to_init_use = emit.Object(to_init);
```

##### 万能基类
```csharp
//随便初始化一个
var to_string = emit.String("字符串");
//初始化object
var to_object = emit.Object(to_string);
//强制转换 非as type
var to_object = to_object.AS(typeof(string));
//获取字段的值
var lb = to_object.GetField("fieldName");
//获取属性的值
var lb = to_object.GetPropterty("propName");
//给字段赋值
to_object.SetField("fieldName",value);
//给属性赋值
to_object.SetPropterty("propName",value);
//调用函数
to_object.Call("methodName",new object[]{ value });
```

##### 函数使用方法
```csharp
//调用静态函数
var method = il.ReflectStaticMethod("methodName",returnType,new object[]{ value });
//调用非静态函数
to_object.ReflectMethod("methodName",returnType,new object[]{ value });
//返回函数结果 如果不返回则被推出计算堆
var lb = method.ReturnRef();
```

##### 可计算类型
支持 Byte Int16 Int32 Single Double Decimal String
```csharp
var to_int = emit.Int32(100);
var to_int1 = emit.Int32(200);
var to_int2 = to_int + to_int1;
to_int2 = to_int + 200;

var to_long = emit.Int64(64234);
var to_int3 = to_int2 / to_long;

var to_float = emit.Float(5.3f);
var to_float1 = to_int3 * to_float;

var to_string = emit.String("测试数据");
var to_string1 = emit.String("s1231");
to_string = to_string + to_string1;
to_string = to_string + "s1231";
```

##### IF判断
```csharp
emit.IF(emit.Boolean(true),()=>{
	//TODO
}).ElseIF(emit.Boolean(true),()=>{
	//TODO
}).Else(()=>{
	//TODO
}).IFEnd();
```

##### For 正循环 逆循环
```csharp
emit.For(emit.Int32(),emit.Int32(100),(index,tab)=>{
	//index 循环的索引
	tab.Break();//跳出循环使用
});

emit.Forr(emit.Int32(100),emit.Int32(),(index,tab)=>{
	//index 循环的索引
	tab.Break();//跳出循环使用
});
```

##### While 循环
```csharp
emit.While(emit.Boolean(true),(tab)=>{
	tab.Break();//跳出循环使用
});
```

##### Try 异常捕获
```csharp
emit.Try(()=>{
	//TODO
}).Catch((ex)=>{
	//TODO
	emit.Throw(ex);
}).Finally(()=>{
	//TODO
}).TryEnd();
```

## BigCookieKit.Office(xlsx)使用手册
##### 读取Excel导出DataTable
```csharp
string path = Path.Combine(resource, "test.xlsx");
ReadExcelKit excelKit = new ReadExcelKit(path);
excelKit.AddConfig(config =>
{
    config.SheetIndex = 1; // 工作簿位置 由1开始
    config.ColumnNameRow = 1; // 列头行 必须有
    config.StartRow = 2; // 开始行 对应Excel
    //config.StartColumn = "C"; //开始列 对应Excel
    //config.EndColumn = "D"; //结束列 对应Excel
    //config.EndRow = 6; // 结束行 对应Excel
    //config.ColumnSetting = new[] { //自定义列配置 可以自由的选择读某些列
    //    new ColumnConfig(){ ColumnName="Id", ColumnType=typeof(int), NormalType= ColumnNormal.Increment },
    //    new ColumnConfig(){ ColumnName="UniqueNo", ColumnType=typeof(Guid), NormalType= ColumnNormal.Guid },
    //    new ColumnConfig(){ ColumnName="CreateTime", ColumnType=typeof(DateTime), NormalType= ColumnNormal.NowDate },
    //    new ColumnConfig(){ ColumnName="测试1", ColumnType=typeof(string), Column="A" },
    //    new ColumnConfig(){ ColumnName="测试2", ColumnType=typeof(string), Column="B" },
    //};
});
DataTable dt = excelKit.XmlReadDataTable();
```

##### 读取Excel导出Dictionary
```csharp
string path = Path.Combine(resource, "test.xlsx");
ReadExcelKit excelKit = new ReadExcelKit(path);
excelKit.AddConfig(config =>
{
    config.SheetIndex = 1;
    config.ColumnNameRow = 1;
    config.StartRow = 2;
    //config.EndRow = 100;
    //config.StartColumn = "C";
    //config.EndColumn = "D";
});
var dics = excelKit.XmlReaderDictionary();
```

##### 读取Excel导出IEnumerable<object[]>
```csharp
string path = Path.Combine(resource, "test.xlsx");
ReadExcelKit excelKit = new ReadExcelKit(path);
excelKit.AddConfig(config =>
{
    config.SheetIndex = 1;
    config.StartRow = 1;
    //config.EndRow = 4;
    //config.StartColumn = "C";
    //config.EndColumn = "D";
});
var rows = excelKit.XmlReaderSet();
```

## BigCookieKit.Network网络库使用手册
##### 客户端
```csharp
class Program
{
    static void Main(string[] args)
    {
        NetworkClient client = new NetworkClient("127.0.0.1", 7447);
        client.Mode = ApplyMode.Client;
        client.OnConnect = user =>
        {
            Random random = new Random();
            while (true)
            {
                //完全可以接收无间隔发送
                //user.SendMessage(RamdomString());
                string str = Console.ReadLine();
                switch (str)
                {
                    case "close":
                        user.Disconnect();
                        break;
                    default:
                        user.SendMessage(str);
                        break;
                }
            }
        };

        client.OnCallBack = (user, packet) =>
        {
            Console.WriteLine(Encoding.UTF8.GetString(packet));
        };
        //不用处理器
        client.Handle = new NoneHandle();
        //简单粘包处理器
        client.Handle = new EasyHandle();
        //正规的TCP粘包处理器(默认)
        client.Handle = new TcpHandle();

        client.Start();

        Thread.Sleep(-1);
    }

    public static byte[] RamdomString(int length = 1024)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new Random();
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length - 1)];
        }
        return System.Text.Encoding.UTF8.GetBytes(stringChars);
    }
}
```

##### 服务端
```csharp
class Program
{
    static void Main(string[] args)
    {
        NetworkServer tcpServer = new NetworkServer(7447);
        tcpServer.Protocol = NetworkProtocol.Http1;
        tcpServer.BufferSize = 4096;
        tcpServer.OnConnect = user =>
        {
            Console.WriteLine($"{user.UserHost}:{user.UserPort}接入~");
        };

        tcpServer.OnReceive = (user, packet) =>
        {
            string res = Encoding.UTF8.GetString(packet);
            Console.WriteLine($"[{user.UserHost}:{user.UserPort}]:{res}");
        };

        tcpServer.OnExit = user =>
        {
            Console.WriteLine($"{user.UserHost}:{user.UserPort}离开~");
        };

        //不用处理器
        client.Handle = new NoneHandle();
        //简单粘包处理器
        client.Handle = new EasyHandle();
        //正规的TCP粘包处理器(默认)
        client.Handle = new TcpHandle();
        //添加管道
        tcpServer.Handle.AddPipe<TestPipe>();

        tcpServer.Start();

        Thread.Sleep(-1);
    }
}

//IPipe是管道模型
class TestPipe : IPipe
{
    public async Task InvokeAsync(Session session, Action context)
    {
        Console.WriteLine("接收前1");
        context?.Invoke();
        Console.WriteLine("接收后1");
    }
}
```
