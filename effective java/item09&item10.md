### item09 try-with-resources 优先于 try-finally

> 尽量使用try-with-resources方式来关闭资源，普通的写法会利用多个try-catch 来保证资源的释放，这样写及不雅观，如果是底层出异常，那么try里面的代码抛异常，且finally里面的代码也抛异常，抛出第二个异常，第二个异常会完全的抹除第一个异常的信息。
>
> 而使用try-with-resources，会只保留第一个异常。

例子：

```java
/** 普通写法**/
InputStream in =new FileInputStream("inputFile");
try {
    OutputStream outputStream = new FileOutputStream("output");
    byte[] buf = new byte[255];
    int n;
    try {
        while ((n = in.read(buf)) >= 0) {
            outputStream.write(buf, 0, n);
        }
    }finally {
        outputStream.close();
    }
}finally {
    in.close();
}
/** 优雅写法**/
try(InputStream inputStream=new FileInputStream("inputFile");
    OutputStream out=new FileOutputStream("output");){
    byte[] buf = new byte[255];
    int n;
    while ((n = inputStream.read(buf)) >= 0) {
        out.write(buf, 0, n);
    }
}
```

### item10 覆盖equals时候必须遵守的约定

```java
public boolean equals(Object obj) {
    return (this == obj);
}
```

> Object中的原生equals()方法只保证每个对象只与它自己相等。

- 自反性 对象必须等于其自身
- 对称性 a.equals(b)==true b.equals(a)必须也是true，常见于父类 子类
- 传递性 a.equals(b)==true b.equals(c)==true 那么a.equals(c)也必须为true
- 一致性 相等的对象永远相等，不相等的对象永远不相等。针对不可变对象。
- 对于任何非null的引用值x，x.equals(null)必须返回false

#### 重写equals方法

- 使用==操作符检测是否为对象的引用
- 使用instanceof检测参数类型是否是正确的类型
- 把参数转换为正确的类型
- 检查类中的每一个关键成员变量是否相等。

例子：

```java
public class PhoneNumber {
    private final short areaCode,prefix,lineNum;

    public PhoneNumber(short areaCode, short prefix, short lineNum) {
        this.areaCode = areaCode;
        this.prefix = prefix;
        this.lineNum = lineNum;
    }
	
    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        PhoneNumber that = (PhoneNumber) o;
        return areaCode == that.areaCode &&
                prefix == that.prefix &&
                lineNum == that.lineNum;
    }

    @Override
    public int hashCode() {
        return Objects.hash(areaCode, prefix, lineNum);
    }
}
```

