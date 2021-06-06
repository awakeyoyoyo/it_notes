### Item05:Prefer dependency injection to hardwiring resources 优先考虑依赖注入来引用资源

在实际项目中，有许多类会依赖一个或者多个底层的资源。例子：拼写检查器需要依赖词典，因此有几种可以将其实现的方法。

#### 1、将类实现为静态工具类

```java
public class SpellChecker {
    private static final Lexicon dictionary=...;
    private SpellChecker(){
    }
    public boolean isValid(String word){
        //...
        return true;
    }
    //....
}
```

缺点：如果有多部辞典需要用的时候，需要创建多个拼写检查器修改其成员变量

#### 2、将类实现为单例类

```java
public class SpellChecker {
    private final Lexicon dictionary=...;
    private SpellChecker(...){
      ...
    }
  	public static INSTANCE=new SpellChecker(...);
    public boolean isValid(String word){
        //...
        return true;
    }
    //....
}
```

缺点：如果有多部辞典需要用的时候，需要创建多个拼写检查器修改其INSTANCE

#### 3、通过依赖注入，不同的依赖生成不同的操作对象

代码：

```java
public class SpellChecker {
    private final Lexicon dictionary;
    public SpellChecker(Lexicon dictionary){
        this.dictionary= Objects.requireNonNull(dictionary);
    }
    public boolean isValid(String word){
        //...
        return true;
    }
    //....
}
```

比较常用的代码结构吧算是。

总结：

1、静态工具类和singleton类不适合引用底层的资源类

2、不要直接在成员变量处创建资源，而是通过依赖注入

### Item06:Avoid creating unnecessary objects 避免创建不必要的对象

极端的反面例子：

String s=new String("lqh666***");

该语句每次执行的时候都会创建一个新的string实例。但这些创建对象的动作没有必要，传递给String构造器的参数“lqh666***”本来就是一个String实例，功能等于string构造器创建的对象。

改进后：

String s="lqh666***";

这个版本只用了一个string实例，并且不是每次执行的时候都创建一个新的实例，在用一台虚拟机中运行的代码，只要他们包含相同的字符串字面敞亮，该对象就会被重用。

**要点：**

1、当一个方法中需要频繁地创建新对象，需要思考这个对象是否可以缓存。

例子：

```java
public class RomanUtil {
    private static final Pattern ROMAN=Pattern.compile("[0-9]*");

    //每次调用都创建新的Pattern对象
    public static boolean isRomanNumeral(String s){
        return s.matches("[0-9]*");
    }

    //由于ROMAN的正则表达式不会变，所以这里缓存起来，每次都使用同一个Pattern
    public static boolean isRomanNumeral0(String s){
        return ROMAN.matcher(s).matches();
    }
}
```

2、当一个对象是不变的，那么他就基本可以重用

例如：

- 适配器模式下，适配器除了委托的对象之外，没有其他状态信息，所以适配器模式中部分的适配器是不需要创建多个适配器实例的
- Map接口的keySet()方法返回Map的Key视图，要保证每次返回的keySet功能都是相等的，且与其他返回的视图要保持更新，**由内部的迭代器完成**，**不是每个keySet都是独一**，所以这里重复创建并没有意义。

3、自动装箱使得基本类型和装箱类型之间的差别变得模糊，但没有完全消除，所以使用装箱类型会额外的创建多余的对象。

什么是自动装箱拆箱？

```java
//自动装箱
Integer total = 99;
//自定拆箱
int totalprim = total;
```

简单一点说，装箱就是自动将基本数据类型转换为包装器类型；拆箱就是自动将包装器类型转换为基本数据类型。

再细说的话请移步：https://blog.csdn.net/liuchaoxuan/article/details/80788060

这里给出结论：使用基本类型会比使用装箱类型更快。

**当 “==”运算符的两个操作数都是 包装器类型的引用，则是比较指向的是否是同一个对象，而如果其中有一个操作数是表达式（即包含算术运算）则比较的是数值（即会触发自动拆箱的过程）。**

例如：

```java
Long sum=0L;
for(int i=0;i<10;i++){
  sum+=i;
}
```

上述代码使用的装箱类型，每次sum+=i的时候都会构建一个Long实例，即生成了多余的对象，我们使用long基本类型就可以完美解决，优化性能。

