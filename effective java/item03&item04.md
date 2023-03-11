### ååItem03:Enforce the singleton property with a private constructor or an enum type 使用私有构造器或者美剧类强化单例模式

实现单例模式主要由两种方式：这两种方式都要保持构造器为私有的，并且导出共有的静态成员，以便于客户端能够访问该类的唯一实例。

#### 1、用public、final修饰静态成员变量

代码：

```java
public class SingletonWithPublicFinalField {
    public static final SingletonWithPublicFinalField INSTANCE=new SingletonWithPublicFinalField();

    private SingletonWithPublicFinalField(){
//        if (INSTANCE!=null){
//            throw new RuntimeException("Singleton instance already exists!!!");
//        }
    }

    public void leaveTheBuilding(){
        System.out.println("SingletonWithPublicFinalField.leaveTheBuilding() called...");
    }
}
```

通过私有化构造方法，构造方法仅被调用一次由此来保证INSTANCE的全局唯一性。一旦INSTANCE实例化则只会存在一个。

Main代码：

```java
  public static void main(String[] args) throws ClassNotFoundException, IllegalAccessException, InstantiationException {
        SingletonWithPublicFinalField instance01=SingletonWithPublicFinalField.INSTANCE;
        System.out.println(instance01);
    		//直接调用私有的构造方法会抛出can not access a member of class com.effectiveJava.item03.SingletonWithPublicFinalField with modifiers "private" 异常
				Object instance03=Class.forName(SingletonWithPublicFinalField.class.getName()).newInstance();
        System.out.println(instance03);
        //设置调用SingletonWithPublicFinalField的构造方法的时候不进行权限的检测 即无视private
        Constructor.setAccessible(SingletonWithPublicFinalField.class.getConstructors(),true);
        Object instance02=Class.forName(SingletonWithPublicFinalField.class.getName()).newInstance();
        System.out.println(instance02);
    }
```

需要注意客户端借助AccessibleObjcet.setAccessible()方法设置权限的检验，再利用反射调用私有构造器，我们可以添加下列代码，通过抛出Runtime异常的方式来阻止。

```java
        if (INSTANCE!=null){
            throw new RuntimeException("Singleton instance already exists!!!");
        }
```

由此保证了单例。

#### 2、用private、final修饰静态成员变量，提供方法进行访问

**代码：**

```java
public class SingletonWithPublicStaticFactoryMethod {
    private static final SingletonWithPublicStaticFactoryMethod INSTANCE=new SingletonWithPublicStaticFactoryMethod();

    public SingletonWithPublicStaticFactoryMethod() {
    }

    public static SingletonWithPublicStaticFactoryMethod getInstance(){
        return INSTANCE;
    }

    public void leaveTheBuilding(){
        System.out.println("SingletonWithPublicStaticFactoryMethod.leaveTheBuilding() called...");
    }

}
```

Main代码：

```java
  public static void main(String[] args){
        SingletonWithPublicStaticFactoryMethod instance01=SingletonWithPublicStaticFactoryMethod.getInstance();
        System.out.println(instance01);

    }
```

如果有学习过单例模式这个一眼可以看到底不需要解释啦哈哈哈哈哈。

##### 1 2-----（番外）序列化

通过添加readResolve()方法使得序列化以及读取序列化对象相同

一个类实现了 Serializable接口, 我们就可以把它往内存地写再从内存里读出而"组装"成一个跟原来一模一样的对象. 不过当序列化遇到单例时,就有了问题: 从内存读出而组装的对象破坏了单例的规则. 单例是要求**一个JVM中只有一个类对象的**, 而现在通过反序列化,**一个新的对象克隆了出来**.

如果增加了readResolve()方法，这样当JVM从内存中反序列化地"组装"一个新对象时,就会自动调用这个 readResolve方法来返回我们指定好的对象了, 单例规则也就得到了保证. 

**代码：**

```java
public class SingletonWithSerializable implements Serializable {

    public static final SingletonWithSerializable INSTANCE = new SingletonWithSerializable();

    private SingletonWithSerializable() {
        if (INSTANCE != null) {
            throw new RuntimeException("Singleton instance already exists!!!");
        }
    }
		/**
		保证序列化的单例
		*/
//    private Object readResolve(){
//        return INSTANCE;
//    }

    public void leaveTheBuilding(){
        System.out.println("SingletonWithSerializable.leaveTheBuilding() called...");
    }

    public static void main(String[] args) throws IOException, ClassNotFoundException {
        SingletonWithSerializable instance=SingletonWithSerializable.INSTANCE;
        System.out.println("Before serialization: "+instance);

        try(ObjectOutputStream out=new ObjectOutputStream(new FileOutputStream("file1.ser"))){
            out.writeObject(instance);
        }
        try(ObjectInputStream in=new ObjectInputStream(new FileInputStream("file1.ser"))){
            SingletonWithSerializable readObject=(SingletonWithSerializable)in.readObject();
            System.out.println("After deserialization: "+readObject);
        }
    }
}
```

#### 3、声明一个包含单个元素的枚举类型

（简单安全，无需防止序列化中出现的问题以及多次实例化）这是一个实现单例的最佳方法，但是如果该单例类需要被扩展的话，不适合选择该方法（虽然可让枚举去实现接口且抽取变量）

**代码：**

```java
public enum  SingletonWithEnum {
    INSTANCE;

    public void leaveTheBuilding(){
        System.out.println("SingletonWithEnum.leaveTheBuilding() called...");
    }

    public static void main(String[] args) {
        SingletonWithEnum.INSTANCE.leaveTheBuilding();
    }
}
```



### Item04: Enforce noninstantiability with a private constructor 通过私有化构造器强制不可实例化

例如在JAVA提供的一些类中，如java.lang.Math、java.ytil.Arrays...等工具类，这些类我们只需调用其静态方法，大多数情况下是不需要实例化的。实例化对于这些工具类是毫无意义的。我们可以通过私有化其一个无参的构造方法且保证只有这个构造方法，这样就可以防止该类的初始化。也可以防止编译器生成缺省构造器。

代码：

```java
public class UtilityClass {

    //防止初始化且防止编译器生成缺省构造器
    private UtilityClass(){
        throw new AssertionError();
    }
}
```

