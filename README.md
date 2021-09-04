# BigCookieKit
超级工具箱
所有的经验积累库 包含的强大功能

##### BigCookieKit.Reflect 高性能反射库

替代表达式树 作为更高性能的解决方案

##### BigCookieKit.Communication 高性能通信库

使用SEAE异步方式以及极简化结构实现通信

##### BigCookieKit.Office 高性能读写Excel

使用OpenXML方式读写Excel数据的方案 性能是Npoi的3倍以上

##### SequelizeStructured 结构化查询结果

仿造SequelizeORM的结构化结果方案

##### BigCookieKit 工具箱

开发中的常用套件 工具件




## BigCookieKit.Reflect反射库使用手册

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