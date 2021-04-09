**Item01:Consider static factory instead of constructors 考虑用静态工厂方法代替构造器**

优点：

**1、静态工厂方法与构造器不一样的地方是：有方法名**

**2、不必在每次调用他们的时候都创建一个新的对象，可以根据情况选择创建新对象还是返回旧对象或者返回一个不可变的对象**

**何为不可变对象？**

不可变类 是指创建该类的实例后，该实例的实例变量是不可改变的。Java中已有类例如Double和String等。
如果需要创建自定义的不可变类，遵守如下规则：

- 使用private和final修饰符来修饰该类的成员变量；
- 提供带参数的构造器，根据传入的参数来初始化类里的成员变量；
- 仅为该类的成员变量提供getter方法，不要提供setter方法，因为普通方法无法修改final修饰的成员变量；
- 如果有必要，重写Object类的hascode()和equals()方法。equals方法根据关键成员变量来作为两个对象是否相等的标准，除此之外，还应该保证两个用equals方法判断为相等的对象的hashCode()也相等。
  

**3、可以返回原返回类型的任何子类对象**

如Collections类下的unmodifiableList方法，返回了List 的子类UnmodifiableList或者UnmodifiableRandomAccessList

```java
public static <T> List<T> unmodifiableList(List<? extends T> list) {
    return (list instanceof RandomAccess ?
            new UnmodifiableRandomAccessList<>(list) :
            new UnmodifiableList<>(list));
}
```

**4、每次返回的对象根据传入的参数而不同**

**5、方法返回的对象所属的类，在编写包含该静态工厂方法时可以不存在。这种优势构成了服务提供者框架。**

这里书本举的例子是JDBC API.  服务器提供者框架：服务接口（service interface）、提供者注册api（provider registration api）、服务访问api（service access api）

 服务器提供者框架示例： 

```java
//服务接口
public interface Service{
  ...//some methods 
}
//服务提供者接口
public interface Provider{
  Service newService();
}

public class Services{
  private Services(){}
  private static final Map<String,Provider> providers=new ConcurrentHashMap<>();
  public static final String DEFAULT_PROVIDER_NAME="<def>"
  //提供者注册API
  public static void registerDefaultProvider(Provider p){
    registerProvider(DEFAULT_PROVIDER_NAME,p);
  }

 public static void registerProvider(String name,Provider p){
    providers.put(name,p);
  }
  
  //服务访问API
 public static Service newInstance(){
    return newInstance(DEFAULT_PROVIDER_NAME);
  }

  public static Service newInstance(String name){
    Provider p=providers.get(name);
    if(p==null){
      throw new IllegalArgumentException("No provider registered with name: "+name);
    return p.newService();
  }  
}
}
```



服务接口（service interface）：提供者实现。例子：JDBC中的Connection接口

提供者注册api（provider registration api）：提供者用来注册实现的 例子：JDBC中用来注册driver的api。  java.sql.DriverManager.registerDriver(new Driver());

服务访问api（service access api）：这是客户端用来获取服务的实例，服务器访问api是客户端用来指定某种选择实现的条件。  例子：JDBC中getConnection() 根据规则选择具体的Driver的实现类

简单的书写一下jdbc版本的例子(自己手写非源码，提取了重要的点)：

##### 服务接口：Connection （由厂商进行实现）

```java
public class Connection {
    private String DbName;

    public Connection() {
    }

    public Connection(String dbName) {
        DbName = dbName;
    }

    public String getDbName() {
        return DbName;
    }

    public void printf(){
        System.out.println("This is "+getDbName()+" Connection");
    }
}
```

具体的connection由driver类的getConnection()方法提供。

```java
public interface JdbcDriver {
    Connection getConnection();
}
/**
由数据库厂商实现。
*/
class MysqlDriver implements JdbcDriver{
        @Override
        public Connection getConnection() {
            return new Connection("MysqlServer");
        }
    }

    class SqlServerDriver implements JdbcDriver{
        @Override
        public Connection getConnection() {
            return new Connection("SqlServer");
        }
    }
```

##### 提供者访问api&&提供者注册api

```java
public class DriverManager {
    private DriverManager(){}

    private static final Map<String, JdbcDriver> providers=new ConcurrentHashMap<>();

    public static final String DEFAULT_PROVIDER_NAME="<def>";

    //提供者注册API
    public static void registerDriver(String name,JdbcDriver driver){
        providers.put(name,driver);
    }

    //服务访问API
    public static Connection getConnection(String name){
        JdbcDriver driver=providers.get(name);
        if (driver==null) {
            throw new IllegalArgumentException("No provider registered with name: " + name);
        }
        return driver.getConnection();
    }

}
```

##### 最终调用

```java
public class Main {
    public static void main(String[] args) {
        DriverManager.registerDriver("Sql",new AllDriver.SqlServerDriver());
        DriverManager.registerDriver("Mysql",new AllDriver.MysqlDriver());
        DriverManager.getConnection("Sql").printf();
        DriverManager.getConnection("Mysql").printf();
    }
}
```

JDBC就是定义了一个数据库的访问框架，而实际实现的提供方有很多，这里具体的实现情况就是Connection是制定的一套实现接口，也就是客户最关心的，也就是服务接口；而服务方也关心，并且必须提供实现connection接口的方法，同时必须关系如何接入，DriverManager就是用来注册服务方的。它连接了客户和提供方，客户通过调用getConnection来获取自己需要的提供方的实现，但不关心具体由谁来实现。实际的实现是 Driver的connect方法来返回的。


**Item02 Consider a builder when faced with many constructor parameters 在遇到多个构造器参数的时候考虑使用建造者模式。**

简单来说：当一个对象的成员变量太多的时候，而其中又有些成员变量是可选的，那么正常情况下我们就需要根据这些可选的成员变量定义不同的构造方法，但如果成员变量太多则会需要开发者去手动的查看每个构造方法的参数。这是不好的。

改进：

1、JavaBeans模式，先把对象构建出来，然后再用setter方法进行设置。 缺点：会存在线程问题，当对象创建出来未设置好成员变量的属性就被其他线程调用。需要额外进行确保线程安全。

2、重叠构造器模式，即根据每个可选择的参数，定义不同的构造方法。缺点：繁琐，构造方法太多且难以管理。

3、 建造者模式， 他不直接生成想要的对象，而是让客户端利用含有必要的参数调用构造器得到一个builder对象，然后再根据builder对象的方法来设置每个选择的成员变量。

建造者模式例子01:简单版

```java
public class NutritionFacts {
    private final int servingSize;
    private final int servings;
    private final int fat;
    private final int calories;
    private final int sodium;
    private final int carbohydrate;

    public static class Builder{
        /**
         * 必要属性
         */
        private final int servingSize;
        private final int servings;
        /**
         * 选填属性
         */
        private  int fat=0;
        private  int calories=0;
        private  int sodium=0;
        private  int carbohydrate=0;

        public Builder(int servingSize, int servings) {
            this.servingSize = servingSize;
            this.servings = servings;
        }
        public Builder fat(int val){
            fat=val;
            return this;
        }
        public Builder calories(int val){
            calories=val;
            return this;
        }
        public Builder sodium(int val){
            sodium=val;
            return this;
        }
        public Builder carbohydrate(int val){
            carbohydrate=val;
            return this;
        }

        public NutritionFacts build(){
            return new NutritionFacts(this);
        }
    }

    public NutritionFacts(Builder builder) {
        this.servingSize = builder.servingSize;
        this.servings = builder.servings;
        this.fat = builder.fat;
        this.calories = builder.calories;
        this.sodium = builder.sodium;
        this.carbohydrate = builder.carbohydrate;
    }

    @Override
    public String toString() {
        return "NutritionFacts{" +
                "servingSize=" + servingSize +
                ", servings=" + servings +
                ", fat=" + fat +
                ", calories=" + calories +
                ", sodium=" + sodium +
                ", carbohydrate=" + carbohydrate +
                '}';
    }

    public static void main(String[] args) {
        NutritionFacts nutritionFacts=new NutritionFacts.Builder(1,2).fat(12).build();
        System.out.print(nutritionFacts);
    }
}
```

NutritionFacts类中有很多的成员变量，必须的是servingSize和servings两个成员变量。其他都是可选的，这时候通过一个内部类Builder进行巧妙的解决。

核心的部分：

1、Builder的构造方法---需要传入必须的成员变量，

2、Builder类的fat、calories....等方法可以对选择的成员变量进行赋值

3、Builder类中的可选成员变量都有初始化的值

4、NutritionFacts的构造方法变为传入Builder对象，然后根据Builder对象的成员变量进行初始化。

5、Builder.build()方法内部调用的是NutritionFacts的构造方法，把自身传入。

最终的调用：

```java
public static void main(String[] args) {
    NutritionFacts nutritionFacts=new NutritionFacts.Builder(1,2).fat(12).build();
    System.out.print(nutritionFacts);
}
```

先利用Builder的构造方法传入必须参数，在流式编程传入可选参数，最后build()返回NutritionFacts对象。

建造者模式例子02:升级版（提取出一个抽象类以及抽象builder，子类可以继承抽象类，继承抽象builder类编写属于自己的规则）

以pizza为例子

Pizza抽象类：

```java
public abstract class Pizza {
    /**
     * 配料：
     * 火腿，蘑菇，洋葱，胡椒，香肠
     */
    public enum Topping {HAM,MUSHROOM,ONION,PEPPER,SAUSAGE}

    /**
     *  配料
     */
    final Set<Topping> toppings;

    /**
     * 抽象Builder
     */
    abstract static class Builder<T extends Builder<T>> {
        // 用于创建具有指定元素类型的空EnumSet。
        EnumSet<Topping> toppings=EnumSet.noneOf(Topping.class);
        public T addTopping(Topping topping){
            toppings.add(Objects.requireNonNull(topping));
            return self();
        }
        /**
         * 子类通过重写该类来返回对应的子类类型类型
         * @return
         */
        abstract Pizza build();
        /**
         * 子类必须重写该方法并且return this
         * @return
         */
        protected abstract T self();
    }

    Pizza(Builder<?> builder){
        toppings=builder.toppings.clone(); //See Item 50
    }
}
```

大致上跟01版本相差无几，利用范式T，巧妙的build出对应的子类。其中将一些公用的属性抽出到抽象类中进行实现，减少代码的冗余。

两个具体pizza的实现类：

NyPizza:

```java
public class NyPizza extends Pizza {
    public enum Size {SMALL, MEDIUM, LARGE}
    ;
    private final Size size;

    public static class Builder extends Pizza.Builder<Builder>{
        private final Size size;
        public Builder(Size size){
            this.size= Objects.requireNonNull(size);
        }

        @Override
        NyPizza build() {
            return new NyPizza(this);
        }

        @Override
        protected Builder self() {
            return this;
        }
    }

    private NyPizza(Builder builder) {
        super(builder);
        size=builder.size;
    }

    @Override
    public String toString() {
        return "NyPizza{" +
                "size=" + size +
                ", toppings=" + toppings +
                '}';
    }
}
```

Calzone:

```java
public class Calzone extends Pizza {
    /**
     * 是否加酱汁
     */
    private final boolean sauceInside;

    public static class Builder extends Pizza.Builder<Builder>{
        /**
         * 是否加酱汁
         */
        private boolean sauceInside=false;
        public Builder sauceInside(){
            this.sauceInside= true;
            return this;
        }

        @Override
        Calzone build() {
            return new Calzone(this);
        }

        @Override
        protected Builder self() {
            return this;
        }
    }

    private Calzone(Builder builder) {
        super(builder);
        sauceInside=builder.sauceInside;
    }

    @Override
    public String toString() {
        return "Calzone{" +
                "sauceInside=" + sauceInside +
                ", toppings=" + toppings +
                '}';
    }
}
```

可以看出这两个具体pizza的实现类都有不同的成员变量，所以其Builder的实现都会有所不同。