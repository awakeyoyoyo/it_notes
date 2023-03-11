## spring

#### spring是如何加载配置文件到应用程序的

> LoaderBeanDefinition->XmlBeanDefinitionReader->根据输入输出流->document元素解析->根据命名空间->找到不同的NameplaceHandler进行处理->生产BeanDefinition

#### 掌握核心接口BeanDefinitionReader

> 根据输入输出流->document元素解析->根据命名空间->找到不同的NameplaceHandler进行处理->生产BeanDefinition

#### 掌握核心接口BeanFactory

> 本质上是一个容器，是一个懒加载的ioc容器接口，在bean被使用的时候才会去初始化。

#### BeanPostProcessor接口的作用及实现

> 用于实现动态代理,提供before、after的方法，可以用于插手bean初始化过程中的流程

#### BeanFactoryPostProcessor接口的作用及实现

> 用于对于BeanFactory容器的后置处理器，主要对于BeanDefinition进行修改，具体的例子有PlaceHolderBeanFactoryPostProcessor用于处理配置文件中的占位符赋值
>
>
> 自定义注解配合BeanFactoryPostProcessor 可以实现自己的注解来实现@auturie功能

#### Srping Bean的实例化过程?

> 被getbean->在容器中查找是否有该bean->通过反射创建出对象->执行BeanPostProcessor的before方法->根据beanDefinition来对对象进行赋值->BeanPostProcessor的after方法

#### 理解Factory Bean接口

#### 为什么会产生循环依赖问题，以及spring是如何解决？

> 通过缓存的方式来解决依赖问题，一级缓存存储的是已经完成初始化的bean，二级缓存存储的是实例化完成但仍未初始化好的bean，三级缓存存储的是lambad表达式：用来创建代理的bean对象的方法。
>
> 首先假设如果有两个bean a b相互引用，创建a的时候先初始化好了a，然后放入二级缓存中，然后再注入b的时候发现b没有创建，则去循序渐进从一级缓存到二级缓存到三级缓存的寻找。没有然后再创建b，放入二级缓存中，然后注入a的时候就在二级缓存中找到，然后生成了一个完整的b对象，放入一级缓存，再来回调到a，将a完成初始化。
>
> 如果a，b是有使用到aop需要用代理对象的话，那么在创建完a对象后，将其放入二级缓存中，然后判断到a是代理对象，然后从二级缓存中移除a，然后将其代理对象创建的lambad表达式，放入三级缓存中，该匿名内部类方法的参数中存储着实例化好的a， 然后b从三级缓存中找到lambad表达式，创建出a的代理对象。
>
> 三级缓存的意义：代理对象和 原本的对象 相同名字但对象不一样，不能放到同一个缓存中。
>
> spring无法解决的循环引用有：构造器循环引用，因为spirng解决循环引用的方式是通过实例化和初始化分离把

#### IOC问题

#### cglib和jdk动态代理的机制

#### aop问题



> BeanFactory和ApplicationContext 是两个不同的容器



### 循环依赖

两个单例对象之间相互引用，在初始化任意一个对象的时候会涉及到另一个对象的初始化，两者之间相互需要初始化形成循环。



循环依赖的产生有两种形式：

- 构造器

​       循环依赖问题是无法解决的

- set

  通过三级缓存的来解决

#### 解决循环依赖的本质

> 实例化和初始化分割开来，把未完成初始化的bean，提前放入缓存中，方便后续来对象初始化的时候进行getBean操作来进行成员变量的赋值。
>
> 但未初始化的对象和已初始化的对象需要区分开来（未初始化对象不允许被获取来使用），所有需要有两个不同的map来存储



```java
public class DefaultSingletonBeanRegistry extends SimpleAliasRegistry implements SingletonBeanRegistry {

	/** Cache of singleton objects: bean name to bean instance. */
  // 一级缓存
	private final Map<String, Object> singletonObjects = new ConcurrentHashMap<>(256);

	/** Cache of singleton factories: bean name to ObjectFactory. */
  // 三级缓存
  // ObjectFactory 函数式接口 @FunctionalInterface 可以传递lambda表达式进行，可以在调用getObject来调用lambda表达式中的方法
	private final Map<String, ObjectFactory<?>> singletonFactories = new HashMap<>(16);

	/** Cache of early singleton objects: bean name to bean instance. */
  // 二级缓存
	private final Map<String, Object> earlySingletonObjects = new HashMap<>(16);
}
```

为什么需要三级缓存？

AOP为什么需要三级缓存？

##### 获取不到则进行创建Bean的lambda表达式

```java
sharedInstance = getSingleton(beanName, () -> {
   try {
      return createBean(beanName, mbd, args);
   }
   catch (BeansException ex) {
      // Explicitly remove instance from singleton cache: It might have been put there
      // eagerly by the creation process, to allow for circular reference resolution.
      // Also remove any beans that received a temporary reference to the bean.
      destroySingleton(beanName);
      throw ex;
   }
});
```

##### 提前暴露引用 lambda表达式

```java
//将实例化好的bean存储到匿名内部类的中的方法中
addSingletonFactory(beanName, () -> getEarlyBeanReference(beanName, mbd, bean));



protected Object getEarlyBeanReference(String beanName, RootBeanDefinition mbd, Object bean) {
   Object exposedObject = bean;
   if (!mbd.isSynthetic() && hasInstantiationAwareBeanPostProcessors()) {
      for (BeanPostProcessor bp : getBeanPostProcessors()) {
         if (bp instanceof SmartInstantiationAwareBeanPostProcessor) {
            SmartInstantiationAwareBeanPostProcessor ibp = (SmartInstantiationAwareBeanPostProcessor) bp;
            exposedObject = ibp.getEarlyBeanReference(exposedObject, beanName);
         }
      }
   }
   return exposedObject;
}
```



